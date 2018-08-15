using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportObjectManager : MonoBehaviour {

	[SerializeField]
	Camera mainCamera;
	[SerializeField]
	ViewportObject[] viewportPositions;

	void Awake() {
		foreach (ViewportObject viewportObject in viewportPositions) {
			Vector3 position = viewportObject.transform.position;
			position.x = viewportObject.viewportX;
			position.x = mainCamera.ViewportToWorldPoint (position).x;
			position.x = Mathf.Clamp (position.x, viewportObject.minBoundValue, viewportObject.maxBoundValue);
			/*if (viewportObject.bound == ViewportBound.Right) {
				if (position.x < viewportObject.boundValue) {
					position.x = viewportObject.boundValue;
				}
			}
			else if (viewportObject.bound == ViewportBound.Left) {
				if (position.x > viewportObject.boundValue) {
					position.x = viewportObject.boundValue;
				}
			}*/
			viewportObject.transform.position = position;
		}
	}

}
