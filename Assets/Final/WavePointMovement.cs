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
	[SerializeField]
	float rotationOffset;
	float rotationValue;
	Vector3 initialPosition;
	float initialEuler;
	bool shouldTrackMovement;
	TrailRenderer trail;
	[SerializeField]
	bool shouldLoop;

	void Awake() {
		trail = GetComponentInChildren<TrailRenderer> ();
		initialPosition = player.position;
		initialEuler = player.rotation;
		movementInOnePhysicsFrame = speed * Time.fixedDeltaTime;
	}

	void OnEnable() {
		Init ();
		StartCoroutine (StartTracking ());
	}

	void Init() {
		trail.Clear ();
		player.MovePosition (initialPosition);
		player.MoveRotation (initialEuler);
		currentWavePoint = 0;
		SetVelocity ();
	}

	IEnumerator StartTracking() {
		yield return new WaitForFixedUpdate ();
		shouldTrackMovement = true;
	}

	void Next() {
		currentWavePoint++;
		if (currentWavePoint == wavePoints.Length) {
			if (!shouldLoop) {
				shouldTrackMovement = false;
				trail.Clear ();
				player.MovePosition (initialPosition);
				player.MoveRotation (initialEuler);
				currentWavePoint = 0;
				StartCoroutine (FalseAfterPhysicsUpdate ());
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

	void SetVelocity() {
		player.velocity = ((Vector2)wavePoints [currentWavePoint].position - player.position).normalized * speed;
		int frameCount = (int)(Vector2.Distance ((Vector2)wavePoints [currentWavePoint].position, player.position) / movementInOnePhysicsFrame) + 1;
		float eulerDifference = ((wavePoints [currentWavePoint].eulerAngles.z + rotationOffset) % 360 - player.transform.eulerAngles.z) % 360;
		rotationValue = eulerDifference / (float)frameCount;
	}

	void FixedUpdate() {
		if (shouldTrackMovement) {
			player.MoveRotation (player.rotation + rotationValue);
			if (Vector2.Distance (player.position, (Vector2)wavePoints [currentWavePoint].position) < movementInOnePhysicsFrame) {
				Next ();
			}
		}
	}

}
