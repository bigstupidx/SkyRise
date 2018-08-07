using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class CustomAnalytics : MonoBehaviour {

	void Start () {
		Analytics.enabled = true;		
		Analytics.deviceStatsEnabled = true;		
	}
	
	public static void Event (string value) {
		Debug.Log ("analytics: " + value);
		Analytics.CustomEvent (value);
	}
}
