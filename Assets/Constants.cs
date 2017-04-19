//   SPH Fluid Dynamics for Unity3d.
//	Constant
//

using System;
using System.Collections;
using UnityEngine;

public static class Constants {

	public const float DensityOffset                         = 100f;
	public static float GasConstant                			 = 1.205f;//.1f;
	public const float Viscosity                             = 0.2f;
	public static Vector2 Gravity                   		 = new Vector2(0.0f, -9.81f);
	
	public static readonly Rect SimulationDomain       		 = new Rect(-5f, -5f, 20f, 10f);
	public static readonly float CellSpace                   = (SimulationDomain.width + SimulationDomain.height) / 32;
	public static float ParticleMass                		 = CellSpace * 20f;//20f;
	public static float ParticleDamping                		 = .01f;
	public const float TimeStepSeconds                       = 0.01f;//0.01f;
		
	public const int PrimeNumber01                           = 73856093;
	public const int PrimeNumber02                           = 19349663;
	public const int PrimeNumber03                           = 83492791;

	public static float mRadialViscosityGain 				= 0.2f;
	public const float SmallEpsilon                         = 1.192092896e-07f;
}

