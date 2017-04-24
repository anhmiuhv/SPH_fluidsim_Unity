using System;
using UnityEngine;
public class Helper {
	public static float ImplicitField(Vector2 position, Vector2 particle) {
		float radius2 = Constants.Radius * Constants.Radius;
		float dist = (position - particle).sqrMagnitude;
		return radius2 / dist;
 	}
}

