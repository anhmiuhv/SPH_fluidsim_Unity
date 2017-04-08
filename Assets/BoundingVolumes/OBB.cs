//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Oriented bounded box

using System;
using System.Collections;
using UnityEngine;

public class OBB : BoundingVolume {
	
	public Vector2 Extents;
	
	public OBB() {
		Position     = Vector2.zero;
		Extents      = new Vector2(1.0f, 1.0f);
		Axis         = new Vector2[] {
            new Vector2(1,0),
            new Vector2(0,1)
		};
	}
	
	public override void Project(Vector2 axis, out float min, out float max) {
		float pos = Vector2.Dot(this.Position, axis);
		float radius = Math.Abs(Vector2.Dot(axis, this.Axis[0])) * this.Extents.x
			+ Math.Abs(Vector2.Dot(axis, this.Axis[1])) * this.Extents.y;
		min = pos - radius;
		max = pos + radius;
	}
	
	public void Rotate(double angle) {
		Axis[0] = RotateAxis(angle, Axis[0]);
		Axis[1] = RotateAxis(angle, Axis[1]);
	}

	private static Vector2 RotateAxis(double angle, Vector2 axis) {
		return new Vector2(axis.x * (float)Math.Cos(angle) + axis.y * (float)Math.Sin(angle),
						   axis.y * (float)Math.Cos(angle) - axis.x * (float)Math.Sin(angle));
	}
}

