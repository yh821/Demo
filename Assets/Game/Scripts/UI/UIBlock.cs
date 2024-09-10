using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 空白图(无DC)，可点击但不可见
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class UIBlock : MaskableGraphic
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.grey;
		var corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		for (int i = 0; i < corners.Length; i++)
		{
			var index = i + 1;
			if (index >= corners.Length)
				index = 0;
			Gizmos.DrawLine(corners[i], corners[index]);
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}