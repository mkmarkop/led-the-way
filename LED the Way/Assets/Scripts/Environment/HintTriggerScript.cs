using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HintTriggerScript : MonoBehaviour {

	public Canvas hintCanvas;

	void Start() {
		hintCanvas.enabled = false;
	}

	void OnTriggerEnter2D(Collider2D enteringCollider) {
		if (enteringCollider.tag == "Player") {
			if (hintCanvas != null)
				hintCanvas.enabled = true;
		}
	}

	void OnTriggerExit2D(Collider2D exitingCollider) {
		if (exitingCollider.tag == "Player") {
			if (hintCanvas != null)
				hintCanvas.enabled = false;
		}
	}
}
