using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidTrigger : MonoBehaviour {

	[SerializeField]
	GameObject[] asteroid;

	public void Set() {
		foreach (GameObject a in asteroid) {
			a.SetActive (true);
		}
	}
}
