//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a fluid particle

//

using System;
using System.Collections;
using UnityEngine;

public class FluidParticle {
	
	public int Life;
	public float Mass;
	public Vector2 Position;
	public Vector2 PositionOld;
	public Vector2 Velocity;
	public Vector2 Force;
	public float Density;
	public float Pressure;
	
	public Solver Solver;
	public BoundingVolume BoundingVolume;
	
	public FluidParticle() {
		Mass            = 1.0f;
		Position        = Vector2.zero;
		PositionOld     = this.Position;
		Velocity        = Vector2.zero;
		Force           = Vector2.zero;
		Density         = Constants.DensityOffset;
		// update (integrate) using basic verlet with small drag
		Solver          = new Verlet();
		Solver.Damping  = Constants.ParticleDamping;//.001f;
		BoundingVolume  = new PointVolume();
		BoundingVolume.Position = this.Position;
		BoundingVolume.Margin = Constants.CellSpace * .25f;
        
		UpdatePressure();
	}
	
	public void UpdatePressure() {
		Pressure = Constants.GasConstant * (Density - Constants.DensityOffset);
	}
	
	public void Update(float dTime) {
		Life++;
		// integrate
		Solver.Solve(ref Position, ref PositionOld, ref Velocity, Force, Mass, dTime);
		// update bounding volume
		BoundingVolume.Position = Position;
	}
	
	public override int GetHashCode() {
		return (int)(Position.x * Constants.PrimeNumber01) ^ (int)(Position.y * Constants.PrimeNumber02);
	}
	
	public override bool Equals(object obj) {
		if (obj != null) {
            return obj.GetHashCode().Equals(this.GetHashCode());
		}
		return base.Equals(obj);
	}
	
	
}
