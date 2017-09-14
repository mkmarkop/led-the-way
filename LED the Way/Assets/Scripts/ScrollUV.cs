using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ScrollUV : MonoBehaviour {

	public float horizontalScrollingSpeed = 0.25f;
	public float verticalScrollingSpeed = 0.05f;

	private Material meshMaterial;
	private MeshRenderer meshRenderer;

	private PlayerState currentState;

	void OnEnable() {
		PlayerStateMachine.onStateChange += onStateChange;
	}

	void OnDisable() {
		PlayerStateMachine.onStateChange -= onStateChange;
	}

	void Start() {
		meshRenderer = GetComponent<MeshRenderer> ();
		meshMaterial = meshRenderer.sharedMaterial;
	}

	void Update() {
		Vector2 offset = meshMaterial.GetTextureOffset ("_MainTex");
		Vector2 movement = Vector2.zero;

		switch (currentState) {
		case PlayerState.walkingLeft:
			movement = new Vector2 (-Time.deltaTime * horizontalScrollingSpeed * 0.5f, 0);
			meshMaterial.mainTextureOffset = offset + movement;
			break;

		case PlayerState.walkingRight:
			movement = new Vector2 (+Time.deltaTime * horizontalScrollingSpeed * 0.5f, 0);
			meshMaterial.mainTextureOffset = offset + movement;
			break;

		case PlayerState.glidingLeft:
			movement = new Vector2 (-Time.deltaTime * horizontalScrollingSpeed * 0.5f, 0);
			meshMaterial.mainTextureOffset = offset + movement;
			break;

		case PlayerState.glidingRight:
			movement = new Vector2 (+Time.deltaTime * horizontalScrollingSpeed * 0.5f, 0);
			meshMaterial.mainTextureOffset = offset + movement;
			break;
		}
	}

	void onStateChange(PlayerState newState) {
		if (currentState == newState)
			return;
		
		currentState = newState;
	}
}
