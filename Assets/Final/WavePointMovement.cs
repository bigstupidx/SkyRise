using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePointMovement : MonoBehaviour {

	[SerializeField]
	Transform[] wavePoints;
	int currentWavePoint;
	[SerializeField]
	Rigidbody2D player;
	[SerializeField]
	float speed;
	float movementInOnePhysicsFrame;
	float rotationValue;
	Vector3 initialPosition;
	float initialEuler;
	bool shouldTrackMovement;
	TrailRenderer trail;
	[SerializeField]
	bool shouldLoop = true;
	[SerializeField]
	int offset = 1;
	GameObject obstacle;

	void Awake() {
		obstacle = gameObject;
		trail = GetComponentInChildren<TrailRenderer> ();
		initialPosition = player.position;
		initialEuler = player.rotation;
		movementInOnePhysicsFrame = speed * Time.fixedDeltaTime;
	}

	void OnEnable() {
		Init ();
	}

	void Init() {
		shouldTrackMovement = false;
		player.velocity = Vector2.zero;
		player.angularVelocity = 0f;
		trail.Clear ();
		StartCoroutine (StartTracking ());
	}

	IEnumerator StartTracking() {
		yield return new WaitForFixedUpdate ();
		obstacle.SetActive (true);
		player.position = initialPosition;
		player.rotation = initialEuler;
		currentWavePoint = 0;
		SetVelocity (true);
		shouldTrackMovement = true;
	}

	void Next() {
		currentWavePoint++;
		if (currentWavePoint == wavePoints.Length) {
			if (!shouldLoop) {
//				shouldTrackMovement = false;
				gameObject.SetActive (false);
//				StartCoroutine (FalseAfterPhysicsUpdate ());
			} else {
				Init ();
			}
			return;
		}
		SetVelocity ();
	}

	IEnumerator FalseAfterPhysicsUpdate() {
		yield return new WaitForFixedUpdate ();
		gameObject.SetActive (false);
	}

	void SetVelocity(bool isInit = false) {
		player.velocity = ((Vector2)wavePoints [currentWavePoint].position - player.position).normalized * speed;
		if (isInit) {
			rotationValue = 0f;
		} else {
			int frameCount = (int)(Vector2.Distance ((Vector2)wavePoints [currentWavePoint].position, player.position) / movementInOnePhysicsFrame) + 1;
			float eulerDifference = PositiveRotation (wavePoints [currentWavePoint].eulerAngles.z) - PositiveRotation (wavePoints [currentWavePoint - 1].eulerAngles.z);
			eulerDifference = Acute (eulerDifference);
			rotationValue = eulerDifference / (float)frameCount;
		}
	}

	void FixedUpdate() {
		if (shouldTrackMovement) {
			player.MoveRotation (player.rotation + rotationValue);
			if (Vector2.Distance (player.position, (Vector2)wavePoints [currentWavePoint].position) < movementInOnePhysicsFrame * offset) {
				Next ();
			}
		}
	}

	float PositiveRotation(float value) {
		while (value < 0f) {
			value += 360f;
		}
		value %= 360f;
		return value;
	}

	float Acute(float value) {
		if (value > 180f) {
			value -= 360f;
		} else if (value < -180f) {
			value += 360f;
		}
		return value;
	}

}
