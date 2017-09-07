using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlameController : MonoBehaviour {

	public Transform targetToFollow;

	public float jumpForceHorizontal = 25f;
	public float jumpForceVertical = 125f;

	public bool onGround = false;
	private Rigidbody2D rigidBody;
	private float jumpDirection = -1.0f;

	public float jumpWaitDelay = 0.75f;
	public float timeForNextJump = 0.0f;

	public enum flameStates {
		inactive = 0,
		jumping,
		landed,
		falling,
		_stateCount
	}

	public flameStates currentState = flameStates.inactive;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case flameStates.inactive:
			break;

		case flameStates.jumping:
			break;

		case flameStates.landed:
			if (timeForNextJump == 0.0f) {
				timeForNextJump = Time.time +
					jumpWaitDelay + Random.Range(-0.15f, 0.15f);
			} else if (timeForNextJump < Time.time) {
				onStateChange (flameStates.jumping);
			}
			break;

		case flameStates.falling:
			break;
		}
	}

	bool isValidTransition(flameStates newState) {
		bool isValid = false;

		switch (currentState) {
		case flameStates.inactive:
			isValid = true;
			break;

		case flameStates.jumping:
			if (newState == flameStates.landed
			    || newState == flameStates.falling)
				isValid = true;
			break;

		case flameStates.landed:
			if (newState == flameStates.inactive
			    || newState == flameStates.jumping)
				isValid = true;
			break;

		case flameStates.falling:
			if (newState == flameStates.landed)
				isValid = true;
			break;
		}

		return isValid;
	}

	public void onStateChange(flameStates newState) {
		if (newState == currentState)
			return;
		
		switch (newState) {
		case flameStates.inactive:
			break;

		case flameStates.jumping:
			if (onGround) {
				rigidBody.AddForce (new Vector2 (jumpDirection *
				jumpForceHorizontal, jumpForceVertical));
				onGround = false;
				timeForNextJump = 0.0f;
			}
			break;

		case flameStates.landed:
			onGround = true;

			Vector3 localScale = transform.localScale;

			if (targetToFollow.position.x <
			    transform.position.x) {
				jumpDirection = -1.0f;
				localScale.x = 1.0f;
			} else if (targetToFollow.position.x >
			           transform.position.x) {
				jumpDirection = 1.0f;
				localScale.x = -1.0f;
			}

			transform.localScale = localScale;
			break;

		case flameStates.falling:
			break;
		}

		currentState = newState;
	}
}
