using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTriggerScript : MonoBehaviour {

	public Transform spawnTransform;
	public Animator checkpointAnimator;

	public delegate void playerCheckpointHandler(Vector3 position);
	public static event playerCheckpointHandler onCheckpointPassed;

	bool playerHasPassed;

	void Start() {
		playerHasPassed = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag != "Player" || playerHasPassed)
			return;

		playerHasPassed = true;

		if (checkpointAnimator)
			checkpointAnimator.SetBool ("playerHasPassed", true);

		if (onCheckpointPassed != null)
			onCheckpointPassed (spawnTransform.position);
	}
}
