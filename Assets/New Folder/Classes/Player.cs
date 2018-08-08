using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {ShouldMove, ShouldRotate};

public class Player : MonoBehaviour {

	[SerializeField]
	Rigidbody2D player;
	GameState gameState;
	[SerializeField]
	GameObject asteroidWave;
	[SerializeField]
	GameObject bottomAsteroidWave;
	public static Player instance;
	GameVariables gameVariables;
	[SerializeField]
	SpriteRenderer ball;
	[SerializeField]
	TrailRenderer ballTrail;
	[SerializeField]
	Sprite[] balls;
	[SerializeField]
	Color[] trailcolors;
	[SerializeField]
	Transform diamondTarget;
	public bool isTutorialOver { get; set; }
	[SerializeField]
	Tutorial tutorial;
	bool isGameComplete;

	void Awake() {
		isTutorialOver = PlayerPrefs.GetInt ("Tutorial", 0) == 1 ? true : false;
		gameVariables = GameVariables.Instance;
		gameVariables.Init ();
		ball.sprite = balls [Shop.Instance.currentBall];
		Color startColor = trailcolors [Shop.Instance.currentBall];
		Color endColor = trailcolors [Shop.Instance.currentBall];
		startColor.a = 0.5f;
		endColor.a = 0f;
		ballTrail.startColor = startColor;
		ballTrail.endColor = endColor;
		instance = this;
	}

	public void Reset() {
		gameState = GameState.ShouldMove;
		Rotate ();
	}

	void Start() {
		if (isTutorialOver) {
			Reset ();
		}
	}

	void OnDisable() {
		Shop.Instance.SaveData ();
		gameVariables.SaveData ();
	}

	void OnApplicationPause(bool status) {
		if (status) {
			Shop.Instance.SaveData ();
			gameVariables.SaveData ();
		}
	}

	public void OnGameCompleted() {
		ballTrail.Clear ();
		isGameComplete = true;
	}

	void Update() {
		if (isGameComplete) {
			if (Input.GetMouseButtonDown (0) && !UIManager.instance.rateUsScreen.activeSelf) {
				SceneManager.LoadScene (0);
			}
		}
		else if (Time.timeScale > 0f) {
			if (Input.GetMouseButtonDown (0)) {
				if (!isTutorialOver) {
					tutorial.Next ();
					if (tutorial.state == 2) {
						Move ();
					} else if (tutorial.state == 1) {
						Rotate ();
					} else if (!isTutorialOver) {
						Stop ();
					}
				} else {
					if (gameState == GameState.ShouldMove) {
						gameState = GameState.ShouldRotate;
						Move ();
					} else {
						gameState = GameState.ShouldMove;
						Rotate ();
					}
				}
			}

			if (isTutorialOver) {
				if (gameState == GameState.ShouldMove) {
					FollowPlayer.instance.Normal ();
				} else {
//					FollowPlayer.instance.Follow ();
					FollowPlayer.instance.Fasten ();
				}
				GameManager.instance.IncrementScore ();
			} else {
				FollowPlayer.instance.Follow ();
			}
//			ChangeVelocity ();
		}
	}

	void Stop() {
		player.angularVelocity = 0f;
		player.velocity = Vector2.zero;
	}

	void Rotate () {
		player.angularVelocity = gameVariables.rotationSpeed;
		player.velocity = Vector2.zero;

//		FollowPlayer.instance.Move ();
//		player.rotation += GameVariables.instance.rotationSpeed;
	}

	void ChangeVelocity() {
//		player.velocity = GameVariables.instance.movementSpeed * new Vector2 (player.transform.up.x, player.transform.up.y);
	}

	void Move () {
		player.angularVelocity = 0f;
		player.velocity = gameVariables.movementSpeed * new Vector2 (player.transform.up.x, player.transform.up.y);
//		FollowPlayer.instance.Follow ();
//		Vector2 v = new Vector2 (player.transform.up.x, player.transform.up.y);
//		player.position += v * GameVariables.instance.movementSpeed;
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (c.CompareTag ("Obstacle") == true) {
			GameManager.instance.isGameOver = true;
			UIManager.instance.ContinueScreen ();
		} else if (c.CompareTag ("LevelExit") == true) {
			Destroy (c.GetComponent<BoxCollider2D> ());
			GameManager.instance.ChangeThemeNow ();
			gameVariables.IncreaseRotationSpeed ();
//			if(GameManager.instance.isGameOver) {
//				return;
//			}
			PatternManager.instance.SpawnNewPattern ();
		}
		else if(c.CompareTag("Score") == true) {
//			GameObject.Destroy(c.gameObject);
			c.GetComponent<Diamond>().Init(diamondTarget);
		}
		else if (c.CompareTag ("AsteroidTrigger") == true) {
			Debug.Log (asteroidWave);
			if (asteroidWave != null) {
				asteroidWave.SetActive (true);
			}
		}
		else if (c.CompareTag ("BottomAsteroidTrigger") == true) {
			if (bottomAsteroidWave != null) {
				bottomAsteroidWave.SetActive (true);
			}
		}
	}

	void OnBecameInvisible() {
		if(GameManager.instance.isGameOver || isGameComplete) {
			return;
		}
		GameManager.instance.isGameOver = true;
		UIManager.instance.ContinueScreen ();
//		SceneManager.LoadScene (0);
	}

	public void UpdateBall() {
		ball.sprite = balls [Shop.Instance.currentBall];
	}

}
