using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTriggerScript : MonoBehaviour {

	public int damageAmount = 1;

	void OnTriggerEnter2D(Collider2D objectEntering) {
		objectEntering.SendMessage ("hitDamageTrigger", damageAmount,
			SendMessageOptions.DontRequireReceiver);
	}
}
