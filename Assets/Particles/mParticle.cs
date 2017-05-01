//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a fluid particle

//

using System;
using System.Collections;
using UnityEngine;

public struct smallParticle
{
    public Vector2 Position;
    public smallParticle(Vector2 Position)
    {
        this.Position = Position;
    }

}

public class mParticle {
	
    
	public int Life;
	public float Mass;
	public Vector2 Position;
	public Vector2 PositionOld;
	public Vector2 Velocity;
    public float heat;
	public Vector2 Force;
	public float Density;
	public float Pressure;
    public Vector2 grid_index;
	
	public Solver Solver;
	public BoundingVolume BoundingVolume;
	
	public mParticle() {
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
		return (int)(Position.x * Constants.prime1) ^ (int)(Position.y * Constants.prime2);
	}
	
	public override bool Equals(object obj) {
		if (obj != null) {
            return obj.GetHashCode().Equals(this.GetHashCode());
		}
		return base.Equals(obj);
	}

    public smallParticle toE()
    {
        return new smallParticle(Position);
    }
	
	
}

