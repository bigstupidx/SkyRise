using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour {

	public Pattern[] patterns;
	public int previousPattern { get; private set; }
	int nextPattern;
	int visiblePatterns = 3;
	float gap = 1.5f;
	List<int> indices = new List<int> ();

	public static PatternManager instance;

	void Awake() {
		instance = this;
		SetPatternOrder ();
	}

	void SetPatternOrder() {
		int count = patterns.Length;
		int temp = 0;
		while (temp < count) {
			indices.Add (temp);
			temp++;
		}
		temp = 0;
		List<Pattern> patternsList = new List<Pattern> ();
		int randomIndex = -1;
		int notAllowedValue = -1;
		while (temp < count) {
			randomIndex = FindRandomIndex (notAllowedValue);
			if (randomIndex % 2 == 0) {
				notAllowedValue = randomIndex + 1;
			} else {
				notAllowedValue = randomIndex - 1;
			}
			patternsList.Add (patterns [randomIndex]);
			indices.Remove (randomIndex);
			temp++;
		}
		patterns = patternsList.ToArray ();
	}

	int FindRandomIndex(int notAllowedValue = -1) {
		int randomNumber = indices [Random.Range (0, indices.Count)];
		if (randomNumber != notAllowedValue) {
			return randomNumber;
		} else {
			return FindRandomIndex (notAllowedValue);
		}
	}

	void Start() {
		if (Player.instance.isTutorialOver) {
			Init ();
		}
	}

	public void OnTutorialCompleted() {
		Init ();
	}

	void Init() {
		previousPattern = -1;
		nextPattern = visiblePatterns - 1;

		Vector3 currentPosition = patterns [0].parent.position;
		currentPosition.y = Player.instance.transform.position.y - 0.7f;
		patterns [0].parent.position = currentPosition;
		patterns [0].parent.gameObject.SetActive (true);

		float position = patterns [0].lastObject.transform.position.y + gap;
		currentPosition = patterns [1].parent.position;
		currentPosition.y = position;
		patterns [1].parent.position = currentPosition;
		patterns [1].parent.gameObject.SetActive (true);

		position = patterns [1].lastObject.transform.position.y + gap;
		currentPosition = patterns [2].parent.position;
		currentPosition.y = position;
		patterns [2].parent.position = currentPosition;
		patterns [2].parent.gameObject.SetActive (true);

		GameManager.instance.ChangeThemeNow ();
	}

	public void SpawnNewPattern() {
		UIManager.instance.UpdateLevelText ();
		if (previousPattern > -1) {
			patterns [previousPattern].parent.gameObject.SetActive (false);
		}
		previousPattern++;
		if (previousPattern == patterns.Length - 1) {
			//game complete
			Player.instance.OnGameCompleted();
			UIManager.instance.ShowRateUs();
			Time.timeScale = 0f;
			UIManager.instance.OnGameCompleted ();
		} else {
//			patterns [previousPattern].cloudAnimation.Init ();
			if (nextPattern != -1) {
				float position = patterns [nextPattern].lastObject.transform.position.y + gap;
				nextPattern++;
				if (nextPattern == patterns.Length) {
					nextPattern = -1;
				} else {
					Vector3 currentPosition = patterns [nextPattern].parent.position;
					currentPosition.y = position;
					patterns [nextPattern].parent.position = currentPosition;
					patterns [nextPattern].parent.gameObject.SetActive (true);
				}
			}
		}
	}

}
