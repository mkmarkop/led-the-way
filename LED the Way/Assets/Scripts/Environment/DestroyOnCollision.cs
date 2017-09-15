using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D colliderEntering) {
		Destroy (this.gameObject);
	}
}
