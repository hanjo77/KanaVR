using System;
using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class TextBehaviour : MonoBehaviour {

	public string textValue = "ハニョ";
	public float padding = 1f;
	public Material material;

	private float hue = 0;

	public KanaTable kanaTable = new KanaTable ();

	// Use this for initialization
	void Start () {
		WriteText(textValue);
	}
	
	// Update is called once per frame
	void Update () {
		var renderers = GetComponentsInChildren<Renderer> ();
		foreach (var renderer in renderers) {
			hue += .001f;
			if (hue > 1) {
				hue = 0;
			}
			renderer.material.color = Color.HSVToRGB(hue, .5f, .5f);
		}
	}

	void WriteText(string content) {
		var cursor = 0f;
		var height = 0f;
		for (var i = 0; i < content.Length; i++) {
			Kana kana = null;
			if (i < content.Length - 1) {
				kana = kanaTable.FindByKana (content.Substring (i, 2));
			}
			if (kana == null) {
				kana = kanaTable.FindByKana (content.Substring (i, 1));
			}
			else {
				i++;
			}
			if (kana != null) {
				string folder = "hiragana/";
				if (content.Substring (i, 1) == kana.katakana.Substring (kana.katakana.Length - 1, 1)) {
					folder = "katakana/";
				}
				// Debug.Log (folder+kana.romaji);
				GameObject go = Instantiate (Resources.Load (folder+kana.romaji, typeof(GameObject)) as GameObject);
				if (height <= 0) {
					height = (go.GetComponentInChildren<Renderer> ()).bounds.size.y;
				}
				if (material) go.GetComponentInChildren<Renderer> ().material = this.material;
				go.transform.parent = transform;
				go.transform.Rotate (new Vector3 (90, 0, 180));
				go.transform.localPosition = new Vector3 (cursor, (go.GetComponentInChildren<Renderer> ()).bounds.size.y - height, 0);
				cursor += (go.GetComponentInChildren<Renderer> ()).bounds.size.x + padding;
			}
		}
	}

}
