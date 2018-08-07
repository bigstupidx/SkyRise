using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
struct Theme {
	public Color background;
	public Color player;
	public Color arrow;
}

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	[HideInInspector]
	public bool isGameOver;
	[SerializeField]
	Camera background;
	[SerializeField]
	SpriteRenderer player;
	[SerializeField]
	SpriteRenderer arrow;
	[SerializeField]
	Theme[] themes;
	int currentTheme = -1;
	int score;
	[SerializeField]
	Text scoreText;
	float initial;
	float factor = 0.02f;
	[SerializeField]
	Transform playerT;
	[SerializeField]
	Text diamondsText;

	void Awake () {
		diamondsText.text = Shop.Instance.diamonds.ToString ();
		instance = this;
	}

	public void OnTutorialCompleted() {
		Music.instance.Play ();
		initial = playerT.position.y;
	}

	void Start() {
		if (Player.instance.isTutorialOver) {
			OnTutorialCompleted ();
		}
		// scoreCounter = 0;
//		 StartCoroutine(IncrementScore());
//		StartCoroutine(ChangeTheme());
	}

	public void IncrementScore() {
			int currentValue = (int)((playerT.position.y - initial)/0.02f);
			if(currentValue>score) {
				score = currentValue;
			}
		scoreText.text = score.ToString ();
	}

	IEnumerator ChangeTheme() {
		yield return new WaitForSecondsRealtime(10f);
		currentTheme++;
		if(currentTheme == themes.Length) {
			currentTheme = 0;
		}
		// background.backgroundColor = themes[currentTheme].background;
		// player.color = themes[currentTheme].player;
		// arrow.color = themes[currentTheme].arrow;
		StartCoroutine(ChangeTheme());
	}

//	void Update() {
//		background.backgroundColor = Color.Lerp(background.backgroundColor,themes[currentTheme].background,Time.deltaTime);
//
//	}

//	bool startUpdate;
//
//	public void ChangeThemeNow() {
//		Debug.Log ("p theme: " + currentTheme);
//		currentTheme++;
//		if (currentTheme == PatternManager.instance.patterns.Length) {
//			currentTheme = 0;
//		}
//		startUpdate = true;
//		Debug.Log ("theme changed to " + currentTheme);
//	}
//
//	void Update() {
//		if (startUpdate) {
//			background.backgroundColor = Color.Lerp (background.backgroundColor, PatternManager.instance.patterns [currentTheme].color, Time.deltaTime * 2f);
//		}
//	}


	public void ChangeThemeNow() {
		currentTheme++;
		if (currentTheme == PatternManager.instance.patterns.Length) {
			currentTheme = 0;
		}
//		Debug.Log (currentTheme);
		background.backgroundColor = PatternManager.instance.patterns [currentTheme].color;
	}

}
