//   2d SPH Fluid Dynamics for Unity3d.
//  Module: Main script
//

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main: MonoBehaviour {

	// Sim
	private MeshGenerator generator= new MeshGenerator();
	private MeshFilter filter;
	private SPHSimulation fluidSim;
	private Vector2 lGravity;
	private ParticleSystem particleSystem;
	// BoundingVolumes
	private CollisionSolver collisionSolver;
//	private BoundingVolume m_selectedBoundingVolume;
	// Unity Stuff
	private UnityEngine.ParticleSystem ps;
	private UnityEngine.ParticleSystem.EmissionModule emitter;
	private ParticleSystemRenderer prenderer;
	private UnityEngine.ParticleSystem.Particle[] particles;
	private GameObject cam;
	private Material defaultmat;
	public void Awake() {
		// Init simulation
		lGravity = Constants.Gravity * Constants.ParticleMass;
		fluidSim = new SPHSimulation(Constants.CellSpace, Constants.SimulationDomain);
		collisionSolver = new CollisionSolver();
		collisionSolver.Bounciness = 0.2f;
		collisionSolver.Friction   = 0.01f;

		// Init. particle system
		particleSystem = new ParticleSystem();
		float freq = 40;
		int maxPart = 100;
		particleSystem.MaxParticles = maxPart;
		particleSystem.MaxLife = (int)((double)maxPart / freq / Constants.TimeStepSeconds);
				
		// Add Emitter
		ParticleEmitter emitter = new ParticleEmitter();
		emitter.Position       = new Vector2(Constants.SimulationDomain.xMax/2,Constants.SimulationDomain.yMax-Constants.SimulationDomain.yMax/8);
		emitter.VelocityMin    = 2.5f;//Constants.ParticleMass * 0.30f;
		emitter.VelocityMax    = 3.0f;//Constants.ParticleMass * 0.35f;
		emitter.Direction      = new Vector2(0.8f, -0.25f);
		emitter.Distribution   = Constants.SimulationDomain.width * 0.1f;
		emitter.Frequency      = freq;
		emitter.ParticleMass   = Constants.ParticleMass;
		particleSystem.Emitters.Add(emitter);
				
		print (
			   "SimulationDomain: (" + Constants.SimulationDomain.width + "," + Constants.SimulationDomain.height + ")" +
			   " Grid: " + fluidSim.m_grid.Count + " (" + fluidSim.m_grid.Width + "," + fluidSim.m_grid.Height + ")" +
			   " CellSpace: " + Constants.CellSpace + " "
		);
		particles = new UnityEngine.ParticleSystem.Particle[1000];
		filter = GetComponent<MeshFilter> ();

	}
	
	public void Start() {

		

		ps = GetComponent<UnityEngine.ParticleSystem> ();
        var main = ps.main;
        main.startSize = Constants.Radius;
		emitter = ps.emission;
		// add Unity Particle Renderer
		prenderer = GetComponent<ParticleSystemRenderer>();
		prenderer.material.color = new Color(1f,1f,1f,1);
		defaultmat = prenderer.material;

		
		// find cam
		cam = GameObject.Find("Main Camera");
		// fps stuff
		timeleft = updateInterval;
	}
	

	public void FixedUpdate() {
		// Solve collisions only for obbs (not particles)
		collisionSolver.Solve();
		
		// Update particle system
		particleSystem.Update(Constants.TimeStepSeconds);

		// Solve collisions only for particles
		collisionSolver.Solve(ref particleSystem.Particles);

		// Do simulation
		fluidSim.Calculate(ref particleSystem.Particles, lGravity, Constants.TimeStepSeconds);

		Mesh mesh = generator.GenerateMesh (fluidSim.m_grid.getFluidMapCount (), Constants.CellSpace);
		filter.mesh = mesh;

		// align Unity Particles with simulated Particles ...
		int d = particleSystem.Particles.Count - ps.particleCount;
		if (d > 0)
			ps.Emit(d);
		
		ps.GetParticles (particles);
		mParticle p;
		for (int i = 0; i < particleSystem.Particles.Count; i++) {
			p =  particleSystem.Particles[i];
			particles[i].position = new Vector3(p.Position.x, p.Position.y, 0);
			particles[i].remainingLifetime = 1f;
		}
		ps.SetParticles (particles, particleSystem.MaxParticles);

	}
	
	
	private float UItime = 0;
	private float UItimeNext = 0.5f;

	public void OnGUI() {
		
		GUI.Box(new Rect(0,0,220,360),"");
		
		GUI.Label(new Rect(11,0,80,20), fpsstr);
		
		if (GUI.Button(new Rect(10,20,200,20), "Clear Particles") && Time.time >= UItime) {
			ps.Clear ();
			particleSystem.Reset();
			UItime = Time.time + UItimeNext;
		}

		Constants.mRadialViscosityGain =
			GUI.HorizontalSlider(new Rect(10,60,200,20), Constants.mRadialViscosityGain, 0, 10f);
		GUI.Label(new Rect(10, 40, 200, 20), "Radial Vicousity Gain: " + Constants.mRadialViscosityGain);


		particleSystem.MaxParticles =
			(int) GUI.HorizontalSlider(new Rect(10,100,200,20), particleSystem.MaxParticles, 0, 1000);
		
		int pcount = particleSystem.Particles.Count;
		int pmax = particleSystem.MaxParticles;
		GUI.Label(new Rect(10, 80, 200, 20), pcount + " of " + pmax + " Particles");
		
		if (pcount > pmax) {
			for (int i = pcount-1; i >= pmax; i--) {
				particleSystem.Particles.RemoveAt(i);
			}
			ps.Clear ();
		}
		
		float pmass = Constants.ParticleMass;
		pmass = GUI.HorizontalSlider(new Rect(10,140,200,20), pmass, 0.001f, 50);
		if (Constants.ParticleMass != pmass) {
			Constants.ParticleMass = pmass;
			lGravity = Constants.Gravity * Constants.ParticleMass;
			((ParticleEmitter)particleSystem.Emitters[0]).ParticleMass = Constants.ParticleMass;
			foreach (mParticle particle in particleSystem.Particles) {
				particle.Mass = Constants.ParticleMass;
			}
		}
		GUI.Label(new Rect(10, 120, 200, 20), "Particle Mass: " + pmass);
		
		collisionSolver.Bounciness =
			GUI.HorizontalSlider(new Rect(10,180,200,20), collisionSolver.Bounciness, 0, 10);
		GUI.Label(new Rect(10, 160, 200, 20), "Bounciness: " + collisionSolver.Bounciness);
		
		float damp =
			GUI.HorizontalSlider(new Rect(10,220,200,20), Constants.ParticleDamping, 0, 0.1f);
		GUI.Label(new Rect(10, 200, 200, 20), "Damping: " + Constants.ParticleDamping);
		if (Constants.ParticleDamping != damp) {
			foreach (mParticle particle in particleSystem.Particles) {
				particle.Solver.Damping = damp;
			}
			Constants.ParticleDamping = damp;
		}
		
		float gravity = Constants.Gravity.y*-1;
		gravity =
			GUI.HorizontalSlider(new Rect(10,260,200,20), gravity, 0, 20);
		GUI.Label(new Rect(10, 240, 200, 20), "Gravity: " + gravity);
		Constants.Gravity = new Vector2(0, gravity*-1);
		lGravity = Constants.Gravity * Constants.ParticleMass;
		

		
		Constants.GasConstant =
			GUI.HorizontalSlider(new Rect(10,300,200,20), Constants.GasConstant, 0, 10);
		GUI.Label(new Rect(10, 280, 200, 20), "GasConstant: " + Constants.GasConstant);

        Constants.Friction =
            GUI.HorizontalSlider(new Rect(10, 340, 200, 20), Constants.Friction, 0, 0.5f);
        GUI.Label(new Rect(10, 320, 200, 20), "Dissipitation: " + Constants.Friction);

    }
		
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

	public void OnDrawGizmos(){
		if (!Application.isPlaying) return;
		generator.OnDrawGizmos ();
		/*
		for (int i = 0; i < fluidSim.m_grid.Width; i++) {
			for (int j = 0; j < fluidSim.m_grid.Height; j++) {
					if ((i + j) % 2 == 0) {
						Gizmos.color = new Color (0, 0, 0, 0.5F);
					} else
						Gizmos.color = new Color (1, 1, 1, 0.5F);
					Gizmos.DrawCube (new Vector3 (Constants.CellSpace * i + Constants.SimulationDomain.xMin, Constants.CellSpace * j + Constants.SimulationDomain.yMin, 0),
						new Vector3 (Constants.CellSpace, Constants.CellSpace, 0));
				
			}
		}
		*/
	}
}

