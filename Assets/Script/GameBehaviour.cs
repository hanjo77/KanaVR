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
	public float backgroundVolume = .7f;

	public float activationTime;
	private List<Kana> _activeKanas = new List<Kana> ();
	private GameObject _gameHUD;
	private GameObject _maleButton;
	private GameObject _femaleButton;
	private Prefs _prefs;
	private bool _isPlaying;
	private AudioSource _audioSource;
	private AudioSource _bgAudioSource;
	private Coroutine _waiter;

	// Use this for initialization
	void Start () {
		_audioSource = gameObject.AddComponent<AudioSource> ();
		_bgAudioSource = gameObject.AddComponent<AudioSource> ();
		_bgAudioSource.clip = Resources.Load("Sounds/soundtrack") as AudioClip;
		_bgAudioSource.loop = true;
		_bgAudioSource.volume = backgroundVolume;
		_bgAudioSource.Play ();
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
				_waiter = StartCoroutine (WaitForStart ());
			} else if (activeKana.textValue == "カタカナ") {
				kanaType = "katakana";
				_waiter = StartCoroutine (WaitForStart ());
			} else if (activeKana.textValue == "かたまり") {
				kanaType = "katamari";
				_waiter = StartCoroutine (WaitForStart ());
			} else if (activeKana.textValue == "male") {
				soundVoice = "Otoya";
				SpeakWord ("konnichiwa");
				_femaleButton.GetComponent<WordBehaviour> ().PaintObject (Color.blue);
				_maleButton.GetComponent<WordBehaviour> ().PaintObject (Color.yellow);
			} else if (activeKana.textValue == "female") {
				soundVoice = "Kyoko";
				SpeakWord ("konnichiwa");
				_femaleButton.GetComponent<WordBehaviour> ().PaintObject (Color.yellow);
				_maleButton.GetComponent<WordBehaviour> ().PaintObject (Color.blue);
			} else if (activeKana != null && kanaTable.FindByKana (activeKana.textValue).romaji == currentKana.romaji) {
				activeKana.PaintObject (Color.green);
				score++;
				SpeakWord ("sodesune");
				GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
				RemoveWords ();
				_waiter = StartCoroutine (WaitForRound ());
			} else if (activeKana != null) {
				activeKana.PaintObject (Color.red);
				SpeakWord ("warui");
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

	IEnumerator WaitForStart()
	{
		SpeakWord (kanaType);
		RemoveWords ();
		while (true) {
			yield return new WaitForSeconds(3.0f);
			StartGame ();
			StopCoroutine (_waiter);
		}
	}

	IEnumerator WaitForEnd()
	{
		while (true) {
			yield return new WaitForSeconds(3.0f);
			_gameHUD.SetActive (false);
			SpeakWord ("gomennasai");
			ShowMenu ();
			StopCoroutine (_waiter);
		}
	}

	IEnumerator WaitForRound()
	{
		GameObject.Find ("RomajiText").GetComponent<TextMesh> ().text = "";
		while (true) {
			yield return new WaitForSeconds(2.0f);
			StartRound ();
			StopCoroutine (_waiter);
		}
	}

	void StartGame() {
		_prefs.Save ();
		lives = 5;
		score = 0;
		_isPlaying = true;
		StartRound ();
	}

	void EndGame() {
		if (_isPlaying) {
			_isPlaying = false;
			MeshExploder[] exploders = GetComponentsInChildren<MeshExploder> ();
			foreach (MeshExploder exploder in exploders) {
				exploder.Explode ();
				GameObject.Destroy (exploder.gameObject);
			}
			StopAllCoroutines ();
			_waiter = StartCoroutine (WaitForEnd ());
		}
	}

	void StartRound() {
		Resources.UnloadUnusedAssets ();
		_gameHUD.SetActive (true);
		GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
		GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);

		currentKana = kanaTable.GetRandomKana();
		while (currentKana.romaji == "Romaji") {
			currentKana = kanaTable.GetRandomKana();
		}
		GameObject.Find ("RomajiText").GetComponent<TextMesh> ().text = currentKana.romaji;

		LoadKanas ();

		_audioSource.clip = Resources.Load("Sounds/" + soundVoice + "/" + currentKana.romaji) as AudioClip;
		_audioSource.PlayDelayed (.5f);
	}

	private void LoadKanas() {
		List<Kana> kanas = new List<Kana>();
		string tmpKanaType = "";
		if (kanaType == "katamari") {
			int rnd = Random.Range (0, 2);
			tmpKanaType = (rnd == 0 ? "hiragana" : "katakana");
		} else {
			tmpKanaType = kanaType;
		}
		switch (tmpKanaType) {
		case "hiragana":
			kanas = RandomizeAndCrop(currentKana.hiraganaLikes, maxSelection - 1);
			break;
		case "katakana":
			kanas = RandomizeAndCrop(currentKana.katakanaLikes, maxSelection - 1);
			break;
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
	}

	private List<Kana> RandomizeAndCrop (List<Kana> list, int size) {
		if (list.Count >= maxSelection) {
			List<Kana> used = new List<Kana> ();
			while (used.Count < size) {
				Kana tmp = list[Random.Range(0, list.Count-1)];
				list.Remove (tmp);
				used.Add (tmp);
			}
			list = used;
		}
		return list;
	}

	private void SpeakWord(string word) {
		AudioClip ac = Resources.Load("Sounds/" + soundVoice + "/_words/" + word) as AudioClip;
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
		if (_activeKanas.Count <= 0 && _isPlaying) {
			lives--;
			SpeakWord ("itaidesu");
			if (GameObject.Find ("LivesText") != null) {
				GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);
			}
			_waiter = StartCoroutine(WaitForRound());
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
