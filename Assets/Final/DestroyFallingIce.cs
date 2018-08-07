using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFallingIce : MonoBehaviour {

	[SerializeField]
	GameObject parent;
	[SerializeField]
	Rigidbody2D[] ice;

	void OnBecameInvisible () {
		foreach (Rigidbody2D iceElement in ice) {
			iceElement.velocity = Vector2.zero;
		}
		if (parent.activeSelf) {
			parent.SetActive (false);
		}
	}
	
}
