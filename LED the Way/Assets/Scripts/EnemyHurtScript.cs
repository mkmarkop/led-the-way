using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyHurtScript : MonoBehaviour {

	public GameObject enemyObject;

	void OnTriggerEnter2D(Collider2D colliderEntering) {
		if (colliderEntering.tag == "Bullet") {
			enemyObject.SendMessage ("hitByBullet");
		}
	}
}
