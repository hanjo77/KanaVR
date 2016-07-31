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
		GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
		GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);
	}
	
	// Update is called once per frame
	void Update () {
		if (activeKana && activationTime >= 0 && Time.time - activationTime >= gazeWait) {
			activationTime = -1f;
			activeKana.selected = true;
			if (activeKana != null && _kanaTable.FindByKana (activeKana.textValue).romaji == currentKana.romaji) {
				activeKana.PaintObject (Color.green);
				score++;
				GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
				RemoveWords ();
				StartRound ();
			} else if (activeKana != null) {
				activeKana.PaintObject (Color.red);
				lives--;
				GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);
			}
		}
	}

	void StartRound() {
		currentKana = _kanaTable.GetRandomKana();
		GameObject.Find ("RomajiText").GetComponent<TextMesh> ().text = currentKana.romaji;

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
			 
			Vector3 target = new Vector3 (Random.Range (-50, 50), Random.Range (-50, 50), Random.Range (-50, 50)).normalized;

			string text = "";
			switch (kanaType) {
			case "hiragana":
				text = tmpKana.hiragana;
				break;
			case "katakana":
				text = tmpKana.katakana;
				break;
			}

			PlaceKana (text, Color.blue, target*startDistance);
			activeKanas.Add (tmpKana);
		}
		Debug.Log ("Play Sounds/" + currentKana.romaji); 
		AudioClip ac = Resources.Load("Sounds/"+currentKana.romaji) as AudioClip;
		AudioSource.PlayClipAtPoint(ac, Vector3.zero);
	}

	public void RemoveWord(WordBehaviour word) {
		foreach (MeshExploder exploder in word.gameObject.GetComponentsInChildren<MeshExploder>()) {
			exploder.Explode ();
		}
		GameObject.Destroy (word.gameObject);
		activeKanas.Remove (_kanaTable.FindByKana(word.textValue));
		if (activeKanas.Count <= 0) {
			lives--;
			GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);
			StartRound ();
		}
	}

	public void RemoveWords() {
		foreach (MeshExploder exploder in gameObject.GetComponentsInChildren<MeshExploder>()) {
			exploder.Explode ();
		}
		var children = new List<GameObject>();
		foreach (Transform child in transform) {
			children.Add (child.gameObject);
		}
		children.ForEach(child => Destroy(child));
		activeKanas = new List<Kana> ();
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

	private string GetText(string prefix, int number) {
		return prefix + GetJapaneseNumber(number);
	}

	private string GetJapaneseNumber(int number) {
		string result = "";
		string[] numbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
		if (number < 10) {
			return numbers [number];
		}
		else {
			if (number%10 > 0) {
				result = numbers[number%10];
			}
			number /= 10;
			if (number%10 > 0) {
				result = numbers[number%10]+"十"+result;
			}
			number /= 10;
			if (number >= 0) {
				if (number%10 > 0) {
					result = numbers[number%10]+"百"+result;
				}
				number /= 10;
				if (number >= 0) {
					if (number%10 > 0) {
						result = numbers[number%10]+"千"+result;
					}
					number /= 10;
					if (number >= 0) {
						if (number%10 > 0) {
							result = numbers[number%10]+"万"+result;
						}
					}
				}
			}
		}
		return result;
	}
}
