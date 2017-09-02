using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform playerTransform;

	public float trackingSpeed = 0.2f;
	// interpolation parameter, its value is between 0 and 1
	public float currLerpDistance = 1.0f;
	private Vector3 lastTargetPosition = Vector3.zero;
	private Vector3 currTargetPosition = Vector3.zero;
	private PlayerState currentPlayerState = PlayerState.idle;

	void OnEnable() {
		PlayerController.onStateChange += onStateChange;
	}

	void OnDisable() {
		PlayerController.onStateChange -= onStateChange;
	}

	// Use this for initialization
	void Start () {
		currLerpDistance = 1.0f; // position camera at directly at the target
		Vector3 cameraPos = transform.position;
		Vector3 startTargPos = playerTransform.position;

		// 2D game, so set Z the same
		startTargPos.z = cameraPos.z;
		lastTargetPosition = startTargPos;
		currTargetPosition = startTargPos;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		onStateCycle ();

		// Each frame, move the camera by trackingSpeed percent
		// distance from camera to player
		currLerpDistance = Mathf.Clamp (trackingSpeed + currLerpDistance,
			0.0f, 1.0f);
		transform.position = Vector3.Lerp (lastTargetPosition,
			currTargetPosition, currLerpDistance);
	}

	void onStateCycle() {
		switch (currentPlayerState) {
		case PlayerState.walkingLeft:
			trackPlayer ();
			break;

		case PlayerState.walkingRight:
			trackPlayer ();
			break;

		case PlayerState.jumping:
			trackPlayer ();
			break;
		}
	}

	void trackPlayer() {
		Vector3 cameraPos = transform.position;
		Vector3 playerPos = playerTransform.position;

		if (cameraPos.x == playerPos.x
		    && cameraPos.y == playerPos.y) {

			currLerpDistance = 1.0f; // stop tracking, we're directly
									// above the player
			currTargetPosition = cameraPos;
			lastTargetPosition = cameraPos;
		}

		currLerpDistance = 0.0f;

		lastTargetPosition = cameraPos;
		currTargetPosition = playerPos;
		currTargetPosition.z = cameraPos.z;
	}

	void onStateChange(PlayerState newState) {
		currentPlayerState = newState;
	}
}
