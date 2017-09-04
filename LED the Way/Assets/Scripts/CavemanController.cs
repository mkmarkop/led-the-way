using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CavemanController : MonoBehaviour {

	private Animator cavemanAnimator;

	private float walkingDirection = 0.0f;
	public float walkingSpeed = 0.45f;

	public enum cavemanStates {
		inactive = 0,
		landed,
		falling,
		walkingLeft,
		walkingRight,
		attacking,
		_stateCount
	}

	public cavemanStates currentState = cavemanStates.inactive;

	// Use this for initialization
	void Start () {
		cavemanAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case cavemanStates.inactive:
			break;

		case cavemanStates.landed:
			break;

		case cavemanStates.falling:
			break;

		case cavemanStates.walkingLeft:
			transform.Translate (new Vector3 (
				walkingSpeed * walkingDirection * Time.deltaTime, 0f, 0f));
			break;

		case cavemanStates.walkingRight:
			transform.Translate (new Vector3 (
				walkingSpeed * walkingDirection * Time.deltaTime, 0f, 0f));
			break;

		case cavemanStates.attacking:
			break;
		}
	}

	bool isValidTransition(cavemanStates newState) {
		bool isValid = false;

		switch (currentState) {
		case cavemanStates.inactive:
			isValid = true;
			break;

		case cavemanStates.landed:
			if (newState == cavemanStates.inactive
			    || newState == cavemanStates.walkingLeft
			    || newState == cavemanStates.walkingRight
			    || newState == cavemanStates.attacking)
				isValid = true;
			break;

		case cavemanStates.falling:
			break;

		case cavemanStates.walkingLeft:
			if (newState == cavemanStates.inactive
				|| newState == cavemanStates.landed
				|| newState == cavemanStates.walkingRight
				|| newState == cavemanStates.attacking)
				isValid = true;
			break;

		case cavemanStates.walkingRight:
			if (newState == cavemanStates.inactive
				|| newState == cavemanStates.landed
				|| newState == cavemanStates.walkingLeft
				|| newState == cavemanStates.attacking)
				isValid = true;
			break;

		case cavemanStates.attacking:
			if (newState == cavemanStates.inactive
				|| newState == cavemanStates.walkingLeft
				|| newState == cavemanStates.walkingRight
				|| newState == cavemanStates.landed)
				isValid = true;
			break;
		}

		return isValid;
	}

	public void onStateChange(cavemanStates newState) {
		if (newState == currentState)
			return;

		Vector3 localScale = transform.localScale;

		switch (currentState) {
		case cavemanStates.inactive:
			cavemanAnimator.SetBool ("isWalking", false);
			cavemanAnimator.SetBool ("isAttacking", false);
			break;

		case cavemanStates.landed:
			cavemanAnimator.SetBool ("isWalking", false);
			cavemanAnimator.SetBool ("isAttacking", false);
			break;

		case cavemanStates.falling:
			break;

		case cavemanStates.walkingLeft:
			localScale.x = -1.0f;
			transform.localScale = localScale;
			walkingDirection = -1.0f;

			cavemanAnimator.SetBool ("isWalking", true);
			cavemanAnimator.SetBool ("isAttacking", false);
			break;

		case cavemanStates.walkingRight:
			localScale.x = 1.0f;
			transform.localScale = localScale;
			walkingDirection = 1.0f;

			cavemanAnimator.SetBool ("isWalking", true);
			cavemanAnimator.SetBool ("isAttacking", false);
			break;

		case cavemanStates.attacking:
			cavemanAnimator.SetBool ("isWalking", false);
			cavemanAnimator.SetBool ("isAttacking", true);
			break;
		}

		currentState = newState;
	}
}
