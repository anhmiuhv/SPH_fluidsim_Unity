//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Implementation of the Spiky Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. MŸller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.


using System;
using System.Collections;
using UnityEngine;

public class SKSpiky : SmoothingKernel {

	public SKSpiky() {
	}
	
	public SKSpiky(float kernelSize) {
		KernelSize   = kernelSize;
	}

	protected override void CalculateFactor() {
		double kernelRad6 = Math.Pow((double)m_kernelSize, 6.0);
		m_factor = (float)(15.0 / (Math.PI * kernelRad6));
	}
	
	public override float Calculate(ref Vector2 distance) {
		float lenSq = distance.sqrMagnitude;
		if (lenSq > m_kernelSizeSq)
		{
            return 0.0f;
		}
		if (lenSq < Constants.SmallEpsilon)
		{
            lenSq = Constants.SmallEpsilon;
		}
		float f = m_kernelSize - (float)Math.Sqrt((double)lenSq);
		return m_factor * f * f * f;
	}
	
	public override Vector2 CalculateGradient(ref Vector2 distance) {
		float lenSq = distance.sqrMagnitude;
		if (lenSq > m_kernelSizeSq)
		{
            return new Vector2(0.0f, 0.0f);
		}
		if (lenSq < Constants.SmallEpsilon)
		{
            lenSq = Constants.SmallEpsilon;
		}
		float len = (float)Math.Sqrt((double)lenSq);
		float f = -m_factor * 3.0f * (m_kernelSize - len) * (m_kernelSize - len) / len;
		return new Vector2(distance.x * f, distance.y * f);
	}
	
	public override float CalculateLaplacian(ref Vector2 distance) {
		throw new NotImplementedException();
	}
}

