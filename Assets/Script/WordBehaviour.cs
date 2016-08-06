using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using AssemblyCSharp;

public class WordBehaviour : MonoBehaviour  {
	public string textValue = "ひょう";
	public float padding = 1f;
	public float scale = 1f;
	public Vector3 position;
	public Vector3 rotation;
	public Material material;
	public Color color = Color.blue;
	public float speed;
	public bool selected;
	public bool isStatic;
	public bool unMovable;

	private GameObject _word;
	private Vector3 _headPos;
	private KanaTable _kanaTable;
	private NumberTable _numberTable;
	private GameBehaviour _gameController;

	// Use this for initialization
	void Start () {

		_gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameBehaviour>();
		_kanaTable = _gameController.kanaTable;
		_numberTable = new NumberTable();

		WriteText(textValue);
		_word = gameObject;
		_word.tag = "KanaText";

		AddBoxCollider (_word);
		_word.transform.parent = _gameController.transform;
		_word.transform.position = position;

		Transform vrHead = GvrViewer.Instance.gameObject.transform;
		Vector3 colliderSize = _word.GetComponent<BoxCollider> ().size;
		_headPos = vrHead.GetComponentInChildren<Camera>().transform.position;
		// Debug.DrawLine (_headPos, position);
		Quaternion rotation = Quaternion.LookRotation(_headPos - position);
		_word.transform.rotation = rotation;
		_word.transform.Rotate (new Vector3(180, 0, 180));

		PaintObject (color);

		if (!isStatic) {
			EventTrigger trigger = _word.AddComponent<EventTrigger>( );

			EventTrigger.Entry entryEnter = new EventTrigger.Entry();
			entryEnter.eventID = EventTriggerType.PointerEnter;
			entryEnter.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callEnter = new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerEnter);
			entryEnter.callback.AddListener(callEnter);
			trigger.triggers.Add(entryEnter);

			EventTrigger.Entry entryExit = new EventTrigger.Entry();
			entryExit.eventID = EventTriggerType.PointerExit;
			entryExit.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callExit = new UnityEngine.Events.UnityAction<BaseEventData>(OnPointerExit);
			entryExit.callback.AddListener(callExit);
			trigger.triggers.Add(entryExit);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!unMovable) {
			_word.transform.position = Vector3.MoveTowards (
				_word.transform.position, 
				_headPos, 
				(speed + (_gameController.score / 10f)) * Time.deltaTime);
			if (Vector3.Distance (_headPos, _word.transform.position) < 1f) {
				transform.parent.gameObject.GetComponent<GameBehaviour>().RemoveWord (this);
			}
		}
	}

	GameObject WriteText(string content) {
		var cursor = 0f;
		var height = 0f;
		GameObject go = new GameObject();
		go.transform.parent = _gameController.transform;
		go.tag = "KanaText";
		string[] words = content.Split (':');
		if (words.Length > 1) {
			GameObject[] objects = {
				WriteText (words [0]),
				WriteText ("dp"),
				WriteNumber (words [1])
			};
			height = -1;
			foreach (GameObject obj in objects) {
				obj.transform.parent = _gameController.transform;
				Renderer rend = obj.GetComponentInChildren<Renderer> ();
				if (height <= 0) {
					height = rend.bounds.size.z;
				}
				float posY = rend.bounds.size.z - height;
				if (material)
					rend.material = this.material;
				rend.material.SetColor ("_SpecColor", color);
				go.transform.parent = go.transform;
				go.transform.Rotate (new Vector3 (90, 0, 180));
				go.transform.localPosition = new Vector3 (cursor, posY, 0);
				go.transform.localScale = new Vector3 (scale, scale, scale);
				cursor += (rend.bounds.size.x + padding) * scale;
			}
		} else {
			go = Resources.Load ("hiragana/" + content, typeof(GameObject)) as GameObject;
			if (go == null) {
				for (var i = 0; i < content.Length; i++) {
					Kana kana = null;
					if (i < content.Length - 1) {
						kana = _kanaTable.FindByKana (content.Substring (i, 2));
					}
					if (kana == null) {
						kana = _kanaTable.FindByKana (content.Substring (i, 1));
					} else {
						i++;
					}
					if (kana != null) {
						string folder = "hiragana/";
						if (content.Substring (i, 1) == kana.katakana.Substring (kana.katakana.Length - 1, 1)) {
							folder = "katakana/";
						}
						if (i > 0) { 
							cursor += (scale * padding);
						}
						// Debug.Log (folder+kana.romaji);
						go = Instantiate (Resources.Load (folder + kana.romaji, typeof(GameObject)) as GameObject);
						Renderer rend = go.GetComponentInChildren<Renderer> ();
						if (height <= 0) {
							height = rend.bounds.size.z;
						}
						float posY = rend.bounds.size.z - height;
						if (kana.romaji == "_") {
							posY += ((height - rend.bounds.size.z) / 2);
						}
						if (material)
							rend.material = this.material;
						rend.material.SetColor ("_SpecColor", color);
						go.transform.parent = transform;
						go.transform.Rotate (new Vector3 (90, 0, 180));
						go.transform.localPosition = new Vector3 (cursor, posY, 0);
						go.transform.localScale = new Vector3 (scale, scale, scale);
						cursor += rend.bounds.size.x;
					}
				}
				foreach (Transform t in transform) {
					Vector3 pos = new Vector3 (
						t.localPosition.x - (cursor / 2), 
						t.localPosition.y, 
						t.localPosition.z);
					t.localPosition = pos;
				}
			} else {
				go = Instantiate (go);
				Renderer rend = go.GetComponentInChildren<Renderer> ();
				rend.material.SetColor ("_SpecColor", color);
				go.transform.parent = transform;
				go.transform.Rotate (new Vector3 (90, 0, 180));
				go.transform.position = new Vector3 ((rend.bounds.size.x*scale)/-2, 0, 0);
				go.transform.localScale = new Vector3 (scale, scale, scale);
			}
		}
		return go;
	}

	GameObject WriteNumber(string content) {
		var cursor = 0f;
		var height = 0f;
		GameObject go = new GameObject();
		go.tag = "KanaText";
		go.transform.parent = _gameController.transform;
		for (var i = 0; i < content.Length; i++) {
			Kana kana = _numberTable.FindByKana (content.Substring (i, 1));
			if (kana != null) {
				string folder = "numbers/";
				if (i > 0) { 
					cursor += (scale * padding);
				}
				Debug.Log (folder+kana.romaji);
				go = Instantiate (Resources.Load (folder + kana.romaji, typeof(GameObject)) as GameObject);
				Renderer rend = go.GetComponentInChildren<Renderer> ();
				if (height <= 0) {
					height = rend.bounds.size.z;
				}
				float posY = rend.bounds.size.z - height;
				if (kana.romaji == "_") {
					posY += ((height - rend.bounds.size.z) / 2);
				}
				if (material)
					rend.material = this.material;
				rend.material.SetColor ("_SpecColor", color);
				go.transform.parent = transform;
				go.transform.Rotate (new Vector3 (90, 0, 180));
				go.transform.localPosition = new Vector3 (cursor, posY, 0);
				go.transform.localScale = new Vector3 (scale, scale, scale);
				cursor += rend.bounds.size.x;
			}
		}
		foreach (Transform t in transform) {
			Vector3 pos = new Vector3 (
				t.localPosition.x - (cursor / 2), 
				t.localPosition.y, 
				t.localPosition.z);
			t.localPosition = pos;
		}
		return go;
	}

	void AddBoxCollider (GameObject wordObject) {
		BoxCollider collider = wordObject.AddComponent<BoxCollider> ();
		bool hasBounds = false;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		foreach (Renderer childRenderer in wordObject.GetComponentsInChildren<Renderer>()) {
			if (hasBounds) {
				bounds.Encapsulate(childRenderer.bounds);
			}
			else {
				bounds = childRenderer.bounds;
				hasBounds = true;
			}
			childRenderer.gameObject.AddComponent<MeshExploder> ();
		}
		collider.center = bounds.center - wordObject.transform.position;
		collider.size = bounds.size;
	}

	public void OnPointerEnter(BaseEventData eventData)
	{
		GameObject selection = ((PointerEventData)eventData).pointerCurrentRaycast.gameObject;
		GameBehaviour game = transform.parent.gameObject.GetComponent<GameBehaviour> ();
		bool isActiveVoice = (
			(game.soundVoice == "Otoya" && textValue == "male") ||
			(game.soundVoice == "Kyoko" && textValue == "female")
		);
		if (!isActiveVoice) {
			PaintObject (Color.cyan);
		}
		eventData.selectedObject = selection;
		game.FocusKana (this);
	}

	public void OnPointerExit(BaseEventData eventData)
	{    
		GameObject selection = ((PointerEventData)eventData).selectedObject;
		GameBehaviour game = transform.parent.gameObject.GetComponent<GameBehaviour> ();
		bool isActiveVoice = (
			(game.soundVoice == "Otoya" && textValue == "male") ||
			(game.soundVoice == "Kyoko" && textValue == "female")
		);
		if (!selected || isActiveVoice) {
			PaintObject (isActiveVoice ? Color.yellow : Color.blue);
		}
		eventData.selectedObject = null;
		game.activationTime = -1f;
		game.activeKana = null;
	}

	public void PaintObject(Color col) {
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			renderer.material.color = col;
		}
	}
}
