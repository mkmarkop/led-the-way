using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour {

	float velocityXSmoothing;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	float jumpHeight = 1.25f;
	float timeToJumpApex = .4f;
	float jumpVelocity = 8f;

	public LayerMask collisionMask;

	public delegate void playerStateHandler(PlayerState newState);

	public static event playerStateHandler onStateChangeTry;

	public static float[] stateDelayTimer = new
		float[(int)PlayerState._stateCount];

	const float skinWidth = .02f;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
	BoxCollider2D collider;

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	Vector3 velocity;
	float gravity = -20f;

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	// Use this for initialization
	void Start () {
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();

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
			velocity.y = 0;
		if (collisions.left || collisions.right)
			velocity.x = 0;

		velocity.y += gravity * Time.deltaTime;

		float horizontal = Input.GetAxis ("Horizontal");
		if (onStateChangeTry != null) {
			if (horizontal < 0.0f) {
				onStateChangeTry (PlayerState.walkingLeft);
			} else if (horizontal > 0.0f) {
				onStateChangeTry (PlayerState.walkingRight);
			} else {
				onStateChangeTry (PlayerState.idle);
				velocity.x = 0f;
			}
		}

		float jump = Input.GetAxis ("Jump");

		if (jump > 0.0f && collisions.below) {
			velocity.y = jumpVelocity;
		}

		if (jump > 0.0f && onStateChangeTry != null)
			onStateChangeTry (PlayerState.jumping);

		velocity.x = Mathf.SmoothDamp (velocity.x, horizontal * 1f, ref velocityXSmoothing,
			(collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		Move (velocity * Time.deltaTime);
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

	public struct CollisionInfo {
		public bool above, below, left, right;
		public void Reset() {
			above = below = left = right = false;
		}
	}
}
