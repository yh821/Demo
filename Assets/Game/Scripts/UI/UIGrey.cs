using UnityEngine;

namespace Game
{
	public class UIGrey : MonoBehaviour
	{
		private bool isGrey = false;

		public bool GetGrey() => isGrey;

		public void SetGrey(bool isGrey, Material greyMat)
		{
			if (this.isGrey == isGrey) return;
			if (isGrey && greyMat == null) return;
			this.isGrey = isGrey;

			var graphics = ListPool<UnityEngine.UI.Graphic>.Get();
			GetComponentsInChildren(true, graphics);
			foreach (var graphic in graphics)
			{
				var layer = graphic.gameObject.layer;
				if (graphic is UIEffect
				    || layer == LayerMask.NameToLayer("UIEffect")
				    || layer == LayerMask.NameToLayer("UI3DEffect"))
					continue;
				graphic.material = isGrey ? greyMat : null;
			}
			ListPool<UnityEngine.UI.Graphic>.Release(graphics);
		}
	}
}