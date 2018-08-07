using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour {

//	new Transform transform;
	[SerializeField]
	float speed;
	bool shouldRotate;

	void OnBecameVisible() {
		shouldRotate = false;
	}

	void OnBecameInvisible() {
		shouldRotate = true;
	}

	void Update () {
		transform.Rotate (0, 0, speed);
	}
}
