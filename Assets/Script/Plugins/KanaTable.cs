using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class KanaTable
	{
		List<Kana> kanas = new List<Kana>();

		public KanaTable ()
		{
			TextAsset txt = (TextAsset)Resources.Load("kanadata", typeof(TextAsset));
			string fileData = txt.text;
			TextAsset txtLikes = (TextAsset)Resources.Load("kanalikes", typeof(TextAsset));
			string fileLikes = txtLikes.text;
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
			string[] likeLines = fileLikes.Split ('\n');
			foreach (string line in likeLines) 
			{
				string[] lineData = (line.Trim()).Split(';');

				if (lineData.Length == 2) 
				{
					int i = 0;
					string[] kanaTypes = { "hiragana", "katakana" };
					foreach (string kanaType in kanaTypes) {
						string[] brothers = lineData [i].Split (',');
						foreach (string brother in brothers) {
							Kana kana = FindByRomaji (brother);
							if (kana != null) {
								foreach (string tmpBrother in brothers) {
									Kana tmpKana = FindByRomaji (tmpBrother);
									if (tmpKana != null && tmpKana.romaji != kana.romaji) {
										switch (kanaType) {
										case "hiragana":
											kana.hiraganaLikes.Add(tmpKana);
											break;
										case "katakana":
											kana.katakanaLikes.Add(tmpKana);
											break;
										}
									}
								}
							}
						}
						i++;
					}
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

		public Kana GetRandomKana() {
			return kanas[new System.Random().Next( 1, kanas.Count -2 )];
		}
	}

	public class Kana
	{
		public string romaji { get; set; }
		public string hiragana { get; set; }
		public string katakana { get; set; }
		public List<Kana> katakanaLikes { get; set; }
		public List<Kana> hiraganaLikes { get; set; }

		public Kana(string romaji, string hiragana, string katakana)
		{
			this.romaji = romaji;
			this.hiragana = hiragana;
			this.katakana = katakana;
			this.hiraganaLikes = new List<Kana> ();
			this.katakanaLikes = new List<Kana> ();
		}
	}
}
	