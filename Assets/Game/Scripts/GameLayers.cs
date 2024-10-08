﻿using UnityEngine;

public static class GameLayers
{
	private static int? walkable;
	private static int? road;
	private static int? water;
	private static int? clickable;
	private static int? uiEffect;
	private static int? ui3D;

	public static int Default => 0;

	public static int Walkable
	{
		get
		{
			if (!walkable.HasValue)
				walkable = LayerMask.NameToLayer("Walkable");
			return walkable.Value;
		}
	}

	public static int Road
	{
		get
		{
			if (!road.HasValue)
				road = LayerMask.NameToLayer("Road");
			return road.Value;
		}
	}

	public static int Water
	{
		get
		{
			if (!water.HasValue)
				water = LayerMask.NameToLayer("Water");
			return water.Value;
		}
	}

	public static int Clickable
	{
		get
		{
			if (!clickable.HasValue)
				clickable = LayerMask.NameToLayer("Clickable");
			return clickable.Value;
		}
	}

	public static int UIEffect
	{
		get
		{
			if (!uiEffect.HasValue)
				uiEffect = LayerMask.NameToLayer("UIEffect");
			return uiEffect.Value;
		}
	}

	public static int UI3D
	{
		get
		{
			if (!ui3D.HasValue)
				ui3D = LayerMask.NameToLayer("UI3D");
			return ui3D.Value;
		}
	}
}