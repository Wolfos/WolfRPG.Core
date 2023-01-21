using System;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core
{
	[AttributeUsage(AttributeTargets.Property)]
	public class AssetReferenceAttribute : Attribute
	{
		public Type Type;

		public AssetReferenceAttribute(Type type)
		{
			Type = type;
		}
	}
	
	public class AssetReference
	{
		public string Guid { get; set; }

		private object _cachedAsset;
		
		/// <summary>
		/// Get referenced asset. First time use loads the asset synchronously. The asset is cached afterwards
		/// </summary>
		public T GetAsset<T>()
		{
			if (_cachedAsset != null)
			{
				return (T) _cachedAsset;
			}
			
			var operation = Addressables.LoadAssetAsync<T>(Guid);
			var asset = operation.WaitForCompletion();
			_cachedAsset = asset;
			return asset;
		}
	}
}