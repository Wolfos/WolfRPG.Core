using System;
using UnityEngine;

namespace WolfRPG.Core.Localization
{
	[Serializable]
	public class LocalizedString
	{
		/// <summary>
		/// Identifier by which to look up the localized string from the file
		/// </summary>
		public string Identifier;

		public string Get()
		{
			return Get(LocalizationFile.TargetLanguage);
		}

		public string Get(SystemLanguage language)
		{
			if (string.IsNullOrEmpty(Identifier)) return string.Empty;

			return LocalizationFile.Get(Identifier, language);
		}
	}
}