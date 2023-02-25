using UnityEngine;

namespace WolfRPG.Core.Localization
{
	public class LocalizedString
	{
		/// <summary>
		/// Identifier by which to look up the localized string from the file
		/// </summary>
		public string Identifier { get; set; }

		public string Get()
		{
			if (string.IsNullOrEmpty(Identifier)) return string.Empty;

			return LocalizationFile.Get(Identifier, Application.systemLanguage);
		}
	}
}