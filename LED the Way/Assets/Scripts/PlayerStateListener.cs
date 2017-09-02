using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour {

	public float playerWalkSpeed = 1.0f;

	private Animator playerAnimator;
	private PlayerState currentState = PlayerState.idle;

	void OnEnable() {
		PlayerController.onStateChange += onStateChange;
	}

	void OnDisable() {
		PlayerController.onStateChange -= onStateChange;
	}

	void Start() {
		playerAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case PlayerState.idle:
			break;

		case PlayerState.walkingLeft:
			transform.Translate (new Vector3 (
				(playerWalkSpeed * -1.0f) * Time.deltaTime,
				0f, 0f));
			break;

		case PlayerState.walkingRight:
			transform.Translate (new Vector3 (
				(playerWalkSpeed * 1.0f) * Time.deltaTime,
				0f, 0f));
			
			break;

		case PlayerState.actionButton:
			break;

		case PlayerState.jumping:
			break;

		case PlayerState.landed:
			break;

		case PlayerState.falling:
			break;

		case PlayerState.killed:
			break;

		case PlayerState.resurrect:
			break;
		}
	}
	 
	bool isValidTransition(PlayerState newState) {
		bool isValid = false;

		switch (currentState) {
		case PlayerState.idle:
			isValid = true;
			break;

		case PlayerState.walkingLeft:
			isValid = true;
			break;

		case PlayerState.walkingRight:
			isValid = true;
			break;

		case PlayerState.actionButton:
			break;

		case PlayerState.jumping:
			break;

		case PlayerState.landed:
			break;

		case PlayerState.falling:
			break;

		case PlayerState.killed:
			break;

		case PlayerState.resurrect:
			break;
		}

		return isValid;
	}

	void onStateChange(PlayerState newState) {
		if (currentState == newState)
			return;

		if (!isValidTransition (newState))
			return;

		Vector3 localScale = transform.localScale;

		switch (newState) {
		case PlayerState.idle:
			playerAnimator.SetBool ("isWalking", false);
			break;

		case PlayerState.walkingLeft:
			if (localScale.x > 0)
				localScale.x = -1.0f;
			transform.localScale = localScale;
			playerAnimator.SetBool ("isWalking", true);
			break;

		case PlayerState.walkingRight:
			if (localScale.x < 0)
				localScale.x = 1.0f;
			transform.localScale = localScale;
			playerAnimator.SetBool ("isWalking", true);
			break;
		}
		
		currentState = newState;
	}
}
