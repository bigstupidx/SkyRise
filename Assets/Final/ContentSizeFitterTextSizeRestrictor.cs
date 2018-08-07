using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ContentSizeFitter),typeof(Text))]
public class ContentSizeFitterTextSizeRestrictor : MonoBehaviour {

	Text text;
	ContentSizeFitter contentSizeFitter;
	[SerializeField]
	int textLength;

	void Awake() {
		text = GetComponent<Text> ();
		contentSizeFitter = GetComponent<ContentSizeFitter> ();
	}

	void Update() {
		contentSizeFitter.enabled = text.text.Length < textLength;
	}

}
