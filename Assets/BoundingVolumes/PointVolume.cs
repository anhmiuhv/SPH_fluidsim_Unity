//   SPH Fluid Dynamics for Unity3d.
// Point Volume
//

using System;
using System.Collections;
using UnityEngine;

public class PointVolume : BoundingVolume {
	
	public Vector2[] Axis;
	
	public PointVolume() {
		this.Position = Vector2.zero;
	}
	
	public override void Project(Vector2 axis, out float min, out float max) {
		min = max = Vector2.Dot(Position, axis);
	}
	
	
}

