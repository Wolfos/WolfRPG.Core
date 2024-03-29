using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core.Localization
{
	public class LocalizationFile
	{
		// Label to identify the file in Addressables
		public const string Label = "WolfRPG Localization File";

		private static readonly List<string> LanguageLookUpTable = new() {"Afrikaans","Arabic","Basque","Belarusian","Bulgarian","Catalan","Chinese","Czech","Danish","Dutch","English","Estonian","Faroese","Finnish","French","German","Greek","Hebrew","Hungarian","Icelandic","Indonesian","Italian","Japanese","Korean","Latvian","Lithuanian","Norwegian","Polish","Portuguese","Romanian","Russian","SerboCroatian","Slovak","Slovenian","Spanish","Swedish","Thai","Turkish","Ukrainian","Vietnamese","ChineseSimplified","ChineseTraditional","Hindi" };
		
		private static Dictionary<string, LocalizationString> _loadedStrings;
		private static List<SystemLanguage> _languages;

		public static SystemLanguage DefaultLanguage { get; set; } = SystemLanguage.English;
		public static SystemLanguage TargetLanguage { get; set; } = Application.systemLanguage;

		private static void Load()
		{
			// No caching in the editor
			#if UNITY_EDITOR == false
			if (_loadedStrings != null && _loadedStrings.Count > 0) return;
			#endif

			Debug.Log("Loading localization file");
			var operation = Addressables.LoadAssetAsync<TextAsset>(Label);
			var asset = operation.WaitForCompletion();
			Addressables.Release(operation);
			
			var lines = asset.text.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
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
					LocalizedString = new()
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

		public static bool HasIdentifier(string identifier)
		{
			Load();
			return _loadedStrings.ContainsKey(identifier);
		}

		public static string Get(string identifier)
		{
			return Get(identifier, TargetLanguage);
		}

		public static string Get(string identifier, SystemLanguage language)
		{
			// Load will only run once during runtime, but every time in-editor so that changes will show without having to restart the editor
			Load();

			if (_loadedStrings.ContainsKey(identifier) == false)
			{
				Debug.LogError($"Localized string for {identifier} was not found");
				return "";
			}

			var str = _loadedStrings[identifier];
			if (str.LocalizedString.ContainsKey(language))
			{
				return str.LocalizedString[language];
			}

			return str.LocalizedString[DefaultLanguage];
			
		}
	}
}