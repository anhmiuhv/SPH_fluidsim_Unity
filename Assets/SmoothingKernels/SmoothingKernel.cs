//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       Abstract base class of a Smoothing-Kernel for SPH-based fluid simulation
//                      based on the paper:
//                      M. MŸller et al., "Particle-Based Fluid Simulation for Interactive Applications",
//                      SCA 03, pages 154-159.
//

using System;
using System.Collections;
using UnityEngine;

public abstract class SmoothingKernel {

	protected float m_factor;
	protected float m_kernelSize;
	protected float m_kernelSizeSq;
	protected float m_kernelSize3;
	
	public float KernelSize {
		get { return m_kernelSize; }
		set {
            m_kernelSize      = value;
            m_kernelSizeSq    = m_kernelSize * m_kernelSize;
            m_kernelSize3     = m_kernelSize * m_kernelSize * m_kernelSize;
            CalculateFactor();
		}
	}
	
	public SmoothingKernel() {
		m_factor     = 1.0f;
		KernelSize   = 1.0f;
	}
	
	public SmoothingKernel(float kernelSize) {
		m_factor     = 1.0f;
		KernelSize   = kernelSize;
	}
	
	protected abstract void CalculateFactor();
	
	public abstract float Calculate(ref Vector2 distance);
	
	public abstract Vector2 CalculateGradient(ref Vector2 distance);
	
	public abstract float CalculateLaplacian(ref Vector2 distance);
	
}
