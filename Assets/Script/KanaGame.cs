using UnityEngine;
using System.Collections;

public class KanaGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject word = PlaceWord("しょうじゅう", Color.blue, new Vector3(0, 3, 20), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject PlaceWord(string text, Color color, Vector3 position, Quaternion rotation) {
		GameObject word = Resources.Load("Prefabs/Word", typeof(GameObject)) as GameObject;
		WordBehaviour wordBehaviour = word.GetComponent<WordBehaviour> ();
		wordBehaviour.textValue = text;
		wordBehaviour.color = color;
		Instantiate(word, position, rotation);
		return word;
	}
}
