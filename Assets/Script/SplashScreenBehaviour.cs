using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using System.Collections;

public class SplashScreenBehaviour : MonoBehaviour {

	private bool _adLoaded;
	private Prefs _prefs;
	[SerializeField] string iosGameId;
	[SerializeField] string androidGameId;
	[SerializeField] bool enableTestMode;

	// Use this for initialization
	void Start () {
		_prefs = new Prefs();
		_prefs.Load();
		ShowAd ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowAd()
	{
		string gameId = null;

		#if UNITY_IOS // If build platform is set to iOS...
		gameId = iosGameId;
		#elif UNITY_ANDROID // Else if build platform is set to Android...
		gameId = androidGameId;
		#endif

		if (string.IsNullOrEmpty(gameId)) { // Make sure the Game ID is set.
			Debug.LogError("Failed to initialize Unity Ads. Game ID is null or empty.");
		} else if (!Advertisement.isSupported) {
			Debug.LogWarning("Unable to initialize Unity Ads. Platform not supported.");
		} else if (Advertisement.isInitialized) {
			Debug.Log("Unity Ads is already initialized.");
		} else {
			Debug.Log(string.Format("Initialize Unity Ads using Game ID {0} with Test Mode {1}.",
				gameId, enableTestMode ? "enabled" : "disabled"));
			Advertisement.Initialize(gameId, enableTestMode);
		}
	}
		
	void OnGUI ()
	{
		if (!_adLoaded && Advertisement.IsReady ()) {
			Advertisement.Show ();
			_adLoaded = true;
		}
	}

	public void OnNoVrPointerUp(BaseEventData eventData)
	{
		_prefs.isVr = false;
		_prefs.Save ();
		SceneManager.LoadScene ("GameScene");
	}

	public void OnVrPointerUp(BaseEventData eventData)
	{
		_prefs.isVr = true;
		_prefs.Save ();
		SceneManager.LoadScene ("GameScene");
	}
}
