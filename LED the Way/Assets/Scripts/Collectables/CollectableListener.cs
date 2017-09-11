using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CollectableListener : MonoBehaviour {

	int numCollected = 0;

	public const string SEPARATOR = "x ";
	private Text text;

	void Awake() {
		text = GetComponent<Text> ();
	}

	void OnEnable() {
		CollectableController.onCollect += onCollected;
	}

	void OnDisable() {
		CollectableController.onCollect -= onCollected;
	}

	// Use this for initialization
	void Start () {
		text.text = SEPARATOR + numCollected.ToString ();
	}

	void onCollected() {
		numCollected++;
		text.text = SEPARATOR + numCollected.ToString ();
	}
}
