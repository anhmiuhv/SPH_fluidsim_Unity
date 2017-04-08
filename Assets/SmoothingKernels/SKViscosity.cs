//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Implementation of the Viscosity Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. MŸller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.


using System;
using System.Collections;
using UnityEngine;

public class SKViscosity : SmoothingKernel {
	
	public SKViscosity() {
	}
	
	public SKViscosity(float kernelSize) {
		KernelSize   = kernelSize;
	}
	
	protected override void CalculateFactor()
	{
		m_factor = (float)(15.0 / (2.0f * Math.PI * m_kernelSize3));
	}
	
	public override float Calculate(ref Vector2 distance) {
		float lenSq = distance.sqrMagnitude;
		if (lenSq > m_kernelSizeSq)
		{
            return 0.0f;
		}
		if (lenSq < Constants.SingleEpsilon)
		{
            lenSq = Constants.SingleEpsilon;
		}
		float len = (float)Math.Sqrt((double)lenSq);
		float len3 = len * len * len;
		return m_factor * (((-len3 / (2.0f * m_kernelSize3)) + (lenSq / m_kernelSizeSq) + (m_kernelSize / (2.0f * len))) - 1.0f);
	}
	
	public override Vector2 CalculateGradient(ref Vector2 distance) {
		throw new NotImplementedException();
	}
	
	public override float CalculateLaplacian(ref Vector2 distance) {
		float lenSq = distance.sqrMagnitude;
		if (lenSq > m_kernelSizeSq)
		{
            return 0.0f;
		}
		if (lenSq < Constants.SingleEpsilon)
		{
            lenSq = Constants.SingleEpsilon;
		}
		float len = (float)Math.Sqrt((double)lenSq);
		return m_factor * (6.0f / m_kernelSize3) * (m_kernelSize - len);
	}
}

