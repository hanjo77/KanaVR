using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class KanaTable
	{
		List<Kana> kana = new List<Kana>();

		public KanaTable ()
		{
			string fileData = System.IO.File.ReadAllText("Assets/Script/kana.csv");
			string[] lines = fileData.Split('\n');
			foreach (string line in lines) 
			{
				string[] lineData = (line.Trim()).Split(';');
				if (lineData.Length >= 3) 
				{
					kana.Add (new Kana (lineData [0], lineData [1], lineData [2]));
				}
			}
		}

		public Kana FindByKana(string kanaChar)
		{
			return kana.Find (x => (x.hiragana == kanaChar || x.katakana == kanaChar));
		}

		public Kana FindByRomaji(string romaji)
		{
			return kana.Find (x => x.romaji == romaji);
		}

		public Kana FindByHiragana(string hiragana)
		{
			return kana.Find (x => x.hiragana == hiragana);
		}

		public Kana FindByKatakana(string katakana)
		{
			return kana.Find (x => x.katakana == katakana);
		}
	}

	public class Kana
	{
		public string romaji { get; set; }
		public string hiragana { get; set; }
		public string katakana { get; set; }

		public Kana(string romaji, string hiragana, string katakana)
		{
			this.romaji = romaji;
			this.hiragana = hiragana;
			this.katakana = katakana;
		}
	}
}
	