//   2d SPH Fluid Dynamics for Unity3d.
//  Module: Main script
//

using System;
using System.Collections;
using UnityEngine;

public class SPHUnity : MonoBehaviour {

	// Sim
	private SPHSimulation m_fluidSim;
	private Vector2 m_gravity;
	private ParticleSystem m_particleSystem;
	// BoundingVolumes
	private CollisionResolver m_collisionSolver;
//	private BoundingVolume m_selectedBoundingVolume;
	// Unity Stuff
	private UnityEngine.ParticleSystem ps;
	private UnityEngine.ParticleSystem.EmissionModule emitter;
	private ParticleSystemRenderer prenderer;
	private UnityEngine.ParticleSystem.Particle[] particles;
	private GameObject cam;
	private Material defaultmat;
	private Material noglowmat;
	
	public void Awake() {
		// Init simulation
		m_gravity = Constants.Gravity * Constants.ParticleMass;
		m_fluidSim = new SPHSimulation(Constants.CellSpace, Constants.SimulationDomain);
		m_collisionSolver = new CollisionResolver();
		m_collisionSolver.Bounciness = 0.2f;
		m_collisionSolver.Friction   = 0.01f;

		// Init. particle system
		m_particleSystem = new ParticleSystem();
		float freq = 40;
		int maxPart = 100;
		m_particleSystem.MaxParticles = maxPart;
		m_particleSystem.MaxLife = (int)((double)maxPart / freq / Constants.TimeStepSeconds);
				
		// Add Emitter
		FluidParticleEmitter emitter = new FluidParticleEmitter();
		emitter.Position       = new Vector2(Constants.SimulationDomain.xMax/2,Constants.SimulationDomain.yMax-Constants.SimulationDomain.yMax/8);
		emitter.VelocityMin    = 2.5f;//Constants.ParticleMass * 0.30f;
		emitter.VelocityMax    = 3.0f;//Constants.ParticleMass * 0.35f;
		emitter.Direction      = new Vector2(0.8f, -0.25f);
		emitter.Distribution   = Constants.SimulationDomain.width * 0.1f;
		emitter.Frequency      = freq;
		emitter.ParticleMass   = Constants.ParticleMass;
		m_particleSystem.Emitters.Add(emitter);
				
		print (
			   "SimulationDomain: (" + Constants.SimulationDomain.width + "," + Constants.SimulationDomain.height + ")" +
			   " Grid: " + m_fluidSim.m_grid.Count + " (" + m_fluidSim.m_grid.Width + "," + m_fluidSim.m_grid.Height + ")" +
			   " CellSpace: " + Constants.CellSpace + " "
		);
		particles = new UnityEngine.ParticleSystem.Particle[1000];
	}
	
	public void Start() {
		
//		float x = Constants.SimulationDomain.width / 2;
//		float y = 0;
//		float w = Constants.SimulationDomain.width / 8;
//		float h = Constants.SimulationDomain.height / 7;
//
//		OBB obb = new OBB();
//		obb.Position = new Vector2(x, y);
// 		obb.Extents  = new Vector2(w, h);
//		m_collisionSolver.BoundingVolumes.Add( obb );		
		

		ps = GetComponent<UnityEngine.ParticleSystem> ();
		emitter = ps.emission;
		// add Unity Particle Renderer
		prenderer = GetComponent<ParticleSystemRenderer>();
		prenderer.material.color = new Color(1f,.817f,.39f,1);
		defaultmat = prenderer.material;
		noglowmat = prenderer.material;
		
		// find cam
		cam = GameObject.Find("Main Camera");
		// fps stuff
		timeleft = updateInterval;
	}
	

	public void FixedUpdate() {
		// Solve collisions only for obbs (not particles)
		m_collisionSolver.Solve();
		
		// Update particle system
		m_particleSystem.Update(Constants.TimeStepSeconds);
		
		// Solve collisions only for particles
		m_collisionSolver.Solve(ref m_particleSystem.Particles);

		// Do simulation
		m_fluidSim.Calculate(ref m_particleSystem.Particles, m_gravity, Constants.TimeStepSeconds);


		// align Unity Particles with simulated Particles ...
		int d = m_particleSystem.Particles.Count - ps.particleCount;
		if (d > 0)
			ps.Emit(d);
		
		ps.GetParticles (particles);
		FluidParticle p;
		for (int i = 0; i < m_particleSystem.Particles.Count; i++) {
			p = (FluidParticle) m_particleSystem.Particles[i];
			particles[i].position = new Vector3(p.Position.x, p.Position.y, 0);
			particles[i].color = Color.white;
			particles[i].remainingLifetime = 1f;
		}
		ps.SetParticles (particles, m_particleSystem.MaxParticles);
	}
	
	
	private float UItime = 0;
	private float UItimeNext = 0.5f;

	public void OnGUI() {
		
		GUI.Box(new Rect(0,0,220,360),"");
		
		GUI.Label(new Rect(11,0,80,20), fpsstr);
		
		if (GUI.Button(new Rect(10,20,200,20), "Clear Particles") && Time.time >= UItime) {
			ps.Clear ();
			m_particleSystem.Reset();
			UItime = Time.time + UItimeNext;
		}
		
		if (GUI.Button(new Rect(10,50,200,20), "Switch Glow Effect") && Time.time >= UItime) {
			MonoBehaviour glow = (MonoBehaviour) cam.GetComponent("GlowEffect");
			
			if (glow.enabled) {
				glow.enabled = false;
				prenderer.material = noglowmat;
			}
			else {
				glow.enabled = true;
				prenderer.material = defaultmat;
			}
			UItime = Time.time + UItimeNext;
		}

		m_particleSystem.MaxParticles =
			(int) GUI.HorizontalSlider(new Rect(10,100,200,20), m_particleSystem.MaxParticles, 0, 500);
		
		int pcount = m_particleSystem.Particles.Count;
		int pmax = m_particleSystem.MaxParticles;
		GUI.Label(new Rect(10, 80, 200, 20), pcount + " of " + pmax + " Particles");
		
		if (pcount > pmax) {
			for (int i = pcount-1; i >= pmax; i--) {
				m_particleSystem.Particles.RemoveAt(i);
			}
			ps.Clear ();
		}
		
		float pmass = Constants.ParticleMass;
		pmass = GUI.HorizontalSlider(new Rect(10,140,200,20), pmass, 0.001f, 50);
		if (Constants.ParticleMass != pmass) {
			Constants.ParticleMass = pmass;
			m_gravity = Constants.Gravity * Constants.ParticleMass;
			((FluidParticleEmitter)m_particleSystem.Emitters[0]).ParticleMass = Constants.ParticleMass;
			foreach (FluidParticle particle in m_particleSystem.Particles) {
				particle.Mass = Constants.ParticleMass;
			}
		}
		GUI.Label(new Rect(10, 120, 200, 20), "Particle Mass: " + pmass);
		
		m_collisionSolver.Bounciness =
			GUI.HorizontalSlider(new Rect(10,180,200,20), m_collisionSolver.Bounciness, 0, 10);
		GUI.Label(new Rect(10, 160, 200, 20), "Bounciness: " + m_collisionSolver.Bounciness);
		
		float damp =
			GUI.HorizontalSlider(new Rect(10,220,200,20), Constants.ParticleDamping, 0, 0.1f);
		GUI.Label(new Rect(10, 200, 200, 20), "Damping: " + Constants.ParticleDamping);
		if (Constants.ParticleDamping != damp) {
			foreach (FluidParticle particle in m_particleSystem.Particles) {
				particle.Solver.Damping = damp;
			}
			Constants.ParticleDamping = damp;
		}
		
		float gravity = Constants.Gravity.y*-1;
		gravity =
			GUI.HorizontalSlider(new Rect(10,260,200,20), gravity, 0, 20);
		GUI.Label(new Rect(10, 240, 200, 20), "Gravity: " + gravity);
		Constants.Gravity = new Vector2(0, gravity*-1);
		m_gravity = Constants.Gravity * Constants.ParticleMass;
		
		m_fluidSim.Viscosity =
			GUI.HorizontalSlider(new Rect(10,300,200,20), m_fluidSim.Viscosity, 0, 10);
		GUI.Label(new Rect(10, 280, 200, 20), "Viscosity: " + m_fluidSim.Viscosity);
		
		Constants.GasConstant =
			GUI.HorizontalSlider(new Rect(10,340,200,20), Constants.GasConstant, 0, 10);
		GUI.Label(new Rect(10, 320, 200, 20), "GasConstant: " + Constants.GasConstant);
		
	}

	// some fps fragment stolen from
	// http://www.unifycommunity.com/wiki/index.php?title=FramesPerSecond
	
	public  float updateInterval = 0.5F;
	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	private string fpsstr; // output string
	
	public void Update() {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			fpsstr = format;
			
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
		
	}
}

