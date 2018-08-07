using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	[SerializeField]
	Transform mainCamera;
	[SerializeField]
	Transform player;
	Rigidbody2D playerR;
	float gap;
	public static FollowPlayer instance;

	void Awake() {
		instance = this;
		playerR = player.GetComponent<Rigidbody2D> ();
	}

	public void Follow() {
		if (playerR.velocity.y > 0f) {
			Vector3 pos = player.transform.position + (Vector3.up * 2.5f) + Vector3.back;
			pos.x = 0f;
			mainCamera.transform.position = pos;
		}
	}

	public void Normal() {
		mainCamera.Translate (0, GameVariables.Instance.normalGameSpeed, 0);
//		Vector3 pos = mainCamera.position;
//		pos.y += GameVariables.instance.normalGameSpeed;
//		mainCamera.position = pos;
	}

	public void Fasten() {
//		mainCamera.transform.position = player.transform.position + (Vector3.up * 2.5f) + Vector3.back;
//		Vector3 pos = mainCamera.position;
		if (player.position.y > mainCamera.position.y + 1f) {
			Vector3 pos = player.transform.position - Vector3.up + Vector3.back;
			pos.x = 0f;
			mainCamera.transform.position = pos;
//			mainCamera.Translate (0, GameVariables.Instance.fastGameSpeed, 0);
//			pos.y += GameVariables.instance.fastGameSpeed;
		} else {
			mainCamera.Translate (0, GameVariables.Instance.normalGameSpeed, 0);
//			pos.y += GameVariables.instance.normalGameSpeed;
		}
//		mainCamera.position = pos;
	}
}
