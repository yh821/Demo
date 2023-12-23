﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UnityEngine_UI_Image_TypeWrap
{
	public static void Register(LuaState L)
	{
		L.BeginEnum(typeof(UnityEngine.UI.Image.Type));
		L.RegVar("Simple", get_Simple, null);
		L.RegVar("Sliced", get_Sliced, null);
		L.RegVar("Tiled", get_Tiled, null);
		L.RegVar("Filled", get_Filled, null);
		L.RegFunction("IntToEnum", IntToEnum);
		L.EndEnum();
		TypeTraits<UnityEngine.UI.Image.Type>.Check = CheckType;
		StackTraits<UnityEngine.UI.Image.Type>.Push = Push;
	}

	static void Push(IntPtr L, UnityEngine.UI.Image.Type arg)
	{
		ToLua.Push(L, arg);
	}

	static bool CheckType(IntPtr L, int pos)
	{
		return TypeChecker.CheckEnumType(typeof(UnityEngine.UI.Image.Type), L, pos);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Simple(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.UI.Image.Type.Simple);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Sliced(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.UI.Image.Type.Sliced);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Tiled(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.UI.Image.Type.Tiled);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Filled(IntPtr L)
	{
		ToLua.Push(L, UnityEngine.UI.Image.Type.Filled);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		UnityEngine.UI.Image.Type o = (UnityEngine.UI.Image.Type)arg0;
		ToLua.Push(L, o);
		return 1;
	}
}

