using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
#else
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class Leaderboard : MonoBehaviour {

	public static Leaderboard instance;
	string leaderboardID;

	void Awake() {
		instance = this;
		#if UNITY_ANDROID
		PlayGamesPlatform.Activate ();
		leaderboardID = GoogleLeaderboard.leaderboard_high_score;
		#elif UNITY_IOS
		leaderboardID = "com.skyrise.score";
		#endif
		if (!Social.localUser.authenticated) {
			Social.localUser.Authenticate (OnAuthentication);
		}
		Debug.Log ("la");
		#if UNITY_IOS
		LaunchManager.IsFirstLaunchForCurrentSession = false;
		#endif
	}

	public void PostScore(int value) {
		Social.Active.ReportScore ((long)value, leaderboardID, null);
	}

	public void ShowLeadeboard() {
		if (Social.localUser.authenticated) {
			#if UNITY_ANDROID
			PlayGamesPlatform.Instance.ShowLeaderboardUI (GoogleLeaderboard.leaderboard_high_score);
			#elif UNITY_IOS
			GameCenterPlatform.ShowLeaderboardUI (leaderboardID, TimeScope.AllTime);
			#endif
		} else {
			Social.localUser.Authenticate (
				success => {
					if(success) {
						#if UNITY_ANDROID
						PlayGamesPlatform.Instance.ShowLeaderboardUI (GoogleLeaderboard.leaderboard_high_score);
						#elif UNITY_IOS
						GameCenterPlatform.ShowLeaderboardUI (leaderboardID, TimeScope.AllTime);
						#endif
					}
			});
		}
	}

	void OnAuthentication(bool success) {
	}

}
