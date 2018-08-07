using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio {

	public bool IsMute { get; private set; }

	Audio() {
		IsMute = PlayerPrefs.GetInt ("IsMute", 0) == 1 ? true : false;
	}

	private static Audio instance;

	public static Audio Instance {
		get {
			if (instance == null) {
				instance = new Audio ();
			}
			return instance;
		}
	}

	public void SaveData() {
		PlayerPrefs.SetInt ("IsMute", IsMute ? 1 : 0);
	}

	public void Toggle() {
		IsMute = !IsMute;
	}
}
