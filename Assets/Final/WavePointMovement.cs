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
	[SerializeField]
	int offset = 1;
	bool shouldDebug;

	void Awake() {
		trail = GetComponentInChildren<TrailRenderer> ();
		initialPosition = player.position;
		initialEuler = player.rotation;
		movementInOnePhysicsFrame = speed * Time.fixedDeltaTime;
	}

	void OnEnable() {
		Init ();
	}

	void Init() {
		player.velocity = Vector2.zero;
		player.angularVelocity = 0f;
		if(shouldDebug)
		Debug.Log ("init");
		trail.Clear ();
		StartCoroutine (StartTracking ());
	}

	IEnumerator StartTracking() {
		yield return new WaitForFixedUpdate ();
		player.MovePosition (initialPosition);
		player.MoveRotation (initialEuler);
		currentWavePoint = 0;
		SetVelocity (true);
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
				shouldTrackMovement = false;
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
		if (isInit && shouldDebug) {
			Debug.Log (player.velocity);
			Debug.Log (wavePoints [currentWavePoint].position);
			Debug.Log (player.position);
		}
		if (isInit) {
			rotationValue = 0f;
		} else {
			int frameCount = (int)(Vector2.Distance ((Vector2)wavePoints [currentWavePoint].position, player.position) / movementInOnePhysicsFrame) + 1;
			float eulerDifference = ((wavePoints [currentWavePoint].eulerAngles.z + rotationOffset) % 360 - player.transform.eulerAngles.z) % 360;
			rotationValue = eulerDifference / (float)frameCount;
		}
		if (shouldDebug) {
			Debug.Log (player.velocity);
			Debug.Log (rotationValue);
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

}
