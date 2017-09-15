using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

	[Range(0f, 1f)]
	public float boltThickness;

	public GameObject BranchedBoltPrefab;
	public List<GameObject> BoltNodes;

	public float timeBetweenBolts = 1.5f;

	public const float MinimumHeight = 150f;
	bool active = true;

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
		if (active) {
			Vector3 randomCoordinate = new Vector3 (Random.Range (0, Screen.width),
				                           Random.Range (0, Screen.height - MinimumHeight), 0f);
			Vector3 startPos = Camera.main.ScreenToWorldPoint (randomCoordinate);

			GameObject branchObj = (GameObject)GameObject.Instantiate (BranchedBoltPrefab);
			BranchLightning branchComp = branchObj.GetComponent<BranchLightning> ();

			branchComp.Initialize (randomNode (), startPos, boltThickness);
			branchesObj.Add (branchObj);
			active = false;
			StartCoroutine (ActivateCloud());
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

	IEnumerator ActivateCloud() {
		yield return new WaitForSeconds (timeBetweenBolts + Random.Range(-0.1f, 0.3f));
		active = true;
		yield return null;
	}
}
