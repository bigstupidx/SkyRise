using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

	public bool isMute { get; private set; }
	AudioSource audioSource;
	public static Music instance;

	void Awake() {
		if (LaunchManager.IsFirstLaunchForCurrentSession) {
			instance = this;
			DontDestroyOnLoad (gameObject);
			audioSource = GetComponent<AudioSource> ();
			isMute = PlayerPrefs.GetInt ("Mute", 0) == 1;
			audioSource.mute = isMute;
//			audioSource.Play ();
		} else {
			Destroy (gameObject);
		}
		Debug.Log ("musa");
	}

	public void Stop() {
		audioSource.Stop ();
	}

	public void Play() {
		Stop ();
		audioSource.Play ();
	}

	public void Pause() {
		audioSource.Pause ();
	}

	public void UnPause() {
		audioSource.UnPause ();
	}

	public void Toggle() {
		isMute = !isMute;
		audioSource.mute = isMute;
		PlayerPrefs.SetInt ("Mute", isMute ? 1 : 0);
	}

}
