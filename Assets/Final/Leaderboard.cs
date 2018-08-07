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
		leaderboardID = GoogleLeaderboard.leaderboard_score;
		#elif UNITY_IOS
		leaderboardID = "com.skyrise.score";
		#endif
		if (!Social.localUser.authenticated) {
			Social.localUser.Authenticate (OnAuthentication);
		}
		Debug.Log ("la");
		LaunchManager.IsFirstLaunchForCurrentSession = false;
	}

	public void PostScore(int value) {
		Social.Active.ReportScore ((long)value, leaderboardID, null);
	}

	public void ShowLeadeboard() {
		if (Social.localUser.authenticated) {
			GameCenterPlatform.ShowLeaderboardUI (leaderboardID, TimeScope.AllTime);
		} else {
			Social.localUser.Authenticate (
				success => {
					if(success) {
						GameCenterPlatform.ShowLeaderboardUI (leaderboardID, TimeScope.AllTime);
					}
			});
		}
	}

	void OnAuthentication(bool success) {
	}

}
