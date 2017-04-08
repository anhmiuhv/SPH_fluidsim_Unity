//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       Implementation of the Poly6 Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. MŸller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.
//
//
//

using System;
using System.Collections;
using UnityEngine;

public class SKPoly6 : SmoothingKernel {
	
	public SKPoly6() {
	}
	
	public SKPoly6(float kernelSize) {
		KernelSize = kernelSize;
	}
	
    protected override void CalculateFactor() {
		double kernelRad9 = Math.Pow((double)m_kernelSize, 9.0);
		m_factor = (float)(315.0 / (64.0 * Math.PI * kernelRad9));
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
		float diffSq = m_kernelSizeSq - lenSq;
		return m_factor * diffSq * diffSq * diffSq;
	}
	
	public override Vector2 CalculateGradient(ref Vector2 distance) {
		float lenSq = distance.sqrMagnitude;
		if (lenSq > m_kernelSizeSq)
		{
            return new Vector2(0.0f, 0.0f);
		}
		if (lenSq < Constants.SingleEpsilon)
		{
            lenSq = Constants.SingleEpsilon;
		}
		float diffSq = m_kernelSizeSq - lenSq;
		float f = -m_factor * 6.0f * diffSq * diffSq;
		return new Vector2(distance.x * f, distance.y * f);
	}
	
	public override float CalculateLaplacian(ref Vector2 distance) {
		throw new NotImplementedException();
	}
}

