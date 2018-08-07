using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour {

	Transform target;
	Transform diamond;
	float speed = 5f;
	bool shouldDestroy;

	public void Init(Transform target) {
		Destroy (GetComponent<PolygonCollider2D> ());
		diamond = transform;
		this.target = target;
		StartCoroutine (Move ());
	}

	IEnumerator Move() {
		yield return new WaitForSecondsRealtime (0.016f);
		diamond.position = Vector3.MoveTowards (diamond.position, target.position, speed);
		if (shouldDestroy) {
			Shop.Instance.UpdateDiamonds (1);
			UIManager.instance.UpdateDiamondsText ();
			Destroy (diamond.gameObject);
		} else {
			if (Vector3.Distance (diamond.position, target.position) < speed) {
				shouldDestroy = true;
			}
			StartCoroutine (Move ());
		}
	}
}
