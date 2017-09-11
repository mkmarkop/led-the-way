using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class CollectableController : MonoBehaviour {

	public delegate void collectHandler ();
	public static event collectHandler onCollect;

	private bool collected = false;
	private Animator animator;

	void Awake() {
		animator = GetComponent<Animator> ();
	}
	// Use this for initialization
	void Start () {
		animator.Play (0, -1, Random.value);
	}

	void OnTriggerEnter2D(Collider2D collidedObject) {
		if (collected)
			return;

		if (collidedObject.tag == "Player") {
			if (onCollect != null)
				onCollect ();
			collected = true;
			Destroy (this.gameObject);
		}
	}
}
