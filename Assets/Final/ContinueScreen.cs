using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScreen : MonoBehaviour {

	[SerializeField]
	Text countdownText;
	int countdown;
	[SerializeField]
	Text diamondsText;
	[SerializeField]
	GameObject continueViaVideo;
	public int diamonds;
	public int videoWatchCount;

	void Awake() {
		videoWatchCount = 0;
		diamonds = 5;
	}

	void OnEnable () {
		if (videoWatchCount < 3) {
			#if UNITY_EDITOR
			continueViaVideo.SetActive (true);
			#else 
			continueViaVideo.SetActive(SkyRiseAds.instance.IsRewarededInterLoaded ());
			#endif
		} else {
			continueViaVideo.SetActive (false);
		}
		diamondsText.text = diamonds.ToString ();
		countdown = 30;
		countdownText.text = countdown.ToString ();
		StartCoroutine (Countdown ());
	}
	
	IEnumerator Countdown() {
		yield return new WaitForSecondsRealtime (1f);
		countdown--;
		countdownText.text = countdown.ToString ();
		if (countdown > 0) {
			StartCoroutine (Countdown ());
		} else {
			UIManager.instance.GameOver ();
		}
	}

	void OnDisable() {
		StopCoroutine (Countdown ());
	}
}
