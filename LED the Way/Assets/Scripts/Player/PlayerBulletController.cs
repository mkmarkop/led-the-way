using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBulletController : MonoBehaviour {

	public Transform playerTransform = null;
	public float bulletImpulse = 100f;

	private Rigidbody2D rigidbody;

	void Awake() {
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	public void Launch(float xDirection, float angle) {
		Quaternion rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		Vector3 bulletForce = rotation * new Vector3 (bulletImpulse * xDirection, 0f, 0f);
		transform.rotation = rotation;
		rigidbody.AddForce (new Vector2 (bulletForce.x, bulletForce.y));

		Destroy (gameObject, 1f);
	}
}
