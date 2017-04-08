//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Base class of a bounding volume

using System;
using System.Collections;
using UnityEngine;

public abstract class BoundingVolume {
	
	public Vector2 Position;
	public Vector2[] Axis;
	public bool IsFixed;
	public float Margin;
	
	protected BoundingVolume() {
		IsFixed = false;
		Margin = Constants.SingleEpsilon;
	}
	
	public abstract void Project(Vector2 axis, out float min, out float max);
	
	public bool Intersects(BoundingVolume other) {
		Vector2 penetrationNormal;
		float penetrationLength;
		bool r = Intersects(other, out penetrationNormal, out penetrationLength);
		return r;
	}
	
	public bool Intersects(BoundingVolume other, out Vector2 penetrationNormal, out float penetrationLength) {
		penetrationNormal = Vector2.zero;
		penetrationLength = float.MaxValue;
		
		// Axis of this
		if (Axis != null) {
            foreach (Vector2 axis in Axis) {
				if (!FindLeastPenetrating(axis, other, ref penetrationNormal, ref penetrationLength)) {
					return false;
				}
            }
		}
		
		// Axis of other
		if (other.Axis != null) {
            foreach (Vector2 axis in other.Axis) {
				if (!FindLeastPenetrating(axis, other, ref penetrationNormal, ref penetrationLength)) {
					return false;
				}
            }
		}
		
		// Flip penetrationDirection to point away from this
		if (Vector2.Dot(other.Position - Position, penetrationNormal) > 0.0f) {
			penetrationNormal = penetrationNormal * -1.0f;
		}
		return true;
	}
	
	private bool FindLeastPenetrating(Vector2 axis, BoundingVolume other, ref Vector2 penetrationNormal, ref float penetrationLength) {
		float minThis, maxThis, minOther, maxOther;
		
		// Tests if separating axis exists
		if (TestSeparatingAxis(axis, other, out minThis, out maxThis, out minOther, out maxOther)) {
            return false;
		}
		
		// Find least penetrating axis
		float diff = Math.Min(maxOther, maxThis) - Math.Max(minOther, minThis);
		// Store penetration vector
		if (diff < penetrationLength) {
            penetrationLength    = diff;
            penetrationNormal    = axis;
		}
		return true;
	}
	
	private bool TestSeparatingAxis(Vector2 axis, BoundingVolume other, out float minThis, out float maxThis, out float minOther, out float maxOther) {
		Project(axis, out minThis, out maxThis);
		other.Project(axis, out minOther, out maxOther);
		
		// Add safety margin distance
		minThis  -= Margin;
		maxThis  += Margin;
		minOther -= other.Margin;
		maxOther += other.Margin;
		
		if (minThis >= maxOther || minOther >= maxThis) {
            return true;
		}
		return false;
	}
	
	
}


