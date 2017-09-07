using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchLightning : MonoBehaviour {

	// Main bolt and its braches
	List<GameObject> boltsObj = new List<GameObject>();

	public GameObject boltPrefab;

	// Branch is complete if there aren't any bolts
	public bool IsComplete { get { return boltsObj.Count == 0; } }

	public Vector2 BranchStart { get; private set; }

	public Vector2 BranchEnd { get; private set; }

	public void Initialize(Vector2 start, Vector2 end, float thickness) {
		BranchStart = start;
		BranchEnd = end;

		GameObject mainBoltObj = (GameObject)GameObject.Instantiate (boltPrefab);
		mainBoltObj.transform.parent = this.transform;
		LightningBolt mainBoltComp = mainBoltObj.GetComponent<LightningBolt> ();
		mainBoltComp.Initialize (5);
		mainBoltComp.ActivateBolt (start, end, Color.white, thickness);
		boltsObj.Add (mainBoltObj);

		int numBranches = Random.Range (4, 7);
		Vector2 distance = end - start;

		List<float> branchingPoints = new List<float> ();
		for (int i = 0; i < numBranches; i++) {
			branchingPoints.Add (Random.value);
		}
		branchingPoints.Sort ();

		for (int i = 0; i < branchingPoints.Count; i++) {
			// Starting point of this branch
			Vector2 boltStart = mainBoltComp.GetPoint (branchingPoints [i]);
			// Alternate between left and right, rotate around Z-axis
			Quaternion rot = Quaternion.AngleAxis (30 * ((i % 2) == 0 ? 1 : -1),
				                 new Vector3 (0, 0, 1));

			// End point will be 50%-75% of main bolt distance and shrunk
			// down the further we are from the main bolt start
			Vector2 adjust = rot * (Random.Range (.5f, .75f) * distance * (1 - branchingPoints [i]));
			Vector2 boltEnd = adjust + boltStart;

			GameObject boltObj = (GameObject)GameObject.Instantiate (boltPrefab);
			boltObj.transform.parent = this.transform;
			LightningBolt boltComp = boltObj.GetComponent<LightningBolt> ();
			boltComp.Initialize (5);
			boltComp.ActivateBolt (boltStart, boltEnd, Color.white, thickness);

			boltsObj.Add (boltObj);
		}
	}

	public void UpdateBranch() {
		for (int i = boltsObj.Count - 1; i >= 0; i--) {
			GameObject boltObj = boltsObj [i];

			LightningBolt boltComp = boltObj.GetComponent<LightningBolt> ();
			boltComp.UpdateBolt ();

			if (boltComp.IsComplete) {
				boltsObj.RemoveAt (i);
				Destroy (boltObj);
			}
		}
	}

	public void Draw() {
		foreach (GameObject boltObj in boltsObj) {
			boltObj.GetComponent<LightningBolt> ().Draw ();
		}
	}
}
