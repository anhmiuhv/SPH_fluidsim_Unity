//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       A particle emitter
//

//

using System;
using System.Collections;
using UnityEngine;

public class FluidParticleEmitter {
	
	private UnityEngine.Random m_randGen;
	private Vector2 m_direction;
	private double m_time;
	
	public Vector2 Position;
	public Vector2 Direction
	{
		get { return m_direction; }
		set
		{
            m_direction = value;
			Vector3 dir = new Vector3(m_direction.x, 0, m_direction.y);
            dir.Normalize();
			m_direction = new Vector2(dir.x, dir.z);
		}
	}
	public float Distribution;
	public float VelocityMin;
	public float VelocityMax;
	public double Frequency;
	public float ParticleMass;
	public bool Enabled;
	
	public FluidParticleEmitter() {
		Position        = Vector2.zero;
		VelocityMin     = 0.0f;
		VelocityMax     = this.VelocityMin;
		Direction       = Vector2.up;
		Distribution    = 1.0f;
		Frequency       = 128.0f;
		ParticleMass    = 1.0f;
		Enabled         = true;
	}
	
	public void Emit(ref ArrayList particles, double dTime) {
		if (this.Enabled) {
            // Calc particle count based on frequency
            m_time += dTime;
            int nParts = (int)(this.Frequency * m_time);
            if (nParts > 0) {
				// Create Particles
				for (int i = 0; i < nParts; i++) {
					// Calc velocity based on the distribution along the normalized direction
					float dist = UnityEngine.Random.value * Distribution - Distribution * 0.5f;
					Vector2 normal = new Vector2(Direction.y, -Direction.x);
					normal = normal * dist;
					Vector2 vel = Direction + normal;
					Vector3 vel3 = new Vector3(vel.x, 0, vel.y);
					vel3.Normalize();
					vel = new Vector2(vel3.x, vel3.z);
					float velLen = UnityEngine.Random.value * (this.VelocityMax - this.VelocityMin) + this.VelocityMin;
					vel = vel * velLen;
					// Calc Oldpos (for right velocity) using simple euler
					// oldPos = this.Position - vel * m_time;
					Vector2 oldPos = this.Position - vel * (float)m_time;
					FluidParticle f = new FluidParticle();
					f.Position    = Position;
					f.PositionOld = oldPos;
					f.Velocity    = vel;
					f.Mass        = ParticleMass;
					particles.Add(f);
				}
				// Reset time
				m_time = 0.0;
            }
		}
	}
}

