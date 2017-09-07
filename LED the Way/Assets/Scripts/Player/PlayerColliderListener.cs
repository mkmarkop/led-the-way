using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerColliderListener : MonoBehaviour {

	public PlayerStateListener targetListener = null;

	void OnTriggerEnter2D(Collider2D collidedObject) {
		switch(collidedObject.tag) {
		case "Ground":
			targetListener.onStateChange (PlayerState.landed);
			break;
		}
	}

	void OnTriggerExit2D(Collider2D collidedObject) {
		switch (collidedObject.tag) {
		case "Ground":
			targetListener.onStateChange (PlayerState.falling);
			break;
		}
	}
}