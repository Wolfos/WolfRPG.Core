using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core.Localization
{
	public class LocalizationFile
	{
		// Label to identify the file in Addressables
		public static string Label = "WolfRPG Localization File";

		private static readonly List<string> LanguageLookUpTable = new() {"Afrikaans","Arabic","Basque","Belarusian","Bulgarian","Catalan","Chinese","Czech","Danish","Dutch","English","Estonian","Faroese","Finnish","French","German","Greek","Hebrew","Hungarian","Icelandic","Indonesian","Italian","Japanese","Korean","Latvian","Lithuanian","Norwegian","Polish","Portuguese","Romanian","Russian","SerboCroatian","Slovak","Slovenian","Spanish","Swedish","Thai","Turkish","Ukrainian","Vietnamese","ChineseSimplified","ChineseTraditional","Hindi" };
		
		private static Dictionary<string, LocalizationString> _loadedStrings;
		private static List<SystemLanguage> _languages;

		private static void Load()
		{
			// No caching in the editor
			#if UNITY_EDITOR == false
			if (_loadedStrings != null && _loadedStrings.Count > 0) return;
			#endif

			var operation = Addressables.LoadAssetAsync<TextAsset>(Label);
			var asset = operation.WaitForCompletion();
			
			var lines = asset.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
			_loadedStrings = new();
			
			var splitString = lines[0].Split(';');
			_languages = new();
			// First column is the identifiers column so skip that
			for(var i = 1; i < splitString.Length; i++)
			{
				if (LanguageLookUpTable.Contains(splitString[i]) == false)
				{
					Debug.LogError($"Invalid language {splitString[i]}");
					return;
				}
				
				var index = LanguageLookUpTable.FindIndex(lang => lang == splitString[i]);
				_languages.Add((SystemLanguage)index);
			}

			for (var i = 1; i < lines.Length; i++)
			{
				splitString = lines[i].Split(';');
				var localizationString = new LocalizationString
				{
					LocalizedString = new Dictionary<SystemLanguage, string>()
				};
				var identifier = "";
				for (var ii = 0; ii < splitString.Length; ii++)
				{
					var text = splitString[ii];
					if (ii == 0)
					{
						identifier = text;
						continue;
					}

					var language = _languages[ii - 1];
					localizationString.LocalizedString.Add(language, text);
				}
				
				_loadedStrings.Add(identifier, localizationString);
			}
		}

		public static string Get(string identifier, SystemLanguage language)
		{
			// Load will only run once during runtime
			Load();

			if (_loadedStrings.ContainsKey(identifier) == false)
			{
				Debug.LogError($"Localized string for {identifier} was not found");
				return "";
			}

			return _loadedStrings[identifier].LocalizedString[language];
		}
	}
}