using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

	[Range(0f, 1f)]
	public float boltThickness;

	public GameObject BranchedBoltPrefab;
	public List<GameObject> BoltNodes;

	List<GameObject> branchesObj;

	void Start() {
		branchesObj = new List<GameObject> ();
	}

	Vector2 randomNode() {
		Vector3 nodePos = BoltNodes [Random.Range (0, BoltNodes.Count)]
			.transform.position;
		return new Vector2 (nodePos.x, nodePos.y);
	}

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			GameObject branchObj = (GameObject)GameObject.Instantiate (BranchedBoltPrefab);
			BranchLightning branchComp = branchObj.GetComponent<BranchLightning> ();

			branchComp.Initialize (randomNode (), mousePos, boltThickness);
			branchesObj.Add (branchObj);
		}
		for (int i = branchesObj.Count - 1; i >= 0; i--) {
			BranchLightning branchComp = branchesObj [i].GetComponent<BranchLightning> ();

			if (branchComp.IsComplete) {
				Destroy (branchesObj [i]);
				branchesObj.RemoveAt (i);
				continue;
			}

			branchComp.UpdateBranch ();
			branchComp.Draw ();
		}
	}
}
