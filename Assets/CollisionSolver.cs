//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Solves collisions
//
//

using System;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSolver {

	public List<BoundingVolume> BoundingVolumes;
	public float Bounciness;
	public float Friction;
	

	public CollisionSolver() {
		BoundingVolumes = new List<BoundingVolume>();
		Bounciness      = 1.0f;
		Friction        = 0.0f;
	}
	
	public bool Solve() {
		bool hasCollided = false;
		Vector2 penetration;
		float penLen;
		foreach (BoundingVolume bv1 in BoundingVolumes) {
            foreach (BoundingVolume bv2 in BoundingVolumes) {
				if (bv1 != bv2) {
					if (bv1.Intersects(bv2, out penetration, out penLen)) {
						hasCollided = true;
						penetration *= penLen;
						if (bv2.IsFixed) {
							bv1.Position += penetration;
						}
						else {
							bv2.Position -= penetration;
						}
					}
				}
            }
		}
		return hasCollided;
	}
	
	public bool Solve(ref List<mParticle> particles) {
		bool hasCollided = false;
		Vector2 penetration, penNormal, v, vn, vt;
		float penLen, dp;
		foreach (BoundingVolume bv in BoundingVolumes) {
            foreach (mParticle particle in particles) {
				if (bv.Intersects(particle.BoundingVolume, out penNormal, out penLen)) {
					hasCollided = true;
					penetration = penNormal * penLen;
					if (particle.BoundingVolume.IsFixed) {
						bv.Position += penetration;
					}
					else {
						particle.BoundingVolume.Position -= penetration;

						v = particle.Position - particle.PositionOld;
						// eq penNormal.PerpendicularRight.
						// see: http://opentk.svn.sourceforge.net/viewvc/opentk/trunk/Source/OpenTK/Math/Vector2.cs?revision=2530&view=markup
						Vector2 tangent = new Vector2(penNormal.y, -penNormal.x);
						dp = Vector2.Dot(v, penNormal);
						vn = penNormal * (dp * -Bounciness);
						dp = Vector2.Dot(v, tangent);
						vt = tangent * (dp * (1.0f -Friction));
						v = vn + vt;
						particle.Position -= penetration;
						particle.PositionOld = particle.Position - v;
					}
				}
            }
		}
		return hasCollided;
	}
	
	public BoundingVolume FindIntersect(BoundingVolume boundingVolume) {
		for (int i = 0; i < BoundingVolumes.Count; i++) {
			BoundingVolume bv = (BoundingVolume) BoundingVolumes[i];
			if (bv.Intersects(boundingVolume)) {
				return bv;
			}
		}
		return null;
	}
	
}
