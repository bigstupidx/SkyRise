using System.Collections;
using UnityEngine;

public class GameVariables {

	private static GameVariables instance;
	public float rotationSpeed { get; private set; }
	public float movementSpeed { get; private set; }
	public float normalGameSpeed { get; private set; }
	public float fastGameSpeed { get; private set; }
	private const float difficultyFactor = 1.06f;
	public bool isGameOver;
	int times = 0;
	float lowSpeed = 0.21f;
	float highSpeed = 2.1f;
	public int highScore { get; private set; }
	public int attempts { get; private set; }


	private GameVariables() {
		Application.targetFrameRate = 60;
	}

	public static GameVariables Instance {
		get {
			if (instance == null) {
				instance = new GameVariables ();
			}
			return instance;
		}
	}

	public void High() {
		movementSpeed = highSpeed;
	}

	public void Low() {
		movementSpeed = lowSpeed;
	}

	public void Init() {
		highScore = PlayerPrefs.GetInt ("HighScore", 0);
		attempts = PlayerPrefs.GetInt ("Attempts", 0);
		attempts++;
//		isGameOver = true;
		rotationSpeed = 150f;
		High ();
//		movementSpeed = 2.1f;
		normalGameSpeed = 0.005f;
		fastGameSpeed = 0.075f;
//		fastGameSpeed = normalGameSpeed;
	}

	public void IncreaseRotationSpeed() {
		rotationSpeed += 5f;
	}

	public void ToggleRotationDirection() {
		rotationSpeed = -rotationSpeed;
	}

	public void UpdateHighScore(int value) {
		if (value > highScore) {
			highScore = value;
		}
	}

	public void IncrementAttempts() {
		attempts++;
	}

	public void SaveData() {
		PlayerPrefs.SetInt ("HighScore", highScore);
		PlayerPrefs.SetInt ("Attempts", attempts);
	}

	/*
	void OnEnable() {
		//		StartCoroutine (IncreaseDifficulty ());
	}

	void OnDisable() {
		//		StopCoroutine (IncreaseDifficulty ());
	}

	IEnumerator IncreaseDifficulty() {
		yield return new WaitForSecondsRealtime (10f);
		times++;
//		rotationSpeed *= difficultyFactor;
//		movementSpeed *= difficultyFactor;
		normalGameSpeed *= difficultyFactor;
//		fastGameSpeed *= difficultyFactor;
//		Debug.Log (rotationSpeed);
		if (times < 6) {
			StartCoroutine (IncreaseDifficulty ());
		}
	}*/

}
