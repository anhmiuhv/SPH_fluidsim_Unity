//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       A particle system, containing a List of ParticleEmitters

using System;
using System.Collections;
using UnityEngine;

public class ParticleSystem {
	
	private bool m_wasMaxReached;
	public ArrayList Particles;
	public ArrayList Emitters;
	public bool HasEmitters {
		get { return Emitters != null && Emitters.Count > 0; }
	}
	public ArrayList Consumers;
	public bool HasConsumers {
		get { return Consumers != null && Consumers.Count > 0; }
	}
	public int MaxLife;
	public int MaxParticles;
	public bool DoRebirth;
	
	public ParticleSystem() {
		Emitters        = new ArrayList();
		Consumers       = new ArrayList();
		MaxLife         = 1024;
		MaxParticles    = 500;
		DoRebirth       = true;
		Reset();
	}
	
	public void Reset() {
		Particles       = new ArrayList(MaxParticles);
		m_wasMaxReached = false;
	}
	
	public void Update(double dTime) {
		
		// Consume particles in a certain range
		if (this.HasConsumers) {
			for (int i = 0; i < Consumers.Count; i++) {
				FluidParticleConsumer consumer = (FluidParticleConsumer) Consumers[i];
				consumer.Consume(ref Particles);
            }
		}
		// Check if emit is allowed
		if (m_wasMaxReached && !DoRebirth) {
			// NOP
		}
		else if (this.Particles.Count < this.MaxParticles) {
			if (this.HasEmitters) {
				// Emit new particles
				for (int i = 0; i < Emitters.Count; i++) {
					FluidParticleEmitter emitter = (FluidParticleEmitter) Emitters[i];
					emitter.Emit(ref Particles, dTime);
				}
            }
		}
		else {
			m_wasMaxReached = true;
		}
	}
	
	public static ArrayList Create(int nParticles, float cellSpace, Rect domain, float particleMass) {
		ArrayList particles = new ArrayList(nParticles);
		// Init. Particle positions
		float x0 = domain.x + cellSpace;
		float x = x0;
		float y = domain.y;
		for (int i = 0; i < nParticles; i++) {
            if (x == x0) {
				y += cellSpace;
            }
            Vector2 pos = new Vector2(x, y);
			FluidParticle p = new FluidParticle();
			p.Position = pos;
			p.PositionOld = pos;
			p.Mass = particleMass;
			particles.Add(p);
            x = x + cellSpace < domain.width ? x + cellSpace : x0;
		}
		return particles;
	}
	
}

