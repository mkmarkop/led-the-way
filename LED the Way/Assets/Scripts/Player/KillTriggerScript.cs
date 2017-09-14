using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTriggerScript : MonoBehaviour {

	void OnTriggerExit2D(Collider2D objectEntering) {
		objectEntering.SendMessage ("exitGameZone",
			SendMessageOptions.DontRequireReceiver);
	}
}
