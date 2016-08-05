using UnityEngine;
public class Prefs {

	public string voice;
	public bool isVr;

	public void Load()
	{       
		voice = PlayerPrefs.GetString("voice", "Otoya"); 
		isVr = (PlayerPrefs.GetInt ("vr", 0) > 0) ? true : false;
	}

	public void Save()
	{       
		PlayerPrefs.SetString("voice", voice); 
		PlayerPrefs.SetInt("vr", (isVr ? 1 : 0)); 
	}

	public void SetAll(ref GameBehaviour game)
	{       
		SetVoice(ref game);
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
}