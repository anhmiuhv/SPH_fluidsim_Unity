//   SPH Fluid Dynamics for Unity3d.
//	Constant
//

using System;
using System.Collections;
using UnityEngine;

public static class Constants {

    public static int MAX_PARTICLES = 2000;
    public static int resolution = 6;
    
	public const float DensityOffset                         = 100f;
	public static float GasConstant                			 = 0.7f;
    public static float Friction = 0.5f;
    public static float Radius = 0.5f;
	public static Vector2 Gravity                   		 = new Vector2(0.0f, -9.81f);
	
	public static readonly Rect SimulationDomain       		 = new Rect(-5f, -5f, 20f, 10f);
	public static readonly float CellSpace                   = (SimulationDomain.width + SimulationDomain.height) / 32;
	public static float ParticleMass                		 = CellSpace * 20f;//20f;
	public static float ParticleDamping                		 = .01f;
	public const float TimeStepSeconds                       = 0.01f;//0.01f;

    public const int prime1 = 223;
    public const int prime2 = 127;
    

    public static float mRadialViscosityGain 				= 2f;
	public const float SmallEpsilon                         = 1.192092896e-07f;

    //compute shader things
    public static int SIZE_PARTICLE = 8;
    public static int SIZE_GRID = 12;
	#if UNITY_STANDALONE_OSX
    public static int WARP_SIZE = 512;
	#endif
	#if UNITY_STANDALONE_WIN
	public static int WARP_SIZE = 1024;
	#endif

}

