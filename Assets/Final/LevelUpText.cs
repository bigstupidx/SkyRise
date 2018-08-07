using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpText : MonoBehaviour {

	[SerializeField]
	MeshRenderer[] textMesh;

	void Awake() {
		foreach (MeshRenderer text in textMesh) {
			text.sortingOrder = 20;
		}
	}

}
