using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingIce : MonoBehaviour {

	[SerializeField]
	float speed;
	[SerializeField]
	Rigidbody2D[] ice; 

	void OnBecameVisible () {
		foreach (Rigidbody2D iceElement in ice) {
			iceElement.velocity = Vector2.down * speed;
		}
	}
	
}
