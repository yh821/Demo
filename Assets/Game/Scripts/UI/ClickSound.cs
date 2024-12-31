using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
	public sealed class UIClickSound : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public static ClickDelegate OnClick;

		public delegate void ClickDelegate(string bundleName, string assetName);

		[SerializeField] public AssetID audioAsset;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (audioAsset.IsEmpty || OnClick == null) return;
			OnClick(audioAsset.BundleName, audioAsset.AssetName);
		}
	}
}