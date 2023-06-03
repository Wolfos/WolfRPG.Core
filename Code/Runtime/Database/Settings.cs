using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.UnityConverters.Math;

namespace WolfRPG.Core
{
	public static class Settings
	{
		public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto,
			Converters = new JsonConverter[] {
				new Vector3Converter(),
				new StringEnumConverter(),
				new QuaternionConverter()
			}
		};
	}
}