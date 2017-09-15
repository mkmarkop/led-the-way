using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour {

	void Awake() {
		GameManager.onGameStateChange += onGameStateChange;
	}

	void Start() {
		gameObject.SetActive (false);
	}

	void OnDestroy() {
		GameManager.onGameStateChange -= onGameStateChange;
	}

	public void BackToMenu() {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("Main_Menu");
	}

	public void QuitGame() {
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

	void onGameStateChange(GameManager.GameState newState) {
		switch (newState) {
		case GameManager.GameState.paused:
			gameObject.SetActive (true);
			break;
		case GameManager.GameState.unpaused:
			gameObject.SetActive (false);
			break;
		}
	}
}
