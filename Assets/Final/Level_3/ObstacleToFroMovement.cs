using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleToFroMovement : MonoBehaviour {

	[SerializeField]
	MovementDirection movementDirection;
	[SerializeField]
	float speed;
	[SerializeField]
	float rightBound = 5.25f;
	[SerializeField]
	float leftBound = -5.25f;
	Rigidbody2D obstacle;
	Transform obstacleTransform;

	void Awake() {
		obstacleTransform = transform;
		obstacle = GetComponent<Rigidbody2D> ();
		SetVelocity ();
	}

	void FixedUpdate () {
		if (movementDirection == MovementDirection.Right) {
			if (obstacle.position.x < rightBound) {
				return;
			}
		} else {
			if (obstacle.position.x > leftBound) {
				return;
			}
		}
		ToggleDirection ();
		SetVelocity ();
	}

	void ToggleDirection() {
		if (movementDirection == MovementDirection.Right) {
			movementDirection = MovementDirection.Left;
		} else {
			movementDirection = MovementDirection.Right;
		}
	}

	void SetVelocity() {
		obstacle.velocity = (movementDirection == MovementDirection.Right ? Vector2.right : Vector2.left) * speed;
	}
}
