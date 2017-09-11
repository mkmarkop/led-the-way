using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsExtensions {

	public static bool ContainsBounds(this Bounds bounds, Bounds target) {
		return bounds.Contains (target.min) && bounds.Contains (target.max);
	}
}
