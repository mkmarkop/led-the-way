using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageTriggerScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D objectEntering) {
		objectEntering.SendMessage ("hitDamageTrigger",
			SendMessageOptions.DontRequireReceiver);
	}
}
