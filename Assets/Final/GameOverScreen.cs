using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {

	[SerializeField]
	Text level;
	[SerializeField]
	Text attempts;
	[SerializeField]
	Text score;
	[SerializeField]
	Text highScore;
	[SerializeField]
	Text scoreInGame;
	[SerializeField]
	Text levelInGame;

	void OnEnable() {
		score.text = scoreInGame.text;
		level.text = levelInGame.text + "/10";
		attempts.text = GameVariables.Instance.attempts.ToString ();
		highScore.text = GameVariables.Instance.highScore.ToString ();
	}

	public void RemoveAds() {
//		IAP.instance.RemoveAds ();
	}

	public void ShowLeaderboard() {
		Leaderboard.instance.ShowLeadeboard ();
	}

}
