using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum GameState {
		paused,
		unpaused
	};

	GameState currentState = GameState.unpaused;
	public delegate void gameStateHandler(GameState newState);
	public static event gameStateHandler onGameStateChange;

	public void PauseGame() {
		currentState = GameState.paused;
		Time.timeScale = 0f;
		if (onGameStateChange != null)
			onGameStateChange (currentState);
	}

	public void UnpauseGame() {
		currentState = GameState.unpaused;
		Time.timeScale = 1f;
		if (onGameStateChange != null)
			onGameStateChange (currentState);
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (currentState == GameState.unpaused) {
				PauseGame ();
			} else {
				UnpauseGame ();
			}
		}
	}

}
