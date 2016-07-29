using System;
using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class WordBehaviour : MonoBehaviour {

	public string textValue = "ハニョ";
	public float padding = 1f;
	public Material material;
	public Color color = new Color ();

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
		BoxCollider boxCollider = GetComponent<BoxCollider>();
		FitToChildren (boxCollider);
	}

	void FitToChildren(BoxCollider collider) {
		bool hasBounds = false;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		foreach (Renderer childRenderer in GetComponentsInChildren<Renderer>()) {
			if (hasBounds) {
				bounds.Encapsulate(childRenderer.bounds);
			}
			else {
				bounds = childRenderer.bounds;
				hasBounds = true;
			}
		}
		collider.center = bounds.center - transform.position;
		collider.size = bounds.size;
	}
}
