using UnityEngine;
public class Prefs {

	public string voice;

	public void Load()
	{       
		voice = PlayerPrefs.GetString("voice", "Otoya"); 
	}

	public void Save()
	{       
		PlayerPrefs.SetString("voice", voice); 
	}

	public void SetAll(ref GameBehaviour game)
	{       
		SetVoice(ref game);
	}
	public void SetVoice(ref GameBehaviour game)
	{       
		game.soundVoice = voice;
	} 
}