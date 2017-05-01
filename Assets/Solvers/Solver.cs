//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Base class of a ordinary differential equation solver
//

//

using System;
using System.Collections;
using UnityEngine;

public abstract class Solver {
	
	public float Damping;
	
	public Solver() {
		Damping = 1.0f;
	}
	
	public abstract void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 acceleration, float timeStep);
	
	public virtual void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 force, float mass, float timeStep) {
		Solve(ref position, ref positionOld, ref velocity, force / mass, timeStep);
	}
}

