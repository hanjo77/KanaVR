using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class GameBehaviour : MonoBehaviour {

	public float startDistance = 100f;
	public float speed = 10f;
	public float gazeWait = 2f;
	public int maxSelection = 4;
	public string kanaType = "hiragana";
	public WordBehaviour activeKana;
	public Kana currentKana;
	public int score = 0;
	public int lives = 5;

	public float activationTime;
	private KanaTable _kanaTable;
	private List<Kana> activeKanas = new List<Kana> ();

	// Use this for initialization
	void Start () {
		_kanaTable = new KanaTable ();
		StartRound ();
	}
	
	// Update is called once per frame
	void Update () {
		if (activeKana && activationTime >= 0 && Time.time - activationTime >= gazeWait) {
			activationTime = -1f;
			activeKana.selected = true;
			if (activeKana != null && _kanaTable.FindByKana (activeKana.textValue).romaji == currentKana.romaji) {
				activeKana.PaintObject (Color.green);
				score++;
				RemoveWords ();
				StartRound ();
			} else if (activeKana != null) {
				activeKana.PaintObject (Color.red);
				lives--;
			}
		}
	}

	void StartRound() {
		currentKana = _kanaTable.GetRandomKana();
		List<Kana> kanas = new List<Kana>();
		switch (kanaType) {
		case "hiragana":
			kanas = currentKana.hiraganaLikes;
			break;
		case "katakana":
			kanas = currentKana.katakanaLikes;
			break;
		}
		if (kanas.Count >= maxSelection) {
			kanas = kanas.GetRange (0, maxSelection - 1);
		}
		kanas.Add (currentKana);
		foreach (Kana tmpKana in kanas) {

			float posX = Random.Range (-50, 50);
			float posY = Random.Range (-50, 50);

			string text = "";
			switch (kanaType) {
			case "hiragana":
				text = tmpKana.hiragana;
				break;
			case "katakana":
				text = tmpKana.katakana;
				break;
			}

			PlaceKana (text, Color.blue, new Vector3 (posX, posY, startDistance));
			activeKanas.Add (tmpKana);
		}
		AudioClip ac = Resources.Load("Sounds/"+currentKana.romaji) as AudioClip;
		AudioSource.PlayClipAtPoint(ac, Vector3.zero);
	}

	public void RemoveWord(WordBehaviour word) {
		GameObject.Destroy (word.gameObject);
		activeKanas.Remove (_kanaTable.FindByKana(word.textValue));
		if (activeKanas.Count <= 0) {
			StartRound ();
		}
	}

	public void RemoveWords() {
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));
	}

	public GameObject PlaceKana(string text, Color color, Vector3 position) {
		GameObject word = new GameObject ();
		word.transform.parent = transform;
		WordBehaviour wordBehaviour = word.AddComponent<WordBehaviour> ();
		wordBehaviour.textValue = text;
		wordBehaviour.color = color;
		wordBehaviour.position = position;
		wordBehaviour.speed = speed;
		return word;
	}
}
