using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AssemblyCSharp;

public class Speaker : MonoBehaviour
{
	private KanaTable _kanaTable;
	private AudioSource[] _audioSources;
	private int activeSource = 0;
	private string _soundVoice;
	private int currentClip = 0;
	private List<AudioClip> _clips;


	void Awake ()
	{
		_kanaTable = new KanaTable ();
		_audioSources = new AudioSource[] {
			gameObject.AddComponent<AudioSource>(), 
			gameObject.AddComponent<AudioSource>() 
		};
		_clips = new List<AudioClip> ();
	}

	void Update() {
		AudioSource audioSource = _audioSources [activeSource];
		if (currentClip < _clips.Count) {
			if (!audioSource.isPlaying) {
				activeSource = (activeSource == 0 ? 1 : 0);
				audioSource = _audioSources [activeSource];
				audioSource.clip = _clips [currentClip];
				audioSource.Play ();
				currentClip++;
			}
		} else {
			if (!audioSource.isPlaying) {
				_clips = new List<AudioClip> ();
			}
		}
	}

	public void Say(string word) 
	{
		int i = 0;
		while (i < word.Length) {
			Kana kana = null;
			if (i < word.Length - 1) {
				kana = _kanaTable.FindByKana (word.Substring (i, 2));
			}
			if (kana != null) {
				_clips.Add(
					Resources.Load("Sounds/" + _soundVoice + "/" + kana.romaji) as AudioClip
				);
				i++;
			}
			else {
				kana = _kanaTable.FindByKana (word.Substring (i, 1));
				if (kana != null) {
					_clips.Add(
						Resources.Load("Sounds/" + _soundVoice + "/" + kana.romaji) as AudioClip
					);
				}
			}
			i++;
		}
	}

	public void SetVoice(String voice)
	{
		_soundVoice = voice;
	}
}

