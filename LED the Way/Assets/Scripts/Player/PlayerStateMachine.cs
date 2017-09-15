using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerStateMachine : MonoBehaviour {

	public PlayerController playerController;
	public GameObject bulletPrefab;
	public Transform shootingPoint;

	private int damageTaken = 0;
	private int currentHealth = 6;
	public const int MaxHealth = 6;
	public static event playerHealthHandler onReceiveDamage;
	public delegate void playerHealthHandler (int newHealth);

	public delegate void playerStateHandler(PlayerState newState);
	public static event playerStateHandler onStateChange;

	private const float INVULNERABLE_TIME = 2.5f;

	private bool onGround = true;
	private Animator playerAnimator;
	public PlayerState currentState = PlayerState.idle;

	private Vector3 respawnPoint;

	void OnEnable() {
		CheckpointTriggerScript.onCheckpointPassed += onCheckpointPassed;
	}

	void OnDisable() {
		CheckpointTriggerScript.onCheckpointPassed -= onCheckpointPassed;
	}

	void Start() {
		playerAnimator = GetComponent<Animator> ();
		PlayerController.stateDelayTimer [(int)PlayerState.actionButton] = 1.0f;
		PlayerController.stateDelayTimer [(int)PlayerState.jumping] = 1.0f;
		PlayerController.stateDelayTimer [(int)PlayerState.hurt] = 1.0f;

		respawnPoint = transform.position;
		onStateChangeTry (PlayerState.falling);
	}
	
	// Update is called once per frame
	public void onStateCycle () {
		
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

		case PlayerState.glidingLeft:
			if (newState == PlayerState.falling
			    || newState == PlayerState.killed
			    || newState == PlayerState.actionButton
			    || newState == PlayerState.recover
			    || newState == PlayerState.glidingRight
			    || newState == PlayerState.hurt
			    || newState == PlayerState.landed)
				isValid = true;
			break;

		case PlayerState.glidingRight:
			if (newState == PlayerState.falling
				|| newState == PlayerState.killed
				|| newState == PlayerState.actionButton
				|| newState == PlayerState.recover
				|| newState == PlayerState.glidingLeft
				|| newState == PlayerState.hurt
				|| newState == PlayerState.landed)
				isValid = true;
			break;

		case PlayerState.actionButton:
			isValid = true;
			break;

		case PlayerState.jumping:
			if (newState == PlayerState.landed
				|| newState == PlayerState.falling
			    || newState == PlayerState.killed
			    || newState == PlayerState.actionButton
				|| newState == PlayerState.recover
				|| newState == PlayerState.glidingLeft
				|| newState == PlayerState.glidingRight
				|| newState == PlayerState.hurt)
				isValid = true;
			break;

		case PlayerState.landed:
			if (newState == PlayerState.idle
			    || newState == PlayerState.walkingLeft
			    || newState == PlayerState.walkingRight
			    || newState == PlayerState.actionButton
				|| newState == PlayerState.hurt
				|| newState == PlayerState.killed
				|| newState == PlayerState.recover) {
				isValid = true;
			}
			break;

		// From falling go to landed
		case PlayerState.falling:
			if (newState == PlayerState.landed
				|| newState == PlayerState.glidingLeft
				|| newState == PlayerState.glidingRight
			    || newState == PlayerState.killed
				|| newState == PlayerState.hurt
				|| newState == PlayerState.recover
			    || newState == PlayerState.actionButton)
				isValid = true;
			break;

		case PlayerState.hurt:
			isValid = true;
			break;

		case PlayerState.recover:
			isValid = true;
			break;

		case PlayerState.killed:
			if (newState == PlayerState.resurrect)
				isValid = true;
			else
				isValid = false;
			break;

		case PlayerState.resurrect:
			if (newState == PlayerState.idle)
				isValid = true;
			else
				isValid = false;
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

		case PlayerState.glidingLeft:
			break;

		case PlayerState.glidingRight:
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

		case PlayerState.hurt:
			float nextAllowedHurtTime =
				PlayerController.stateDelayTimer [(int)PlayerState.hurt];

			if (nextAllowedHurtTime == 0.0f
			    || nextAllowedHurtTime > Time.time)
				abort = true;
			break;

		case PlayerState.recover:
			break;

		case PlayerState.killed:
			break;

		case PlayerState.resurrect:
			break;

		case PlayerState.actionButton:
			float nextAllowedFireTime =
				PlayerController.stateDelayTimer [(int)PlayerState.actionButton];

			if (nextAllowedFireTime == 0.0f
			    || nextAllowedFireTime > Time.time)
				abort = true;
			break;
		}

		return abort;
	}

	PlayerState conditionalTransition(PlayerState newState) {
		PlayerState transit = newState;

		switch (newState) {
		case PlayerState.idle:
			break;

		case PlayerState.walkingLeft:
			if (!onGround)
				transit = PlayerState.glidingLeft;
			break;

		case PlayerState.walkingRight:
			if (!onGround)
				transit = PlayerState.glidingRight;
			break;

		case PlayerState.glidingLeft:
			break;

		case PlayerState.glidingRight:
			break;

		case PlayerState.jumping:
			break;

		case PlayerState.landed:
			break;

		case PlayerState.falling:
			break;

		case PlayerState.hurt:
			break;

		case PlayerState.recover:
			break;

		case PlayerState.killed:
			break;

		case PlayerState.resurrect:
			break;

		case PlayerState.actionButton:
			break;
		}

		return transit;
	}

	public void onStateChangeTry(PlayerState newState) {

		if (currentState == newState)
			return;

		newState = conditionalTransition (newState);

		if (!isValidTransition (newState)) {
			return;
		}

		if (mustAbortTransition (newState)) {
			return;
		}

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

		case PlayerState.glidingLeft:
			if (localScale.x > 0)
				localScale.x = -1.0f;
			transform.localScale = localScale;
			break;

		case PlayerState.glidingRight:
			if (localScale.x < 0)
				localScale.x = 1.0f;
			transform.localScale = localScale;
			break;

		case PlayerState.jumping:
			if (onGround) {
				playerAnimator.SetBool ("444", true);
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

		case PlayerState.hurt:
			currentHealth -= damageTaken;
			damageTaken = 0;

			if (onReceiveDamage != null)
				onReceiveDamage (currentHealth);

			PlayerController.stateDelayTimer [(int)PlayerState.hurt] =
				Time.time + INVULNERABLE_TIME;

			if (currentHealth > 0) {
				playerAnimator.SetLayerWeight (1, 1f);
				StartCoroutine (FinishRecovery ());
			} else {
				onStateChangeTry (PlayerState.killed);
			}
			break;

		case PlayerState.recover:
			playerAnimator.SetLayerWeight (1, 0f);
			break;

		case PlayerState.killed:
			if (onReceiveDamage != null)
				onReceiveDamage (currentHealth);
			
			playerAnimator.SetLayerWeight (1, 0f);

			PlayerController.stateDelayTimer [(int)PlayerState.hurt] =
				Time.time + INVULNERABLE_TIME;

			playerAnimator.SetLayerWeight (1, 1f);
			StartCoroutine (FinishRecovery ());
			break;

		case PlayerState.resurrect:
			currentHealth = MaxHealth;
			transform.position = respawnPoint;
			transform.rotation = Quaternion.identity;
			playerController.ResetVelocity ();
			onReceiveDamage (currentHealth);
			break;

		case PlayerState.actionButton:
			PlayerController.stateDelayTimer [
				(int)PlayerState.actionButton] = Time.time + .5f;

			GameObject newBullet = Instantiate (bulletPrefab);
			PlayerBulletController pbc = newBullet.GetComponent<PlayerBulletController> ();
			newBullet.transform.position = shootingPoint.position;
			pbc.Launch (transform.localScale.x, shootingPoint.rotation.eulerAngles.z);
			break;
		}

		currentState = newState;
		if (PlayerStateMachine.onStateChange != null)
			PlayerStateMachine.onStateChange (currentState);

		if (currentState == PlayerState.killed)
			onStateChangeTry (PlayerState.resurrect);
	}

	IEnumerator DelayedJump(float time, float jumpDirection) {
		yield return new WaitForSeconds (time);

		//rigidBody.AddForce (new Vector2 (jumpDirection *
		//jumpForceHorizontal, jumpForceVertical));

		yield return null;
	}

	IEnumerator	FinishRecovery() {
		yield return new WaitForSeconds (INVULNERABLE_TIME);
		onStateChangeTry (PlayerState.recover);
		yield return null;
	}

	public void hitDamageTrigger(int amount) {
		if (currentHealth <= 0) {
			onStateChangeTry (PlayerState.killed);
		} else {
			damageTaken = amount;
			onStateChangeTry (PlayerState.hurt);
		}
	}

	void onCheckpointPassed(Vector3 position) {
		respawnPoint = position;
	}
}
