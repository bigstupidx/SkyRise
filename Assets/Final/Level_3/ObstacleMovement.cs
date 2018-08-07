using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MovementDirection {Right, Left};

public class ObstacleMovement : MonoBehaviour {

	[SerializeField]
	MovementDirection movementDirection;
	[SerializeField]
	float speed;
	float rightBound = 5.25f;
	float leftBound = -5.25f;
	Rigidbody2D obstacle;
	Transform obstacleTransform;
	Vector3 leftScale = new Vector3 (-1f, 1f, 1f);
	Vector3 rightScale = Vector3.one;

	void Awake() {
		obstacleTransform = transform;
		obstacle = GetComponent<Rigidbody2D> ();
		SetVelocity ();
	}

	void FixedUpdate () {
		if (movementDirection == MovementDirection.Right) {
			if (obstacle.position.x > rightBound) {
				Vector3 temp = obstacle.position;
				temp.x = leftBound;
				obstacle.MovePosition (temp);
			}
		} else {
			if (obstacle.position.x < leftBound) {
				Vector3 temp = obstacle.position;
				temp.x = rightBound;
				obstacle.MovePosition (temp);
			}
		}

//		if (movementDirection == MovementDirection.Right) {
//			if (obstacle.position.x < rightBound) {
//				return;
//			}
//		} else {
//			if (obstacle.position.x > leftBound) {
//				return;
//			}
//		}
//		ToggleDirection ();
//		SetVelocity ();
	}

//	void ToggleDirection() {
//		if (movementDirection == MovementDirection.Right) {
//			obstacleTransform.localScale = leftScale;
//			movementDirection = MovementDirection.Left;
//		} else {
//			obstacleTransform.localScale = rightScale;
//			movementDirection = MovementDirection.Right;
//		}
//	}

	void SetVelocity() {
		obstacle.velocity = (movementDirection == MovementDirection.Right ? Vector2.right : Vector2.left) * speed;
	}
}
