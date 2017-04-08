//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//

using System;
using System.Collections;
using UnityEngine;

public class SPHSimulation {

	public IndexGrid m_grid;
	public float CellSpace;
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
		Viscosity    = Constants.Viscosity;
		m_grid       = new IndexGrid(cellSpace, domain);
		SKGeneral    = new SKPoly6(cellSpace);
		SKPressure   = new SKSpiky(cellSpace);
		SKViscosity  = new SKViscosity(cellSpace);
	}
	
	public void Calculate(ref ArrayList particles, Vector2 globalForce, float dTime) {
		m_grid.Refresh(ref particles);
		CalculatePressureAndDensities(ref particles, ref m_grid);
		CalculateForces(ref particles, ref m_grid, globalForce);
		UpdateParticles(ref particles, dTime);
		CheckParticleDistance(ref particles, ref m_grid);
	}
	
	private void CalculatePressureAndDensities(ref ArrayList particles, ref IndexGrid grid) {
		Vector2 dist;
		foreach (FluidParticle particle in particles) {
            particle.Density = 0.0f;
            foreach (int nIdx in grid.GetNeighbourIndex(particle)) {
				if (particle != ((FluidParticle) particles[nIdx])) {
					dist = particle.Position - ((FluidParticle) particles[nIdx]).Position;
					particle.Density += particle.Mass * this.SKGeneral.Calculate(ref dist);
				}
            }
            particle.UpdatePressure();
		}
	}
	
	private void CalculateForces(ref ArrayList particles, ref IndexGrid grid, Vector2 globalForce) {
		Vector2 f, dist;
		float scalar;
		for (int i = 0; i < particles.Count; i++) {
			FluidParticle p = (FluidParticle) particles[i];
            // Add global force to every particle
            p.Force += globalForce;

            foreach (int nIdx in grid.GetNeighbourIndex(p)) {
				// Prevent double tests
				if (nIdx < i) {
					FluidParticle pn = (FluidParticle) particles[nIdx];
					if (pn.Density > Constants.SingleEpsilon && p != pn) {
						dist = p.Position - pn.Position;

						// pressure
						// f = particles[nIdx].Mass * ((particles[i].Pressure + particles[nIdx].Pressure) / (2.0f * particles[nIdx].Density)) * WSpikyGrad(ref dist);
						//scalar   = pn.Mass * (p.Pressure + pn.Pressure) / (2.0f * pn.Density);
						//f        = SKPressure.CalculateGradient(ref dist);
						//f        = f * scalar;
						//p.Force    -= f;
						//pn.Force   += f;

						// viscosity
						// f = particles[nIdx].Mass * ((particles[nIdx].Velocity - particles[i].Velocity) / particles[nIdx].Density) * WViscosityLap(ref dist) * Constants.VISC0SITY;
						scalar   = pn.Mass * this.SKViscosity.CalculateLaplacian(ref dist) * Viscosity * 1 / pn.Density;
						f        = pn.Velocity - p.Velocity;
						f = f * scalar;
						p.Force    += f;
						pn.Force   -= f;
					}
				}
            }
		}
	}
	
	private void UpdateParticles(ref ArrayList particles, float dTime) {
				
		float r = Domain.xMax;
		float l = Domain.x;
		// Rectangle contains coordinates inverse on y
		float t = Domain.yMax;
		float b = Domain.y;
		
		for (int i = 0; i < particles.Count; i++) {
			FluidParticle particle = (FluidParticle) particles[i];
			// Update velocity + position using forces
			particle.Update(dTime);
            // Clip positions to domain space
            if (particle.Position.x < l) {
				particle.Position.x = l + Constants.SingleEpsilon;
            }
            else if (particle.Position.x > r) {
				particle.Position.x = r - Constants.SingleEpsilon;
            }
            if (particle.Position.y < b) {
				particle.Position.y = b + Constants.SingleEpsilon;
            }
            else if (particle.Position.y > t) {
				particle.Position.y = t - Constants.SingleEpsilon;
            }
            
            // Reset force
            particle.Force = Vector2.zero;
		}
	}
	
	private void CheckParticleDistance(ref ArrayList particles, ref IndexGrid grid) {
		float minDist = .5f * CellSpace;
		float minDistSq = minDist * minDist;
		Vector2 dist;
		for (int i = 0; i < particles.Count; i++) {
			FluidParticle p = (FluidParticle) particles[i];
            foreach (int nIdx in grid.GetNeighbourIndex(p)) {
				FluidParticle pn = (FluidParticle) particles[nIdx];
				if (p != pn) {
					dist = pn.Position - p.Position;
					float distLenSq = dist.sqrMagnitude;
					if (distLenSq < minDistSq) {
						if (distLenSq > Constants.SingleEpsilon) {
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

