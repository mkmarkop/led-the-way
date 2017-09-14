using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerHealthListener : MonoBehaviour {

	public Sprite[] batteryImages = new Sprite[PlayerStateMachine.MaxHealth + 1];

	private Image image;

	void OnEnable() {
		PlayerStateMachine.onReceiveDamage += onHealthChange;
	}

	void OnDisable() {
		PlayerStateMachine.onReceiveDamage -= onHealthChange;
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
