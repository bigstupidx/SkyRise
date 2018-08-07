using System;
using UnityEngine;

public class Pattern : MonoBehaviour {

	public Transform parent;
	public LastObject lastObject;
	public Transform levelCompleteUI;
	public Color color;
//	public CloudAnimation cloudAnimation;
	[HideInInspector]
	public bool isComplete;
	Vector3 gapBetweenExitTriggerAndLevelCompleteUI = Vector3.zero;
	[SerializeField]
	GameObject[] deactivate;
	[SerializeField]
	Transform[] positionReset;
	[SerializeField]
	Vector3[] localPositions;
	[SerializeField]
	Vector3[] localRotations;
	[SerializeField]
	GameObject[] activate;

	void Awake() {
		if (levelCompleteUI != null) {
			levelCompleteUI.position = lastObject.transform.position + gapBetweenExitTriggerAndLevelCompleteUI;
		}
	}

	public void Reset() {
		foreach (GameObject g in deactivate) {
			g.SetActive (false);
		}
		int temp = 0;
		while (temp < positionReset.Length) {
			positionReset [temp].localPosition = localPositions [temp];
			positionReset [temp].localEulerAngles = localRotations [temp];
			temp++;
		}

		foreach (GameObject g in activate) {
			g.SetActive (true);
		}
	}
}
