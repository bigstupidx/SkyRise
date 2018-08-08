using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateUs : MonoBehaviour {

	public void Rate() {
		UIManager.instance.rateUsState = RateUsState.Rated;
		#if UNITY_IOS
		Application.OpenURL("https://itunes.apple.com/app/skyrise/id1406523387?mt=8");
		#elif UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.skyrise.peterdoyle1");
		#endif
	}

	public void Never() {
		UIManager.instance.rateUsState = RateUsState.Never;
	}

}
