using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchLightning : MonoBehaviour {

	List<GameObject> boltsObj = new List<GameObject>();

	public GameObject boltPrefab;

	public bool IsComplete { get { return boltsObj.Count == 0; } }

	public Vector2 Start { get; private set; }
	public Vector2 End { get; private set; }

	public void Initialize(Vector2 start, Vector2 end) {
		Start = start;
		End = end;
		GameObject mainBoltObj = (GameObject)GameObject.Instantiate (boltPrefab);
		LightningBolt mainBoltComponent = mainBoltObj.GetComponent<LightningBolt> ();
		mainBoltComponent.Initialize (5);
		mainBoltComponent.ActivateBolt (start, end, Color.white, 0.25f);
		boltsObj.Add (mainBoltObj);

		int numBranches = Random.Range (3, 6);
		Vector2 diff = end - start;
		List<float> branchPoints = new List<float> ();
		for (int i = 0; i < numBranches; i++)
			branchPoints.Add (Random.value);
		branchPoints.Sort ();

		for (int i = 0; i < branchPoints.Count; i++) {
			Vector2 boltStart = mainBoltComponent.GetPoint (branchPoints [i]);
			Quaternion rot = Quaternion.AngleAxis (30 * ((i & 1) == 0 ? 1 : -1),
				                 new Vector3 (0, 0, 1));
			Vector2 adjust = rot * (Random.Range (.5f, .75f) * diff *
			                 (1 - branchPoints [i]));
			Vector2 boltEnd = adjust + boltStart;
			GameObject boltObj = (GameObject)GameObject.Instantiate (boltPrefab);
			LightningBolt boltComponent = boltObj.GetComponent<LightningBolt> ();
			boltComponent.Initialize (5);

			boltComponent.ActivateBolt (boltStart, boltEnd, Color.white, 0.25f);
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
