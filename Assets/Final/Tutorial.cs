using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

	[SerializeField]
	GameObject[] states;
	public int state;
	[SerializeField]
	GameObject tutorialClouds;
	[SerializeField]
	GameObject tutorialParent;
	[SerializeField]
	GameObject ballPointer;

	public void Init() {
		state = 0;
		ballPointer.SetActive (true);
		states [state].SetActive (true);
		tutorialClouds.SetActive (true);
	}

	public void Next() {
		if (state == 0) {
			ballPointer.SetActive (false);
		}
		state++;
//		if (state > 1) {
			states [state - 1].SetActive (false);
//		}
//		if (state == 3 || state == 4) {
//			StartCoroutine (NextAfterDelay ());
//		}
		if (state == states.Length) {
			foreach (Transform child in tutorialClouds.transform) {
				if (!child.GetComponent<SpriteRenderer> ().isVisible) {
					Destroy (child.gameObject);
				}
			}
			Player.instance.isTutorialOver = true;
			Player.instance.Reset ();
			UIManager.instance.OnTutorialCompleted ();
			GameManager.instance.OnTutorialCompleted ();
			PlayerPrefs.SetInt ("Tutorial", 1);
			CustomAnalytics.Event ("TutorialCompleted");
			End ();
		} else {
			states [state].SetActive (true);
		}

	}

	public void End() {
		Destroy (tutorialParent);
	}

//	IEnumerator NextAfterDelay() {
//		yield return new WaitForSecondsRealtime (2f);
//		Next ();
//	}

}
