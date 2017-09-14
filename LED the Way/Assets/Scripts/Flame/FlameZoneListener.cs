using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FlameZoneListener : MonoBehaviour {

	public FlameController targetObject;

	void OnTriggerEnter2D(Collider2D collidedObject)  {
		switch (collidedObject.tag) {
		case "Player":
			targetObject.onStateChange (
				FlameController.flameStates.playerSighted);
			break;
		}
	}

	void OnTriggerExit2D(Collider2D collidedObject) {
		switch (collidedObject.tag) {
		case "Player":
			targetObject.onStateChange (
				FlameController.flameStates.playerOutOfSight);
			break;
		}
	}
}
