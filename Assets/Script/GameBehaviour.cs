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
	public string kanaType = "katakana";
	public WordBehaviour activeKana;
	public Kana currentKana;
	public int score = 0;
	public int lives = 5;
	public KanaTable kanaTable;
	public string soundVoice = "Kyoko";
	public float minDist = 100;
	public float randomRangeX = 200;
	public float randomRangeY = 100;
	public float randomZ = 250;

	public float activationTime;
	private List<Kana> _activeKanas = new List<Kana> ();
	private GameObject _gameHUD;
	private GameObject _maleButton;
	private GameObject _femaleButton;
	private Prefs _prefs;
	private bool _isPlaying;

	// Use this for initialization
	void Start () {
		GameBehaviour b = this;
		_prefs = new Prefs ();
		_prefs.Load ();
		_prefs.SetAll(ref b);

		_gameHUD = GameObject.FindGameObjectsWithTag ("GameHUD") [0];
		_gameHUD.SetActive (false);

		kanaTable = new KanaTable ();
		ShowMenu ();
	}
	
	// Update is called once per frame
	void Update () {
		if (lives < 0) {
			EndGame ();
		}
		if (activeKana && activationTime >= 0 && Time.time - activationTime >= gazeWait) {
			activationTime = -1f;
			activeKana.selected = true;
			if (activeKana.textValue == "ひらがな") {
				kanaType = "hiragana";
				StartGame ();
			} else if (activeKana.textValue == "カタカナ") {
				kanaType = "katakana";
				StartGame ();
			} else if (activeKana.textValue == "かたまり") {
				kanaType = "katamari";
				StartGame ();
			} else if (activeKana.textValue == "male") {
				soundVoice = "Otoya";
				_femaleButton.GetComponent<WordBehaviour> ().PaintObject (Color.blue);
				_maleButton.GetComponent<WordBehaviour> ().PaintObject (Color.yellow);
			} else if (activeKana.textValue == "female") {
				soundVoice = "Kyoko";
				_femaleButton.GetComponent<WordBehaviour> ().PaintObject (Color.yellow);
				_maleButton.GetComponent<WordBehaviour> ().PaintObject (Color.blue);
			} else if (activeKana != null && kanaTable.FindByKana (activeKana.textValue).romaji == currentKana.romaji) {
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
			_prefs.voice = soundVoice;
		}
	}

	void ShowMenu() {
		GameObject title = PlaceKana ("かな", Color.yellow, new Vector3(0, 25, 50));
		title.GetComponent<WordBehaviour> ().isStatic = true;
		title.GetComponent<WordBehaviour> ().unMovable = true;
		title.GetComponent<WordBehaviour> ().scale = 2f;
		GameObject hiraganaButton = PlaceKana ("ひらがな", Color.blue, new Vector3(-50, 0, 50));
		hiraganaButton.GetComponent<WordBehaviour> ().unMovable = true;
		GameObject katakanaButton = PlaceKana ("カタカナ", Color.blue, new Vector3(50, 0, 50));
		katakanaButton.GetComponent<WordBehaviour> ().unMovable = true;
		GameObject allButton = PlaceKana ("かたまり", Color.blue, new Vector3(0, -20, 50));
		allButton.GetComponent<WordBehaviour> ().unMovable = true;

		GameObject vpiceLabel = PlaceKana ("voice", Color.yellow, new Vector3(0, 0, 50));
		vpiceLabel.GetComponent<WordBehaviour> ().isStatic = true;
		vpiceLabel.GetComponent<WordBehaviour> ().unMovable = true;
		_maleButton = PlaceKana ("male", (soundVoice == "Otoya" ? Color.yellow : Color.blue), new Vector3(-10, 0, 50));
		_maleButton.GetComponent<WordBehaviour> ().unMovable = true;
		_femaleButton = PlaceKana ("female", (soundVoice == "Kyoko" ? Color.yellow : Color.blue), new Vector3(10, 0, 50));
		_femaleButton.GetComponent<WordBehaviour> ().unMovable = true;
		// StartRound ();
	}

	void StartGame() {
		_prefs.Save ();
		lives = 5;
		score = 0;
		_isPlaying = true;
		RemoveWords ();
		StartRound ();
	}

	void EndGame() {
		if (_isPlaying) {
			MeshExploder[] exploders = GetComponentsInChildren<MeshExploder> ();
			foreach (MeshExploder exploder in exploders) {
				exploder.Explode ();
				GameObject.Destroy (exploder.gameObject);
			}
			_gameHUD.SetActive (false);
			ShowMenu ();
			_isPlaying = false;
		}
	}

	void StartRound() {
		string tmpKanaType = "";
		if (kanaType == "katamari") {
			int rnd = Random.Range (0, 2);
			tmpKanaType = (rnd == 0 ? "hiragana" : "katakana");
		} else {
			tmpKanaType = kanaType;
		}
		_gameHUD.SetActive (true);
		GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
		GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);

		currentKana = kanaTable.GetRandomKana();
		while (currentKana.romaji == "Romaji") {
			currentKana = kanaTable.GetRandomKana();
		}
		GameObject.Find ("RomajiText").GetComponent<TextMesh> ().text = currentKana.romaji;

		List<Kana> kanas = new List<Kana>();
		switch (tmpKanaType) {
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

			Vector3 target = new Vector3 (
				Random.Range (-randomRangeX, randomRangeX), 
				Random.Range (-randomRangeY, randomRangeY), 
				Random.Range (randomZ, randomZ)
			).normalized*startDistance;
			while (DistanceToKanas (target) < minDist) {
				target = new Vector3 (
					Random.Range (-randomRangeX, randomRangeX), 
					Random.Range (-randomRangeY, randomRangeY), 
					Random.Range (randomZ, randomZ)
				).normalized*startDistance;
			};

			string text = "";
			switch (tmpKanaType) {
			case "hiragana":
				text = tmpKana.hiragana;
				break;
			case "katakana":
				text = tmpKana.katakana;
				break;
			}

			PlaceKana (text, Color.blue, target);
			_activeKanas.Add (tmpKana);
		}
		AudioClip ac = Resources.Load("Sounds/" + soundVoice + "/" + currentKana.romaji) as AudioClip;
		AudioSource.PlayClipAtPoint(ac, Vector3.zero);
	}

	private float DistanceToKanas(Vector3 pos) {
		float minDist = 100000;
		foreach (Transform child in transform) {
			float dist = Vector3.Distance (child.position, pos);
			if (dist < minDist) {
				minDist = dist;
			}
		}
		return minDist;
	}

	public void RemoveWord(WordBehaviour word) {
		foreach (MeshExploder exploder in word.gameObject.GetComponentsInChildren<MeshExploder>()) {
			exploder.Explode ();
		}
		GameObject.Destroy (word.gameObject);
		_activeKanas.Remove (kanaTable.FindByKana(word.textValue));
		if (_activeKanas.Count <= 0) {
			lives--;
			GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);
			StartRound ();
		}
	}

	public void RemoveWords() {
		foreach (MeshExploder exploder in gameObject.GetComponentsInChildren<MeshExploder>()) {
			exploder.Explode ();
		}
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
		_activeKanas = new List<Kana> ();
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
