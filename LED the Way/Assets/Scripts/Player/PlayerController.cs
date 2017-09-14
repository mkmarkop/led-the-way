using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour {

	public Transform shootingPoint;
	public BoxCollider2D gameBoundaries;
	public PlayerStateMachine playerStateMachine;

	public static float[] stateDelayTimer = new
		float[(int)PlayerState._stateCount];

	public Vector3 Velocity { get; private set; }

	float velocityXSmoothing;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	float jumpHeight = 1.25f;
	float timeToJumpApex = .4f;
	float jumpVelocity = 8f;

	public LayerMask collisionMask;
	const float skinWidth = .02f;
	RaycastOrigins raycastOrigins;
	CollisionInfo collisions;
	BoxCollider2D collider;

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	float gravity = -20f;
	Vector3 _velocity;

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public void Reset() {
			above = below = false;
			left = right = false;
		}
	}

	void Start() {
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();

		shootingPoint.transform.position = new Vector3 (
			shootingPoint.transform.position.x,
			collider.transform.position.y + collider.offset.y,
			shootingPoint.transform.position.y);

		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2f);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
	}

	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? 
				raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin,
				Vector2.up * directionY, rayLength, collisionMask);
			Debug.DrawRay (
				rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) {
				velocity.y = (hit.distance  - skinWidth) * directionY;
				rayLength = hit.distance; // Don't hit anything further away

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? 
				raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin,
				Vector2.right * directionX, rayLength, collisionMask);
			Debug.DrawRay (
				rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit) {
				velocity.x = (hit.distance  - skinWidth) * directionX;
				rayLength = hit.distance; // Don't hit anything further away

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	void Move(Vector3 movement) {
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (movement.x != 0)
			HorizontalCollisions (ref movement);
		
		if (movement.y != 0)
			VerticalCollisions (ref movement);

		transform.Translate (movement);	
	}

	void Update() {
		if (collisions.above || collisions.below)
			_velocity.y = 0;
		if (collisions.left || collisions.right)
			_velocity.x = 0;

		if (collisions.below)
			playerStateMachine.onStateChangeTry (PlayerState.landed);
		else
			playerStateMachine.onStateChangeTry (PlayerState.falling);

		_velocity.y += gravity * Time.deltaTime;

		float horizontal = Input.GetAxis ("Horizontal");
		if (horizontal == 0) {
			_velocity.x = 0f;
			playerStateMachine.onStateChangeTry (PlayerState.idle);
		} else if (horizontal < 0f) {
			playerStateMachine.onStateChangeTry (PlayerState.walkingLeft);
		} else if (horizontal > 0f) {
			playerStateMachine.onStateChangeTry (PlayerState.walkingRight);
		}

		float jump = Input.GetAxis ("Jump");
		if (jump > 0.0f && collisions.below) {
			_velocity.y = jumpVelocity;
			playerStateMachine.onStateChangeTry (PlayerState.jumping);
		}

		_velocity.x = Mathf.SmoothDamp (
			_velocity.x, horizontal * 1f, ref velocityXSmoothing,
			(collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		
		Move (_velocity * Time.deltaTime);

		if (!gameBoundaries.bounds.ContainsBounds (collider.bounds)) {
			playerStateMachine.onStateChangeTry (PlayerState.killed);
		}

		Aim ();

		float fire = Input.GetAxis ("Fire1");
		if (fire != 0.0f) {
			playerStateMachine.onStateChangeTry (PlayerState.actionButton);
		}
	}

	void Aim() {
		Vector3 rotationPivot = collider.transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);

		Vector3 rotationAxis = new Vector3 (0, 0, 1);
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector3 fromVector = (shootingPoint.position - rotationPivot);
		fromVector.Scale (new Vector3 (1f, 1f, 0f));
		Vector3 toVector = (mousePosition - rotationPivot);
		toVector.Scale (new Vector3 (1f, 1f, 0f));

		float mouseAngle = Mathf.Atan2 (toVector.y, toVector.x) * Mathf.Rad2Deg;

		if (transform.localScale.x > 0) {
			if (mouseAngle > 90 || mouseAngle < -25)
				return;
		} else {
			if (mouseAngle < 0)
				mouseAngle += 360f;
			if (mouseAngle < 90 || mouseAngle > 205)
				return;
		}

		float rotationAngle = Quaternion.FromToRotation (fromVector, toVector).eulerAngles.z;

		shootingPoint.RotateAround (rotationPivot, rotationAxis, rotationAngle);
	}

	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2f);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2f);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public void ResetVelocity() {
		Velocity = Vector3.zero;
	}
}
