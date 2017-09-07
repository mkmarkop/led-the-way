using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public delegate void playerStateHandler(PlayerState newState);

	public static event playerStateHandler onStateChange;

	public static float[] stateDelayTimer = new
		float[(int)PlayerState._stateCount];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");
		if (onStateChange != null) {
			if (horizontal < 0.0f) {
				onStateChange (PlayerState.walkingLeft);
			} else if (horizontal > 0.0f) {
				onStateChange (PlayerState.walkingRight);
			} else {
				onStateChange (PlayerState.idle);
			}
		}

		float jump = Input.GetAxis ("Jump");
		if (jump > 0.0f && onStateChange != null)
			onStateChange (PlayerState.jumping);
	}
}
