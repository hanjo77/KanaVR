using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerUp(BaseEventData eventData)
	{
		SceneManager.LoadScene ("GameScene");
	}
}
