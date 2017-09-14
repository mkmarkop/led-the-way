using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class FlameController : MonoBehaviour {

	public Slider healthBar;
	public Transform targetToFollow;

	public float jumpForceHorizontal = 25f;
	public float jumpForceVertical = 125f;

	public bool onGround = false;
	private Rigidbody2D rigidBody;
	private float jumpDirection = -1.0f;

	public float jumpWaitDelay = 0.75f;
	public float timeForNextJump = 0.0f;

	private bool canJump = false;

	private int maxHealth = 2;
	private int health = 2;

	public enum flameStates {
		inactive = 0,
		jumping,
		landed,
		falling,
		hurt,
		killed,
		playerSighted,
		playerOutOfSight,
		_stateCount
	}

	public flameStates currentState = flameStates.inactive;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		healthBar.minValue = 0;
		healthBar.maxValue = maxHealth;
		healthBar.value = health;
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case flameStates.inactive:
			break;

		case flameStates.jumping:
			break;

		case flameStates.landed:
			if (canJump) {
				if (timeForNextJump == 0.0f) {
					timeForNextJump = Time.time +
					jumpWaitDelay + Random.Range (-0.15f, 0.15f);
				} else if (timeForNextJump < Time.time) {
					onStateChange (flameStates.jumping);
				}
			}
			break;

		case flameStates.falling:
			break;

		case flameStates.playerSighted:
			onStateChange (flameStates.landed);
			break;

		case flameStates.playerOutOfSight:
			onStateChange (flameStates.inactive);
			break;
		}
	}

	bool isValidTransition(flameStates newState) {
		bool isValid = false;

		switch (currentState) {
		case flameStates.playerOutOfSight:
			if (currentState != flameStates.jumping)
				isValid = true;
			else
				isValid = false;
			break;

		case flameStates.playerSighted:
			isValid = true;
			break;
			
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
				
		case flameStates.hurt:
			isValid = true;
			break;

		case flameStates.killed:
			isValid = false;
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

		case flameStates.hurt:
			health--;
			healthBar.value = health;
			if (health <= 0)
				onStateChange (flameStates.killed);
			else
				onStateChange(currentState);
			return;
			break;

		case flameStates.killed:
			Destroy (gameObject, 0.1f);
			break;

		case flameStates.playerSighted:
			canJump = true;
			break;

		case flameStates.playerOutOfSight:
			canJump = false;
			break;
		}

		currentState = newState;
	}

	public void hitByBullet() {
		onStateChange (flameStates.hurt);
	}
}
