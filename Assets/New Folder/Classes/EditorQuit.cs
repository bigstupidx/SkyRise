using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorQuit : MonoBehaviour {

	void Awake() {
		#if !UNITY_EDITOR
		Destroy(this);
		#endif
	}

	void OnApplicationQuit() {
		GameManager.instance.isGameOver = true;
	}

}
