using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour {

	[SerializeField]
	ShopBall[] balls;
	Shop shop;
	[SerializeField]
	Text diamonds;
	[SerializeField]
	GameObject notEnoughDiamonds;
	[SerializeField]
	GameObject adsButton;
	public static ShopScreen instance;

	void Awake() {
		instance = this;
		adsButton.SetActive (false);
		shop = Shop.Instance;
	}

	public void ActivateAdsButton() {
		if (isActiveAndEnabled) {
			adsButton.SetActive (true);
		}
	}

	void OnEnable() {
		#if !UNITY_EDITOR
		adsButton.SetActive(SkyRiseAds.instance.IsRewarededInterLoaded ());
		#else
		adsButton.SetActive(true);
		#endif
		diamonds.text = shop.diamonds.ToString ();
		foreach (ShopBall ball in balls) {
			if (ball.index == shop.currentBall) {
				ball.selected.SetActive (true);
				ball.select.SetActive (false);
				ball.buy.SetActive (false);
			} else {
				ball.selected.SetActive (false);
				ball.select.SetActive (shop.ballStates [ball.index]);
				ball.buy.SetActive (!shop.ballStates [ball.index]);
			}
		}
	}

	public void Select(int value) {
		UnselectAll ();
		shop.SelectBall (value);
		balls [value].select.SetActive (false);
		balls [value].selected.SetActive (true);
	}

	void UnselectAll() {
		foreach (ShopBall ball in balls) {
			ball.selected.SetActive (false);
			ball.select.SetActive (shop.ballStates [ball.index]);
			ball.buy.SetActive (!shop.ballStates [ball.index]);
		}
	}

	public void Buy(int value) {
		if (shop.diamonds >= balls[value].price) {
			shop.UpdateDiamonds (-balls[value].price);
			diamonds.text = shop.diamonds.ToString ();
			shop.BuyBall (value);
			balls [value].buy.SetActive (false);
//			balls [value].select.SetActive (true);
			Select(value);
		} else {
			notEnoughDiamonds.SetActive (true);
		}
	}

	void OnDisable() {
		Shop.Instance.SaveData ();
	}

	void OnApplicationPause(bool status) {
		if (status) {
			Shop.Instance.SaveData ();
		}
	}
	public void WatchVideo() {
		#if UNITY_EDITOR
			AddDiamonds();
		#else
			adsButton.SetActive(false);
			SkyRiseAds.instance.ShowRewardedInterstitial (AddDiamonds);
		#endif
	}

	void AddDiamonds() {
		CustomAnalytics.Event ("ShopVideo");
		shop.UpdateDiamonds (2);
		diamonds.text = shop.diamonds.ToString ();
	}

}