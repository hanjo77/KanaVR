using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using AssemblyCSharp;

public class TitleBehaviour : MonoBehaviour  {
	public string textValue = "ひょう";
	public float padding = 1f;
	public Vector3 position;
	public Vector3 rotation;
	public Material material;
	public Color color = Color.blue;

	private Vector3 _headPos;

	public KanaTable kanaTable = new KanaTable ();

	// Use this for initialization
	void Start () {
		WriteText(textValue);

		PaintObject (Color.blue);
	}

	// Update is called once per frame
	void Update () {
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
				Renderer rend = go.GetComponentInChildren<Renderer> ();
				if (height <= 0) {
					height = rend.bounds.size.z;
				}
				float posY = rend.bounds.size.z - height;
				if (kana.romaji == "_") {
					posY += ((height - rend.bounds.size.z) / 2);
				}
				if (material) rend.material = this.material;
				rend.material.SetColor("_SpecColor", color);
				go.transform.parent = transform;
				go.transform.Rotate (new Vector3 (90, 0, 180));
				go.transform.localPosition = new Vector3 (cursor, posY, 0);
				cursor += rend.bounds.size.x + padding;
			}
		}
	}

	public void PaintObject(Color col) {
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.material.color = col;
		}
	}
}
