using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerHealthListener : MonoBehaviour {

	public Sprite[] batteryImages = new Sprite[PlayerStateListener.MAX_HEALTH + 1];

	private Image image;

	void OnEnable() {
		PlayerStateListener.onReceiveDamage += onHealthChange;
	}

	void OnDisable() {
		PlayerStateListener.onReceiveDamage -= onHealthChange;
	}

	void Awake() {
		image = GetComponent<Image> ();
	}

	void onHealthChange(int newHealth) {
		if (newHealth < 0 || newHealth >= batteryImages.Length)
			return;

		image.sprite = batteryImages [newHealth];
	}
}
