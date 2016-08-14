using UnityEngine;
public class Prefs {

	public string voice;
	public bool isVr;
	public int hiScore;
	public string wanikaniKey;

	public void Load()
	{       
		voice = PlayerPrefs.GetString("voice", "Otoya"); 
		isVr = (PlayerPrefs.GetInt ("vr", 0) > 0) ? true : false;
		hiScore = PlayerPrefs.GetInt ("hiscore", 5);
		wanikaniKey = PlayerPrefs.GetString ("wanikaniKey", "");
	}

	public void Save()
	{       
		PlayerPrefs.SetString("voice", voice); 
		PlayerPrefs.SetInt("vr", (isVr ? 1 : 0)); 
		PlayerPrefs.SetInt("hiscore", hiScore); 
		PlayerPrefs.SetString("wanikaniKey", wanikaniKey); 
	}

	public void SetAll(ref GameBehaviour game)
	{       
		SetVoice(ref game);
		SetWanikaniKey(ref game);
		SetVr (ref game);
	}
	public void SetVoice(ref GameBehaviour game)
	{       
		game.soundVoice = voice;
	} 
	public void SetVr(ref GameBehaviour game)
	{       
		GvrViewer viewer = GameObject.FindObjectOfType<GvrViewer> ();
		if (viewer != null) {
			viewer.VRModeEnabled = isVr;
			viewer.GetComponentInChildren<GvrHead> ().trackRotation = isVr;
		}
	} 
	public void SetWanikaniKey(ref GameBehaviour game)
	{       
		game.wanikaniKey = wanikaniKey;
	} 
}