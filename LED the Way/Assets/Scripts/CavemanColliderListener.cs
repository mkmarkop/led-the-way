using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavemanColliderListener : MonoBehaviour {

	public CavemanController targetObject = null;

	void OnTriggerEnter2D(Collider2D collidedObject)  {
		switch (collidedObject.tag) {
		case "Ground":
			targetObject.onStateChange (
				CavemanController.cavemanStates.landed);
			break;
		}
	}

	void OnTriggerExit2D(Collider2D collidedObject) {
		switch (collidedObject.tag) {
		case "Ground":
			targetObject.onStateChange (
				CavemanController.cavemanStates.falling);
			break;
		}
	}
}
