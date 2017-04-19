//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       A particle emitter
//

//

using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter {
	
	private Vector2 direction;
	private double time;
	
	public Vector2 Position;
	public Vector2 Direction
	{
		get { return direction; }
		set
		{
            direction = value;
			Vector3 dir = new Vector3(direction.x, 0, direction.y);
            dir.Normalize();
			direction = new Vector2(dir.x, dir.z);
		}
	}
	public float Distribution;
	public float VelocityMin;
	public float VelocityMax;
	public double Frequency;
	public float ParticleMass;
	public bool Enabled;
	
	public ParticleEmitter() {
		Position        = Vector2.zero;
		VelocityMin     = 0.0f;
		VelocityMax     = this.VelocityMin;
		Direction       = Vector2.up;
		Distribution    = 1.0f;
		Frequency       = 128.0f;
		ParticleMass    = 1.0f;
		Enabled         = true;
	}
	
	public void Emit(ref List<mParticle> particles, double dTime) {
		if (this.Enabled) {
            // Calc particle count based on frequency
            time += dTime;
            int nParts = (int)(this.Frequency * time);
            if (nParts > 0) {
				// Create Particles
				for (int i = 0; i < nParts; i++) {
					// Calc velocity based on the distribution along the normalized direction
					float dist = UnityEngine.Random.value * Distribution - Distribution * 0.5f;
					Vector2 normal = new Vector2(Direction.y, -Direction.x);
					normal = normal * dist;
					Vector2 vel = Direction + normal;
					vel.Normalize ();
					float velLen = UnityEngine.Random.value * (this.VelocityMax - this.VelocityMin) + this.VelocityMin;
					vel = vel * velLen;
					// Calc Oldpos (for right velocity) using simple euler
					// oldPos = this.Position - vel * m_time;
					Vector2 oldPos = this.Position - vel * (float)time;
					mParticle p = new mParticle();
					p.Position    = Position;
					p.PositionOld = oldPos;
					p.Velocity    = vel;
					p.Mass        = ParticleMass;
					particles.Add(p);
				}
				// Reset time
				time = 0.0;
            }
		}
	}
}

