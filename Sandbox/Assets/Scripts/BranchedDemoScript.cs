using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchedDemoScript : MonoBehaviour {

	public GameObject BranchedBoltPrefab;
	List<GameObject> boltsObj;

	int clicks = 0;
	Vector2 pos1, pos2;
	// Use this for initialization
	void Start () {
		boltsObj = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			if (clicks == 0) {
				Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				pos1 = new Vector2 (temp.x, temp.y);
			} else if (clicks == 1) {
				Vector3 temp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				pos2 = new Vector2 (temp.x, temp.y);

				GameObject newBolt = (GameObject)Instantiate (BranchedBoltPrefab);
				newBolt.SetActive (true);
				BranchLightning bComp = newBolt.GetComponent<BranchLightning> ();
				bComp.Initialize (pos1, pos2);
				boltsObj.Add (newBolt);
			}

			clicks++;
			if (clicks > 1) clicks = 0;
		}

		for (int i = 0; i < boltsObj.Count; i++) {
			boltsObj [i].GetComponent<BranchLightning> ().UpdateBranch ();
			boltsObj [i].GetComponent<BranchLightning> ().Draw ();
		}
	}
}
