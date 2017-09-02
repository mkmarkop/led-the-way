using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public delegate void playerStateHandler(PlayerState newState);

	public static event playerStateHandler onStateChange;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxis ("Horizontal");

		PlayerState newState = PlayerState.idle;

		if (horizontal < 0.0f) {
			newState = PlayerState.walkingLeft;
		} else if (horizontal > 0.0f) {
			newState = PlayerState.walkingRight;
		}

		if (onStateChange != null)
			onStateChange (newState);
	}
}
