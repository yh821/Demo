﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_UI_RectMask2DWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(UnityEngine.UI.RectMask2D), typeof(UnityEngine.EventSystems.UIBehaviour));
		L.RegFunction("IsRaycastLocationValid", IsRaycastLocationValid);
		L.RegFunction("PerformClipping", PerformClipping);
		L.RegFunction("UpdateClipSoftness", UpdateClipSoftness);
		L.RegFunction("AddClippable", AddClippable);
		L.RegFunction("RemoveClippable", RemoveClippable);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("padding", get_padding, set_padding);
		L.RegVar("softness", get_softness, set_softness);
		L.RegVar("canvasRect", get_canvasRect, null);
		L.RegVar("rectTransform", get_rectTransform, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsRaycastLocationValid(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)ToLua.CheckObject<UnityEngine.UI.RectMask2D>(L, 1);
			UnityEngine.Vector2 arg0 = ToLua.ToVector2(L, 2);
			UnityEngine.Camera arg1 = (UnityEngine.Camera)ToLua.CheckObject(L, 3, typeof(UnityEngine.Camera));
			bool o = obj.IsRaycastLocationValid(arg0, arg1);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PerformClipping(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)ToLua.CheckObject<UnityEngine.UI.RectMask2D>(L, 1);
			obj.PerformClipping();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateClipSoftness(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)ToLua.CheckObject<UnityEngine.UI.RectMask2D>(L, 1);
			obj.UpdateClipSoftness();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddClippable(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)ToLua.CheckObject<UnityEngine.UI.RectMask2D>(L, 1);
			UnityEngine.UI.IClippable arg0 = (UnityEngine.UI.IClippable)ToLua.CheckObject<UnityEngine.UI.IClippable>(L, 2);
			obj.AddClippable(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveClippable(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)ToLua.CheckObject<UnityEngine.UI.RectMask2D>(L, 1);
			UnityEngine.UI.IClippable arg0 = (UnityEngine.UI.IClippable)ToLua.CheckObject<UnityEngine.UI.IClippable>(L, 2);
			obj.RemoveClippable(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_padding(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)o;
			UnityEngine.Vector4 ret = obj.padding;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index padding on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_softness(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)o;
			UnityEngine.Vector2Int ret = obj.softness;
			ToLua.PushValue(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index softness on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_canvasRect(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)o;
			UnityEngine.Rect ret = obj.canvasRect;
			ToLua.PushValue(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index canvasRect on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_rectTransform(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)o;
			UnityEngine.RectTransform ret = obj.rectTransform;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index rectTransform on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_padding(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)o;
			UnityEngine.Vector4 arg0 = ToLua.ToVector4(L, 2);
			obj.padding = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index padding on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_softness(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			UnityEngine.UI.RectMask2D obj = (UnityEngine.UI.RectMask2D)o;
			UnityEngine.Vector2Int arg0 = StackTraits<UnityEngine.Vector2Int>.Check(L, 2);
			obj.softness = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index softness on a nil value");
		}
	}
}

