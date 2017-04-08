//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//



using System;
using System.Collections;
using UnityEngine;

public class Verlet : Solver {
	
	public Verlet() {
	}
	
	public override void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 acceleration, float timeStep) {
		Vector2 t;
		Vector2 oldPos = position;
		// Position = Position + (1.0f - Damping) * (Position - PositionOld) + dt * dt * a;
		acceleration = acceleration * (timeStep * timeStep);
		t = position - positionOld;
		t = t * (1.0f - Damping);
		t = t + acceleration;
		position = position + t;
		positionOld = oldPos;
		// calculate velocity
		// Velocity = (Position - PositionOld) / dt;
		t = position - positionOld;
		velocity = t / timeStep;
	}
	
}

