using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LaunchScreen : MonoBehaviour {

	[SerializeField]
	GameObject homeScreen;
	[SerializeField]
	GameObject shopScreen;
	[SerializeField]
	Image mute;
	[SerializeField]
	Sprite muteOn;
	[SerializeField]
	Sprite muteOff;
	[SerializeField]
	AudioSource audioSource;

	void Awake () {
		LaunchManager.IsFirstSession = !PlayerPrefs.HasKey ("Launch");
		#if UNITY_ANDROID
		PlayerPrefs.SetInt ("Launch", 1);
		#elif UNITY_IOS
		if (LaunchManager.IsFirstSession) {
			PlayerPrefs.SetInt ("Launch", 1);
			audioSource.Stop ();
			SceneManager.LoadScene (1);
		}
		#endif
	}

	void Start() {
		mute.sprite = Music.instance.isMute ? muteOn : muteOff;
		#if UNITY_ANDROID
		LaunchManager.IsFirstLaunchForCurrentSession = false;
		#endif
	}

	public void Play() {
		Music.instance.Stop ();
		SceneManager.LoadScene (1);
	}

	public void OpenShop() {
		homeScreen.SetActive (false);
		shopScreen.SetActive (true);
	}

	public void BackToHome() {
		shopScreen.SetActive (false);
		homeScreen.SetActive (true);
	}

	public void ToggleAudio() {
//		audioManager.Toggle ();
		Music.instance.Toggle();
		mute.sprite = Music.instance.isMute ? muteOn : muteOff;
	}

	public void ShowLeaderboard() {
		Leaderboard.instance.ShowLeadeboard ();
	}

//	void OnDisable() {
//		audioManager.SaveData ();
//	}
}

public class LaunchManager {
	public static bool IsFirstSession;
	public static bool IsFirstLaunchForCurrentSession = true;
}