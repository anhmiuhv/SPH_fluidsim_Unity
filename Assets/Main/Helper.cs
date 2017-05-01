using System;
using UnityEngine;
public class Helper {
	public static float ImplicitField(Vector2 position, Vector2 particle) {
		float radius2 = Constants.Radius * Constants.Radius;
		float dist = (position - particle).sqrMagnitude;
		if (dist > radius2)
			return 0;
		return (radius2 - dist) * (radius2 - dist);
 	}

    public static T[,] Make2DArray<T>(T[] input, int height, int width)
    {
        T[,] output = new T[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output[i, j] = input[i + j * height];
            }
        }
        return output;
    }
}

