using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum RateUsState {Remind = 0, Never = 1, Rated = 2};

public class UIManager : MonoBehaviour {

	public static UIManager instance;
	[SerializeField]
	GameObject gameOverScreen;
	public GameObject rateUsScreen;
	[SerializeField]
	GameObject continueScreen;
	[SerializeField]
	GameObject gameScreen;
//	int restartAttempts;
//	int maxRestartAttempts = 1;
	[SerializeField]
	GameObject continueButton;
	[SerializeField]
	Transform mainCamera;
	[SerializeField]
	Transform player;
	[SerializeField]
	TrailRenderer playerTrail;
	[SerializeField]
	Text levelText;
	int level;
	[SerializeField]
	Text diamondsText;
	[SerializeField]
	Text scoreText;
	[SerializeField]
	Tutorial tutorial;
	public RateUsState rateUsState;
	[SerializeField]
	ContinueScreen continueScreenMono;
	[SerializeField]
	GameObject notEnoughDiamonds;

	void OnDisable() {
		PlayerPrefs.SetInt ("RateUs", (int)rateUsState);
	}

	void OnApplicationPause(bool status) {
		if (status) {
			PlayerPrefs.SetInt ("RateUs", (int)rateUsState);
		}
	}

	void Awake() {
		rateUsState = (RateUsState)PlayerPrefs.GetInt ("RateUs", 0);
		level = 1;
		CustomAnalytics.Event ("Level" + level);
		instance = this;
//		restartAttempts = 0;
		UpdateDiamondsText ();
	}

	void Start() {
		if (!Player.instance.isTutorialOver) {
			tutorial.Init ();
		} else {
			OnTutorialCompleted ();
		}
	}

	public void OnTutorialCompleted() {
		gameScreen.SetActive (true);
	}

	public void ContinueScreen() {
		gameScreen.SetActive (false);
		Time.timeScale = 0f;
		if (!Player.instance.isTutorialOver) {
			tutorial.End ();
			GameOver ();
			return;
		}
//		#if UNITY_EDITOR
//		if (restartAttempts == maxRestartAttempts) {
//			GameOver();
//			return;
//		}
//		#else
//		if (restartAttempts == maxRestartAttempts || !SkyRiseAds.instance.IsRewarededInterLoaded ()) {
//			GameOver();
//			return;
//		}
//		#endif
//		restartAttempts++;
//		#if !UNITY_EDITOR
//		SkyRiseAds.instance.Init ();
//		#endif
		CustomAnalytics.Event("LevelQuit" + level);
		continueScreen.SetActive (true);
	}

//	public void GameOver() {
//		GameVariables.Instance.UpdateHighScore (int.Parse (scoreText.text));
//		#if UNITY_EDITOR
//		if (restartAttempts == maxRestartAttempts) {
//			continueButton.SetActive (false);
//		}
//		#else
//		if (restartAttempts == maxRestartAttempts || !SkyRiseAds.instance.IsRewarededInterLoaded ()) {
//		continueButton.SetActive (false);
//		}
//		#endif
//		restartAttempts++;
//		gameScreen.SetActive (false);
//		gameOverScreen.SetActive(true);
//		Time.timeScale = 0f;
//	}

	public void ShowRateUs() {
		if (rateUsState == RateUsState.Remind) {
			rateUsScreen.SetActive (true);
		}
	}

	public void OnGameCompleted() {
		int score = int.Parse (scoreText.text);
		Leaderboard.instance.PostScore (score);
		GameVariables.Instance.UpdateHighScore (score);
	}

	public void GameOver() {
		continueScreen.SetActive (false);
		int score = int.Parse (scoreText.text);
		Leaderboard.instance.PostScore (score);
		GameVariables.Instance.UpdateHighScore (score);
		gameOverScreen.SetActive(true);
		if (rateUsState == RateUsState.Remind) {
			if (GameVariables.Instance.attempts % 20 == 0) {
				rateUsScreen.SetActive (true);
			}
		}
		#if !UNITY_EDITOR
		SkyRiseAds.instance.OnGameOver ();
		#endif
	}

	public void Home() {
		Time.timeScale = 1f;
//		#if UNITY_EDITOR
		HomeScene();
//		#else
//		SkyRiseAds.instance.OnGameOver (HomeScene);
//		#endif
	}

	void HomeScene() {
		SceneManager.LoadScene (0);
		Music.instance.Play ();
	}

	public void Restart() {
		Time.timeScale = 1f;
		Music.instance.Stop ();
//		#if UNITY_EDITOR
		SceneManager.LoadScene (1);
//		#else
//		SkyRiseAds.instance.OnGameOver (() => SceneManager.LoadScene (1));
//		#endif
	}

	public void ContinueViaDiamonds() {
		if (Shop.Instance.diamonds >= continueScreenMono.diamonds) {
			CustomAnalytics.Event ("ContinueDiamonds" + continueScreenMono.diamonds);
			Shop.Instance.UpdateDiamonds (-continueScreenMono.diamonds);
			continueScreenMono.diamonds += 5;
			ContinueSuccess ();
		} else {
			notEnoughDiamonds.SetActive (true);
		}
	}

	public void Continue() {
		CustomAnalytics.Event ("ContinueVideo");
		continueScreenMono.videoWatchCount++;
		continueScreen.SetActive (false);
		#if UNITY_EDITOR
			ContinueSuccess ();
		#else
			SkyRiseAds.instance.ShowRewardedInterstitial (ContinueSuccess, GameOver);
		#endif
	}

	void ContinueSuccess() {
		Music.instance.Play ();
		UpdateDiamondsText ();
		Player.instance.UpdateBall ();
//		gameOverScreen.SetActive (false);
		continueScreen.SetActive (false);
		gameScreen.SetActive (true);
		GameManager.instance.isGameOver = false;
		playerTrail.Clear ();
		Player.instance.Reset ();
		Time.timeScale = 1f;
		int pattern = PatternManager.instance.previousPattern + 1;
		if (pattern == PatternManager.instance.patterns.Length) {
			pattern = 0;
		}
		PatternManager.instance.patterns [pattern].Reset ();
		Vector3 lastPattern = PatternManager.instance.patterns [pattern].transform.position;
		Vector3 temp = Vector3.zero;
		temp.y = lastPattern.y;
		player.position = temp;
		temp.y = lastPattern.y + 2.5f;
		temp.z = -1f;
		mainCamera.position = temp;
		player.eulerAngles = Vector3.zero;
	}

	public void UpdateLevelText() {
		if (level == PatternManager.instance.patterns.Length) {
			return;
		} else {
			level++;
			CustomAnalytics.Event ("Level" + level);
		}
		levelText.text = level.ToString ();
	}

	public void UpdateDiamondsText() {
		diamondsText.text = Shop.Instance.diamonds.ToString ();
	}

}
