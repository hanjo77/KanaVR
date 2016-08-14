using UnityEngine;
using UnityEngine.SceneManagement;
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
	public string tmpKanaType = "katakana";
	public WordBehaviour activeKana;
	public Kana currentKana;
	public int score = 0;
	public int startLives = 1;
	public int lives;
	public KanaTable kanaTable;
	public string soundVoice = "Kyoko";
	public float minDist = 50;
	public float randomRangeX = 200;
	public float randomRangeY = 100;
	public float randomZ = 250;
	public float backgroundVolume = .7f;
	public string wanikaniKey;

	public float activationTime;
	private List<Kana> _activeKanas = new List<Kana> ();
	private GameObject _gameHUD;
	private GameObject _pauseHUD;
	private GameObject _maleButton;
	private GameObject _femaleButton;
	private Prefs _prefs;
	private bool _isPlaying;
	private AudioSource _audioSource;
	private AudioSource _bgAudioSource;
	private Coroutine _waiter;
	private bool _isWrong;
	private string[] goodWords = {
		"sodesune",
		"subarashidesu",
		"sugoi",
		"haisodesu",
		"jouzudesu"
	};
	private string[] badWords = {
		"waruidesu",
		"hetadesu",
		"iie"
	};

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

		_pauseHUD = GameObject.FindGameObjectsWithTag ("PauseHUD") [0];
		_pauseHUD.SetActive (false);

		kanaTable = new KanaTable ();
		ShowMenu ();
		Speaker speaker = gameObject.AddComponent<Speaker> ();
		speaker.SetVoice (soundVoice);
		speaker.Say ("いらっしゃいませ");
		/* score = 60;
		_prefs.hiScore = 4567;
		ShowGameOver(); */
	}

	public void FocusKana(WordBehaviour kana) {
		activationTime = Time.time;
		activeKana = kana;
		switch (activeKana.textValue) {
		case "ひらがな":
			SpeakWord ("hiragana");
			break;
		case "カタカナ":
			SpeakWord ("katakana");
			break;
		case "かたまり":
			SpeakWord ("katamari");
			break;
		}
	}

	// Update is called once per frame
	void Update () {
		if (GvrViewer.Instance.BackButtonPressed) {
			SceneManager.LoadScene("StartScene");
		}
		if (lives < 0) {
			EndGame ();
		}
		if (
			(Input.touchCount == 2 && Input.touches[1].phase == TouchPhase.Began) ||
			(GvrViewer.Instance.VRModeEnabled && GvrViewer.Instance.Triggered && _isPlaying)
			) {
			TogglePause ();
		}
		if (activeKana && activationTime >= 0 && Time.time - activationTime >= gazeWait) {
			activationTime = -1f;
			activeKana.selected = true;
			if (activeKana.textValue == "exit") {
				_waiter = StartCoroutine (WaitForRestart ());
			} else if (activeKana.textValue == "pause") {
				TogglePause();
			} else if (activeKana.textValue == "ひらがな") {
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
				if (!_isWrong) {
					IncreaseKanaPoints ();
					score++;
				} else {
					DecreaseKanaPoints ();
					lives--;
				}
				SpeakWord (GetRandomValue(goodWords));
				GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
				_waiter = StartCoroutine (WaitForRound ());
			} else if (activeKana != null) {
				ResolveRound ();
				SpeakWord (GetRandomValue(badWords));
				_isWrong = true;
				DecreaseKanaPoints ();
				lives--;
				_waiter = StartCoroutine (WaitForRound ());
				GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);
			}
			_prefs.voice = soundVoice;
		}
	}

	private void ResolveRound() {
		foreach (Transform kana in transform) {
			WordBehaviour word = kana.GetComponent<WordBehaviour> ();
			if (word != null) {
				if (currentKana.hiragana == word.textValue || currentKana.katakana == word.textValue) {
					word.PaintObject (Color.green);
				}
				else {
					word.PaintObject (Color.red);
				}
			}
		}
	}

	private void IncreaseKanaPoints() {
		switch (tmpKanaType) {
		case "hiragana":
			currentKana.hiraganaPoints++;
			break;
		case "katakana":
			currentKana.katakanaPoints++;
			break;
		}
	}

	private void DecreaseKanaPoints() {
		switch (tmpKanaType) {
		case "hiragana":
			if (currentKana.hiraganaPoints > 0) 
				currentKana.hiraganaPoints--;
			break;
		case "katakana":
			if (currentKana.katakanaPoints > 0) 
				currentKana.katakanaPoints--;
			break;
		}
	}

	void ShowMenu() {
		Place3dText ("hanjo", Color.blue, new Vector3 (0, 0, -50), 4f, false);
		// Place3dText ("2016", Color.yellow, new Vector3 (-5, -20, -50), 1.5f, false);
		Place3dText ("かな", Color.yellow, new Vector3 (0, 25, 50), 2f, false);
		Place3dText ("vr", Color.blue, new Vector3 (0, 45, 70), 6f, false);
		Place3dText ("ひらがな", Color.blue, new Vector3(-50, 0, 50), 1f, true);
		Place3dText ("カタカナ", Color.blue, new Vector3(50, 0, 50), 1f, true);
		Place3dText ("かたまり", Color.blue, new Vector3(0, -20, 50), 1f, true);
		Place3dText ("voice", Color.yellow, new Vector3(0, 0, 50), 1f, false);
		_maleButton = Place3dText ("male", (soundVoice == "Otoya" ? Color.yellow : Color.blue), new Vector3(-10, 0, 50), 1f, true);
		_femaleButton = Place3dText ("female", (soundVoice == "Kyoko" ? Color.yellow : Color.blue), new Vector3(10, 0, 50), 1f, true);
	}

	void ShowGameOver() {
		Place3dText ("game", Color.yellow, new Vector3 (0, 47, 50), 2f, false);
		Place3dText ("over", Color.yellow, new Vector3 (0, 30, 50), 2f, false);
		if (score > _prefs.hiScore) {
			_prefs.hiScore = score;
			_prefs.Save ();
		}
		Place3dText (GetText("正：", score), Color.yellow, new Vector3 (0, 15, 50), 1f, false);
		Place3dText (GetText("高点：", _prefs.hiScore), Color.yellow, new Vector3 (0, 5, 50), 1f, false);
		Place3dText ("exit", Color.blue, new Vector3(0, -7, 50), 1f, true);
	}

	GameObject Place3dText(string text, Color color, Vector3 position, float scale, bool isButton) {
		GameObject label = PlaceKana (text, color, position);
		if (!isButton) label.GetComponent<WordBehaviour> ().isStatic = true;
		label.GetComponent<WordBehaviour> ().unMovable = true;
		label.GetComponent<WordBehaviour> ().scale = scale;
		label.transform.parent = transform;
		return label;
	}

	IEnumerator WaitForStart()
	{
		RemoveWords ();
		while (true) {
			yield return new WaitForSeconds(3.0f);
			StartGame ();
			// TogglePause ();
			StopCoroutine (_waiter);
		}
	}

	IEnumerator WaitForRestart()
	{
		RemoveWords ();
		SpeakWord ("matane");
		while (true) {
			yield return new WaitForSeconds(3.0f);
			_gameHUD.SetActive (false);
			ShowMenu ();
			StopCoroutine (_waiter);
		}
	}

	IEnumerator WaitForEnd()
	{
		RemoveWords ();
		SpeakWord ("gameover");
		while (true) {
			yield return new WaitForSeconds(3.0f);
			_gameHUD.SetActive (false);
			ShowGameOver ();
			StopCoroutine (_waiter);
		}
	}

	IEnumerator WaitForRound()
	{
		RemoveWords ();
		GameObject.Find ("RomajiText").GetComponent<TextMesh> ().text = "";
		while (true) {
			yield return new WaitForSeconds(2.0f);
			StartRound ();
			StopCoroutine (_waiter);
		}
	}

	void StartGame() {
		_prefs.Save ();
		lives = startLives;
		score = 0;
		_isPlaying = true;
		Place3dText ("pause", Color.blue, new Vector3 (0, 0, -50), 1f, true);
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
		Debug.Log ("Start round");
		_isWrong = false;
		if (kanaType == "katamari") {
			int rnd = Random.Range (0, 2);
			tmpKanaType = (rnd == 0 ? "hiragana" : "katakana");
		} else {
			tmpKanaType = kanaType;
		}
		Resources.UnloadUnusedAssets ();
		_gameHUD.SetActive (true);
		GameObject.Find ("PointsText").GetComponent<TextMesh> ().text = GetText("正：", score);
		GameObject.Find ("LivesText").GetComponent<TextMesh> ().text = GetText("生：", lives);

		currentKana = kanaTable.GetRandomKana(tmpKanaType);
		Debug.Log (currentKana.romaji);
		GameObject.Find ("RomajiText").GetComponent<TextMesh> ().text = currentKana.romaji;

		LoadKanas ();

		_audioSource.clip = Resources.Load("Sounds/" + soundVoice + "/" + currentKana.romaji) as AudioClip;
		_audioSource.PlayDelayed (.5f);
	}

	private void LoadKanas() {
		List<Kana> kanas = new List<Kana>();
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
		_audioSource.clip = Resources.Load("Sounds/" + soundVoice + "/_words/" + word) as AudioClip;
		_audioSource.Play();
	}

	private float DistanceToKanas(Vector3 pos) {
		float minDist = 100000;
		foreach (Transform child in transform) {
			if (child.GetComponent<WordBehaviour>() != null) {
				float dist = Vector3.Distance (child.GetComponent<WordBehaviour>().position, pos);
				if (dist < minDist) {
					minDist = dist;
				}
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
			DecreaseKanaPoints ();
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
		foreach (Transform kanaText in transform) {
			GameObject.Destroy(kanaText.gameObject);
		}
		_activeKanas = new List<Kana> ();
		GameObject.FindObjectOfType<GvrReticle> ().ResetReticle ();
	}

	public void TogglePause() {
		if (_isPlaying) {
			if (Time.timeScale > 0) {
				Time.timeScale = 0;
				_gameHUD.SetActive(false);
				_pauseHUD.SetActive (true);
				Debug.Log ("pause");
			} else {
				Time.timeScale = 1;
				_gameHUD.SetActive(true);
				_pauseHUD.SetActive (false);
				Debug.Log ("continue");
			}
		}
	}

	private void SetActiveByTagName(string tagName, bool active) {
		GameObject[] gameHUDs = GameObject.FindGameObjectsWithTag (tagName);
		foreach (GameObject hud in gameHUDs) {
			hud.SetActive (active);
		}
	}

	public GameObject PlaceKana(string text, Color color, Vector3 position) {
		GameObject word = new GameObject ();
		word.tag = "KanaText";
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

	private string GetRandomValue(string[] array) {
		return array [Random.Range (0, array.Length - 1)];
	}

	private string GetJapaneseNumber(int number) {
		string result = "";
		string[] numbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
		if (number >= 0 && number < 10) {
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
