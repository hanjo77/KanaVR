using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using AssemblyCSharp;

public class WordBehaviour : MonoBehaviour  {
	public string kanaType = "hiragana";
	public string textValue = "ひょう";
	public float padding = 1f;
	public Vector3 position;
	public Vector3 rotation;
	public Material material;
	public Color color = Color.blue;
	public float speed;
	public bool selected;

	private GameObject _word;
	private Vector3 _headPos;

	public KanaTable kanaTable = new KanaTable ();

	// Use this for initialization
	void Start () {
		// _word = Instantiate (Resources.Load (kanaType+"/"+kana, typeof(GameObject)) as GameObject);
		WriteText(textValue);
		_word = gameObject;

		AddBoxCollider (_word);
		position.x -= ((_word.GetComponent<BoxCollider> ()).size.x / 2);
		_word.transform.position = position;

		Transform vrHead = GvrViewer.Instance.gameObject.transform;
		Vector3 colliderSize = _word.GetComponent<BoxCollider> ().size;
		_headPos = new Vector3 (colliderSize.x / -2, vrHead.position.y + (colliderSize.z / 2), 0);

		Quaternion rotation = Quaternion.LookRotation(vrHead.position - position);
		_word.transform.rotation = rotation;
		_word.transform.Rotate (new Vector3(180, 0, 180));

		PaintObject (Color.blue);

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
	
	// Update is called once per frame
	void Update () {
		_word.transform.position = Vector3.MoveTowards (
			_word.transform.position, 
			_headPos, 
			speed * Time.deltaTime);
		if (Vector3.Distance (_headPos, _word.transform.position) < .1f) {
			transform.parent.gameObject.GetComponent<GameBehaviour>().RemoveWord (this);
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
//		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
//		FitToChildren (boxCollider);
//		Teleport teleport = gameObject.AddComponent<Teleport> ();
//		EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger> ();
// 		eventTrigger.OnPointerEnter = teleport.OnGazeEnter;
//		eventTrigger.OnPointerExit = teleport.OnGazeExit;
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
		}
		collider.center = bounds.center - wordObject.transform.position;
		collider.size = bounds.size;
	}

	public void OnPointerEnter(BaseEventData eventData)
	{  
		GameObject selection = ((PointerEventData)eventData).pointerCurrentRaycast.gameObject;
		PaintObject (Color.cyan);
		eventData.selectedObject = selection;
		GameBehaviour game = transform.parent.gameObject.GetComponent<GameBehaviour> ();
		game.activationTime = Time.time;
		game.activeKana = this;
	}

	public void OnPointerExit(BaseEventData eventData)
	{    
		GameObject selection = ((PointerEventData)eventData).selectedObject;
		if (!selected) {
			PaintObject (Color.blue);
		}
		eventData.selectedObject = null;
		GameBehaviour game = transform.parent.gameObject.GetComponent<GameBehaviour> ();
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
