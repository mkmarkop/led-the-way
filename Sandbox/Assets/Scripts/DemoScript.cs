using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour {

	public GameObject BoltPrefab;

	List<GameObject> activeBoltsObj;
	List<GameObject> inactiveBoltsObj;
	int maxBolts = 1000;

	int clicks = 0;
	Vector2 pos1, pos2;

	// Use this for initialization
	void Start () {
		activeBoltsObj = new List<GameObject> ();
		inactiveBoltsObj = new List<GameObject> ();

		GameObject p = GameObject.Find ("LightningPoolHolder");

		for (int i = 0; i < maxBolts; i++) {
			GameObject bolt = (GameObject)Instantiate (BoltPrefab);
			bolt.transform.parent = p.transform;
			bolt.GetComponent<LightningBolt> ().Initialize (25);
			bolt.SetActive (false);
			inactiveBoltsObj.Add (bolt);
		}
	}
	
	// Update is called once per frame
	void Update () {
		GameObject boltObj;
		LightningBolt boltComponent;

		int activeLineCount = activeBoltsObj.Count;

		// Go from backwards, so we can remove them from list
		for (int i = activeLineCount - 1; i >= 0; i--) {
			// pull GameObject
			boltObj = activeBoltsObj [i];
			boltComponent = boltObj.GetComponent<LightningBolt> ();

			if (boltComponent.IsComplete) {
				boltComponent.DeactivateSegments ();
				boltObj.SetActive (false);
				activeBoltsObj.RemoveAt (i);
				inactiveBoltsObj.Add (boltObj);
			}
		}

		if (Input.GetMouseButtonDown (0)) {
			if (clicks == 0) {
				Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				pos1 = new Vector2 (temp.x, temp.y);
			} else if (clicks == 1) {
				Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				pos2 = new Vector2 (temp.x, temp.y);
				CreatePooledBolt (pos1, pos2, Color.white, 1f);
			}

			clicks++;
			if (clicks > 1) clicks = 0;
		}

		for (int i = 0; i < activeBoltsObj.Count; i++) {
			activeBoltsObj [i].GetComponent<LightningBolt> ().UpdateBolt ();
			activeBoltsObj [i].GetComponent<LightningBolt> ().Draw ();
		}
	}

	void CreatePooledBolt(Vector2 source, Vector2 dest,
		Color color, float thickness) {
		if (inactiveBoltsObj.Count <= 0) return;

		GameObject boltObj = inactiveBoltsObj [inactiveBoltsObj.Count - 1];
		boltObj.SetActive (true);

		activeBoltsObj.Add (boltObj);
		inactiveBoltsObj.RemoveAt (inactiveBoltsObj.Count - 1);

		LightningBolt boltComponent = boltObj.GetComponent<LightningBolt> ();
		boltComponent.ActivateBolt (source, dest, color, thickness);
	}
}
