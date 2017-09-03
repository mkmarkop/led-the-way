using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateListener : MonoBehaviour {

	public float playerWalkSpeed = 1.0f;
	public float jumpForceHorizontal = 250f;
	public float jumpForceVertical = 500f;

	private bool onGround = true;
	private Animator playerAnimator;
	private Rigidbody2D rigidBody;
	public PlayerState currentState = PlayerState.idle;

	void OnEnable() {
		PlayerController.onStateChange += onStateChange;
	}

	void OnDisable() {
		PlayerController.onStateChange -= onStateChange;
	}

	void Start() {
		rigidBody = GetComponent<Rigidbody2D> ();
		playerAnimator = GetComponent<Animator> ();
		PlayerController.stateDelayTimer [(int)PlayerState.jumping] = 1.0f;
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
			if (newState == PlayerState.landed
				|| newState == PlayerState.falling
			    || newState == PlayerState.killed
			    || newState == PlayerState.actionButton)
				isValid = true;
			break;

		case PlayerState.landed:
			if (newState == PlayerState.idle
			    || newState == PlayerState.walkingLeft
			    || newState == PlayerState.walkingRight
			    || newState == PlayerState.actionButton) {
				isValid = true;
			}
			break;

		case PlayerState.falling:
			if (newState == PlayerState.landed
			    || newState == PlayerState.killed
			    || newState == PlayerState.actionButton)
				isValid = true;
			break;

		case PlayerState.killed:
			break;

		case PlayerState.resurrect:
			break;
		}

		return isValid;
	}

	bool mustAbortTransition(PlayerState newState) {
		bool abort = false;

		switch (newState) {
		case PlayerState.idle:
			break;

		case PlayerState.walkingLeft:
			break;

		case PlayerState.walkingRight:
			break;

		case PlayerState.jumping:
			float nextAllowedJumpedTime =
				PlayerController.stateDelayTimer [(int)PlayerState.jumping];

			if (nextAllowedJumpedTime == 0.0f
			    || nextAllowedJumpedTime > Time.time)
				abort = true;
			break;

		case PlayerState.landed:
			break;

		case PlayerState.falling:
			break;

		case PlayerState.killed:
			break;


		case PlayerState.resurrect:
			break;

		case PlayerState.actionButton:
			break;
		}

		return abort;
	}

	public void onStateChange(PlayerState newState) {
		if (currentState == newState)
			return;

		if (!isValidTransition (newState))
			return;

		if (mustAbortTransition (newState))
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

		case PlayerState.jumping:
			if (onGround) {
				playerAnimator.SetBool ("isJumping", true);
				float jumpDirection = 0.0f;
				if (currentState == PlayerState.walkingLeft)
					jumpDirection = -1.0f;
				else if (currentState == PlayerState.walkingRight)
					jumpDirection = 1.0f;
				else
					jumpDirection = 0.0f;

				// apply the force
				//rigidBody.AddForce (new Vector2 (jumpDirection *
				//jumpForceHorizontal, jumpForceVertical));
				StartCoroutine(DelayedJump(0.2f, jumpDirection));
				onGround = false;
				// disable jumping
				PlayerController.stateDelayTimer [
					(int)PlayerState.jumping] = 0f;
			}
			break;

		case PlayerState.landed:
			onGround = true;
			PlayerController.stateDelayTimer [
				(int)PlayerState.jumping] = Time.time + 0.1f;
			break;

		case PlayerState.falling:
			playerAnimator.SetBool ("isJumping", false);
			PlayerController.stateDelayTimer [
				(int)PlayerState.jumping] = 0f;
			break;
		}
		
		currentState = newState;
	}

	IEnumerator DelayedJump(float time, float jumpDirection) {
		yield return new WaitForSeconds (time);

		rigidBody.AddForce (new Vector2 (jumpDirection *
		jumpForceHorizontal, jumpForceVertical));
	}
}
