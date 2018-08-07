using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop {

	public int currentBall { get; private set; }
	public bool[] ballStates { get; private set; }
	int maxBalls = 10;
	public int diamonds { get; private set; }

	Shop() {
		diamonds = PlayerPrefs.GetInt ("Diamonds", 0);
		currentBall = PlayerPrefs.GetInt ("Ball", 5);
		string ballsString = PlayerPrefs.GetString ("Balls", "05");
		char[] balls = ballsString.ToCharArray ();
		ballStates = new bool[maxBalls];
		foreach (char ball in balls) {
			ballStates [int.Parse (ball.ToString ())] = true;
		}
	}

	private static Shop instance;

	public static Shop Instance {
		get {
			if (instance == null) {
				instance = new Shop ();
			}
			return instance;
		}
	}

	public void BuyBall(int value) {
		CustomAnalytics.Event ("BuyBall" + value);
		ballStates [value] = true;
	}

	public void SelectBall(int value) {
		currentBall = value;
	}

	public void SaveData() {
		PlayerPrefs.SetInt ("Diamonds", diamonds);
		PlayerPrefs.SetInt ("Ball", currentBall);
		string ballsString = "";
		int index = 0;
		while (index < ballStates.Length) {
			if (ballStates [index] == true) {
				ballsString += index.ToString ();
			}
			index++;
		}
		PlayerPrefs.SetString ("Balls", ballsString);
	}

	public void UpdateDiamonds(int value) {
		diamonds += value;
	}

}
