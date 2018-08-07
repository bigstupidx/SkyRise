using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRiseAds : MonoBehaviour {
	#if !UNITY_EDITOR

	string SDK_KEY = "lPtUOU5o5_HVBHBeTrSEvih-uZemi7NSWFxvk1hp0zJBFUqoJG1C-kBtHrd-y_QTELMzN3nN04ZFTO2sf43eoF";
	public static SkyRiseAds instance;
	const int interAdGap = 10;
	int gameOverCount;
	Action OnInterClosed;
	Action OnRewardedInterClosed;
	Action OnRewardedInterFailed;
	public bool AdsRemoved { get; private set; }
	bool IsInit;

	void Awake() {
		if (LaunchManager.IsFirstLaunchForCurrentSession) {
			AdsRemoved = PlayerPrefs.GetInt ("RemoveAds", 0) == 1;
			DontDestroyOnLoad (gameObject);
			instance = this;
			if (!LaunchManager.IsFirstSession) {
				Init ();
			}
		} else {
			if (!SkyRiseAds.instance.IsInit) {
				SkyRiseAds.instance.Init ();
			}
			Destroy (gameObject);
		}
		Debug.Log ("skya");
	}

	public void Init () {
		if (!IsInit) {
			Debug.Log ("skyi");
			AppLovin.SetSdkKey (SDK_KEY);
			AppLovin.InitializeSdk ();
			AppLovin.SetUnityAdListener (gameObject.name);
			IsInit = true;
			Debug.Log ("ads are removed: " + AdsRemoved);
			if (!AdsRemoved) {
				AppLovin.PreloadInterstitial ();
				ShowBanner ();
			}
			AppLovin.LoadRewardedInterstitial ();
		}
	}
	
	void ShowInterstitial (Action OnInterClosed)
	{
		if (!AdsRemoved) {
			this.OnInterClosed = OnInterClosed;
			AppLovin.ShowInterstitial ();
		}
	}

	public bool IsRewarededInterLoaded() {
		if (!IsInit) {
			return false;
		}
		return AppLovin.IsIncentInterstitialReady ();
	}

	public void ShowRewardedInterstitial (Action Success = null, Action Failed = null)
	{
		Debug.Log ("avail: " + AppLovin.IsIncentInterstitialReady ());
		if (AppLovin.IsIncentInterstitialReady ()) {
			OnRewardedInterClosed = Success;
			OnRewardedInterFailed = Failed;
			AppLovin.ShowRewardedInterstitial ();
		} else {
			Failed ();
		}
	}

	void ShowBanner ()
	{
	Debug.Log("banner1");
		if (!AdsRemoved) {
	Debug.Log("banner2");
			AppLovin.ShowAd (AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_BOTTOM);
		}
	}

	private void onAppLovinEventReceived (string value)
	{
		Debug.Log(value);
		switch (value) {
		case "HIDDENREWARDED":
			Music.instance.UnPause ();
			OnRewardedInterClosed ();
			AppLovin.LoadRewardedInterstitial ();
			break;

		case "HIDDENINTER":
			Music.instance.UnPause ();
			if (OnInterClosed != null) {
				OnInterClosed ();
			}
			AppLovin.PreloadInterstitial ();
			break;
		
		case "REWARDREJECTED":
		case "REWARDOVERQUOTA":
		case "REWARDTIMEOUT":
			OnRewardedInterFailed ();
			break;
		
		case "DISPLAYEDREWARDED":
		case "DISPLAYEDINTER":
			Music.instance.Pause ();
			break;
		}
	}

	public void OnGameOver(Action Success = null) {
		if (IsInit) {
			if (!AdsRemoved) {
				gameOverCount++;
				Debug.Log ("GCO: " + gameOverCount);
				if (gameOverCount % interAdGap == 0) {
					ShowInterstitial (Success);
				} else {
					if (Success != null) {
						Success ();
					}
				}
			}
		} else {
			if (Success != null) {
				Success ();
			}
		}
	}

	void SaveData() {
		PlayerPrefs.SetInt ("RemoveAds", AdsRemoved ? 1 : 0);
	}

	void OnDisable() {
		SaveData ();
	}

	void OnApplicationPause(bool status) {
		if (status) {
			SaveData ();
		}
	}

	#endif
}