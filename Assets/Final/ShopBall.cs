using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBall : MonoBehaviour {

	public int index;
	public GameObject selected;
	public GameObject select;
	public GameObject buy;
	public int price;
	[SerializeField]
	Text priceText;

	void Awake() {
		priceText.text = price.ToString ();
	}

}