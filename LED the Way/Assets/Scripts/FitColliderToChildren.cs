using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FitColliderToChildren : MonoBehaviour {

	private BoxCollider2D boxCollider2D;

	void Awake() {
		boxCollider2D = GetComponent<BoxCollider2D> ();
	}

	// Use this for initialization
	void Start () {
		Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
		bool initialized = false;

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer renderer in renderers) {
			if (initialized) {
				bounds.Encapsulate (renderer.bounds);
			} else {
				bounds = renderer.bounds;
				initialized = true;
			}
		}

		if (initialized) {
			boxCollider2D.offset = bounds.center - transform.position;
			boxCollider2D.size = bounds.size;
		} else {
			boxCollider2D.offset = Vector3.zero;
			boxCollider2D.size = Vector3.zero;
		}
	}
}
