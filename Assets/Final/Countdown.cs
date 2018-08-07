using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {

	[SerializeField]
	Tutorial tutorial;
	Text text;
	int count;

	void Awake() {
		text = GetComponent<Text> ();
	}

	void OnEnable() {
		PatternManager.instance.OnTutorialCompleted ();
		count = 3;
		text.text = count.ToString ();
		StartCoroutine (Next ());
	}

	IEnumerator Next() {
		yield return new WaitForSecondsRealtime (1f);
		count--;
		text.text = count.ToString ();
		if (count == 0) {
			tutorial.Next ();
		} else {
			StartCoroutine (Next ());
		}
	}

}
