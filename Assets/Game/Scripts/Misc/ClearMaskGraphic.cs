using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game
{
	public class ClearMaskGraphic : Graphic, IMaterialModifier
	{
		private Material mUnmaskMaterial;

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			var unmaskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
			StencilMaterial.Remove(mUnmaskMaterial);
			mUnmaskMaterial = unmaskMaterial;
			return unmaskMaterial;
		}
	}
}