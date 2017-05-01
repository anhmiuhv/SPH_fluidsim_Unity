//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       A particle consumer, which removes particles in a certain radius.


using System;
using System.Collections;using System.Collections.Generic;
using UnityEngine;

public class ParticleConsummer {
	
	private float m_radiusSquared;
	public Vector2 Position;
	public float Radius
	{
		get { return (float)Math.Sqrt(m_radiusSquared); }
		set
		{
            m_radiusSquared = value * value;
		}
	}
	public bool Enabled;
	
	public ParticleConsummer() {
		Position     = Vector2.zero;
		Radius       = 1.0f;
		Enabled      = true;
	}
	
	public void Consume(ref List<mParticle> particles) {
		if (this.Enabled) {
            for (int i = particles.Count - 1; i >= 0;i--) {
				float distSq = ((particles[i]).Position - this.Position).sqrMagnitude;
				if (distSq < m_radiusSquared) {
					particles.RemoveAt(i);
				}
            }
		}
	}
}

