//   SPH Fluid Dynamics for Unity3d.
//

//	Implementationtion reference from http://rlguy.com/sphfluidsim/index.html
//  and many papers
//
//
//   Modul:             Fluid physics
//

using System;
using System.Collections.Generic;
using UnityEngine;

public class SPHSimulation {

	public Grid m_grid;
	public float CellSpace;
	public float CellSpace2 {
		get {
			return CellSpace * CellSpace;
		}
	}
	public Rect Domain;
	public SmoothingKernel SKGeneral;
	public SmoothingKernel SKPressure;
	public SmoothingKernel SKViscosity;
	public float Viscosity;
	
	public SPHSimulation() {
		CellSpace = 8;
		Domain = new Rect(0,0,256,256);
	}
	
	public SPHSimulation(float cellSpace, Rect domain) {
		CellSpace    = cellSpace;
		Domain       = domain;
		m_grid       = new Grid(cellSpace, domain);
		SKGeneral    = new SKPoly6(cellSpace);
		SKPressure   = new SKSpiky(cellSpace);
		SKViscosity  = new SKViscosity(cellSpace);
	}
	
	public void Calculate(ref List<mParticle> particles, Vector2 globalForce, float dTime) {
		Viscosity    = Constants.mRadialViscosityGain;

		m_grid.Refresh(ref particles);
		PressureAndDensity(ref particles, ref m_grid);
		Forces(ref particles, ref m_grid, globalForce);
		UpdateParticles(ref particles, dTime);
		NoOverlapParticles(ref particles, ref m_grid);
	}
	
	private void PressureAndDensity(ref List<mParticle> particles, ref Grid grid) {
		Vector2 dist;
        int index = 0;
        List<int> list;
        int nIdx;

        foreach (mParticle particle in particles) {
            particle.Density = 0.0f;
            list = grid.GetNeighbourIndex(particle, index);
            for (int i = 0; i < list.Count; i++) {
                nIdx = list[i];
				if (particle != ( particles[nIdx])) {
					dist = particle.Position - ( particles[nIdx]).Position;
					particle.Density += particle.Mass * this.SKGeneral.Calculate(ref dist);
				}
            }
            particle.UpdatePressure();
            index++;
		}
	}
	
	private void Forces(ref List<mParticle> particles, ref Grid grid, Vector2 globalForce) {
		Vector2 f, dist;
		float scalar;
        List<int> list;
        int nIdx;
        for (int i = 0; i < particles.Count; i++) {
			mParticle p = particles[i];
            // Add global force to every particle
            p.Force += globalForce;

            list = grid.GetNeighbourIndex(p, i);
            for (int j = 0; j < list.Count; j++)
            {
                nIdx = list[j];
                if (nIdx < i) {
					mParticle pn =  particles[nIdx];
					if (pn.Density > Constants.SmallEpsilon && p != pn) {
						dist = p.Position - pn.Position;

						// pressure

						scalar   = pn.Mass * (p.Pressure + pn.Pressure) / (2.0f * pn.Density);
						f        = SKPressure.CalculateGradient(ref dist);
						f        = f * scalar;
						p.Force    -= f;
						pn.Force   += f;

						// viscosity
						// f = particles[nIdx].Mass * ((particles[nIdx].Velocity - particles[i].Velocity) / particles[nIdx].Density) * WViscosityLap(ref dist) * Constants.VISC0SITY;

						Vector2 velA = p.Velocity;
						float dist2   = dist.sqrMagnitude ;
						if( dist2 < CellSpace2 )
						{   // Particles are near enough to exchange velocity.
							float length    = Mathf.Sqrt( dist2 ) ;
							Vector2 sepDir  = dist / length ;
							Vector2 velB    = pn.Velocity ;
							Vector2 velDiff = velA - velB ;
							float velSep  = Vector2.Dot(velDiff ,sepDir) ;

							if( velSep < 0.0f )

							{   // Particles are approaching.
								float infl         = 1.0f - length / CellSpace ;
								float velSepA      = Vector2.Dot(velA,sepDir) ;                           // vel of pcl A along sep dir.
								float velSepB      = Vector2.Dot(velB ,sepDir) ;                           // vel of pcl B along sep dir.
								float velSepTarget = ( velSepA + velSepB ) * 0.5f ;            // target vel along sep dir.
								float diffSepA     = velSepTarget - velSepA ;                  // Diff btw A's vel and target.
								float changeSepA   = Viscosity * diffSepA * infl ;  // Amount of vel change to appl
								Vector2  changeA      = changeSepA * sepDir ;                     // Velocity change to apply.

								p.Force += (changeA  - Constants.Friction * p.Velocity) * p.Mass;                                                    // Apply velocity change to A.

								pn.Force -= (changeA - Constants.Friction * p.Velocity) * p.Mass ;                                                    // Apply commensurate change to B.

							}

						
						}

					}
				}
            }
		}
	}
	
	private void UpdateParticles(ref List<mParticle> particles, float dTime) {
				
		float r = Domain.xMax;
		float l = Domain.xMin;
		// Rectangle contains coordinates inverse on y
		float t = Domain.yMax;
		float b = Domain.yMin;
		
		for (int i = 0; i < particles.Count; i++) {
			mParticle particle = particles[i];
			// Update velocity + position using forces
			particle.Update(dTime);
            // Clip positions to domain space
            if (particle.Position.x < l) {
				particle.Position.x = l + Constants.SmallEpsilon;
            }
            else if (particle.Position.x > r) {
				particle.Position.x = r - Constants.SmallEpsilon;
            }
            if (particle.Position.y < b) {
				particle.Position.y = b + Constants.SmallEpsilon;
            }
            else if (particle.Position.y > t) {
				particle.Position.y = t - Constants.SmallEpsilon;
            }
            
            // Reset force
            particle.Force = Vector2.zero;
		}
	}
	
	private void NoOverlapParticles(ref List<mParticle> particles, ref Grid grid) {
		float minDist = .5f * CellSpace;
		float minDistSq = minDist * minDist;
		Vector2 dist;
        List<int> list;
        int nIdx;
        for (int i = 0; i < particles.Count; i++) {
			mParticle p =  particles[i];
            list = grid.GetNeighbourIndex(p, i);
            for (int j = 0; j < list.Count; j++)
            {
                nIdx = list[j];
                mParticle pn =  particles[nIdx];
				if (p != pn) {
					dist = pn.Position - p.Position;
					float distLenSq = dist.sqrMagnitude;
					if (distLenSq < minDistSq) {
						if (distLenSq > Constants.SmallEpsilon) {
							float distLen = (float)Math.Sqrt((double)distLenSq);
							dist = dist * 0.5f * (distLen - minDist) / distLen;
							pn.Position = pn.Position - dist;
							pn.PositionOld = pn.PositionOld - dist;
							p.Position = p.Position + dist;
							p.PositionOld = p.PositionOld + dist;
						}
						else
						{
							float diff = 0.5f * minDist;
							pn.Position.x       -= diff;
							pn.PositionOld.y    -= diff;
							p.Position.x        += diff;
							p.PositionOld.y     += diff;
						}
					}
				}
            }
		}
	}
	
}

