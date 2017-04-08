//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Ordinary differential equation solver using an explicit euler integration


using System;
using System.Collections;
using UnityEngine;

public class EulerExplicit : Solver {
	
	public override void Solve(ref Vector2 position, ref Vector2 positionOld, ref Vector2 velocity, Vector2 acceleration, float timeStep) {
		Vector2 t;
		positionOld = position;
		// Calc new position
		// x = x + v * dt = x + x' * dt
		t = velocity * timeStep;
		t = t * (1.0f - Damping);
		position = position + t;
		// Calc new velocity
		// v = v + a * dt = v + v' * dt; a = f / m
		t = acceleration * timeStep;
		velocity = velocity + t;
	}
	
	
	public EulerExplicit() {
	}
	
}

