using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class NumberTable
	{
		List<Kana> kanas = new List<Kana>();

		public NumberTable ()
		{
			TextAsset txt = (TextAsset)Resources.Load("numbers", typeof(TextAsset));
			string fileData = txt.text;
			string[] lines = fileData.Split('\n');
			foreach (string line in lines) 
			{
				string[] lineData = (line.Trim()).Split(';');
				if (lineData.Length >= 3 && lineData[0].ToLower() != "hiragana") 
				{
					Kana kana = new Kana (lineData [0], lineData [1], lineData [2]);
					kanas.Add (kana);
				}
			}
		}

		public void Start()
		{
		}

		public Kana FindByKana(string kanaChar)
		{
			return kanas.Find (x => (x.hiragana == kanaChar || x.katakana == kanaChar));
		}

		public Kana FindByRomaji(string romaji)
		{
			return kanas.Find (x => x.romaji == romaji);
		}

		public Kana FindByHiragana(string hiragana)
		{
			return kanas.Find (x => x.hiragana == hiragana);
		}

		public Kana FindByKatakana(string katakana)
		{
			return kanas.Find (x => x.katakana == katakana);
		}
	}
}
	