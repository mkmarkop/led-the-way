using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FlameColliderListener : MonoBehaviour {

	public FlameController targetObject = null;

	void OnTriggerEnter2D(Collider2D collidedObject)  {
		switch (collidedObject.tag) {
		case "Ground":
			targetObject.onStateChange (
				FlameController.flameStates.landed);
			break;
		}
	}

	void OnTriggerExit2D(Collider2D collidedObject) {
		switch (collidedObject.tag) {
		case "Ground":
			targetObject.onStateChange (
				FlameController.flameStates.falling);
			break;
		}
	}
}
