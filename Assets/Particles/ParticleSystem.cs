//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       A particle system, containing a List of ParticleEmitters

using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystem {
	
	private bool m_wasMaxReached;
	public List<mParticle> Particles;
	public List<ParticleEmitter> Emitters;
	public bool HasEmitters {
		get { return Emitters != null && Emitters.Count > 0; }
	}
	public List<ParticleConsummer> Consumers;
	public bool HasConsumers {
		get { return Consumers != null && Consumers.Count > 0; }
	}
	public int MaxLife;
	public int MaxParticles;
	public bool DoRebirth;
	
	public ParticleSystem() {
		Emitters        = new List<ParticleEmitter>();
		Consumers       = new List<ParticleConsummer>();
		MaxLife         = 1024;
		MaxParticles    = 1000;
		DoRebirth       = true;
		Reset();
	}
	
	public void Reset() {
		Particles       = new List<mParticle>(MaxParticles);
		m_wasMaxReached = false;
	}
	
	public void Update(double dTime) {
		
		// Consume particles in a certain range
		if (this.HasConsumers) {
			for (int i = 0; i < Consumers.Count; i++) {
				ParticleConsummer consumer = Consumers[i];
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
					ParticleEmitter emitter =  Emitters[i];
					emitter.Emit(ref Particles, dTime);
				}
            }
		}
		else {
			m_wasMaxReached = true;
		}
	}
	
	public static List<mParticle> Create(int nParticles, float cellSpace, Rect domain, float particleMass) {
		List<mParticle> particles = new List<mParticle>(nParticles);
		// Init. Particle positions
		float x0 = domain.x + cellSpace;
		float x = x0;
		float y = domain.y;
		for (int i = 0; i < nParticles; i++) {
            if (x == x0) {
				y += cellSpace;
            }
            Vector2 pos = new Vector2(x, y);
			mParticle p = new mParticle();
			p.Position = pos;
			p.PositionOld = pos;
			p.Mass = particleMass;
			particles.Add(p);
            x = x + cellSpace < domain.width ? x + cellSpace : x0;
		}
		return particles;
	}
	
}

