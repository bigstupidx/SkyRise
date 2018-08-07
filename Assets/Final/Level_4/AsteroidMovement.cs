using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour {

	[SerializeField]
	float speed;
	[SerializeField]
	Transform direction;
	Transform obstacle;
	Rigidbody2D obstacleR;
	[SerializeField]
	MovementDirection movementDirection;
	[SerializeField]
	Vector3 rightBound;
	[SerializeField]
	Vector3 leftBound;
	TrailRenderer trail;

	void Awake() {
		trail = GetComponentInChildren<TrailRenderer> ();
		obstacle = transform;
		obstacleR = GetComponent<Rigidbody2D> ();
	}

	void OnBecameVisible() {
		obstacleR.velocity = direction.up * speed;
	}

	void FixedUpdate() {
		if (movementDirection == MovementDirection.Right) {
			if (obstacle.position.x > rightBound.x) {
				trail.Clear ();
				obstacle.localPosition = leftBound;
			}
		} else {
			if (obstacle.position.x < leftBound.x) {
				obstacle.localPosition = rightBound;
				trail.Clear ();
			}
		}
	}
}
