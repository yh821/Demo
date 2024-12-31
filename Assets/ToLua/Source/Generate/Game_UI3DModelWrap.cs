﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Game_UI3DModelWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Game.UI3DModel), typeof(UnityEngine.UI.MaskableGraphic));
		L.RegFunction("GetTarget", GetTarget);
		L.RegFunction("SetOverrideOrder", SetOverrideOrder);
		L.RegFunction("SetNoOverrideOrder", SetNoOverrideOrder);
		L.RegFunction("SetIsGrey", SetIsGrey);
		L.RegFunction("SetIsSupportClip", SetIsSupportClip);
		L.RegFunction("SetIsSupportMask", SetIsSupportMask);
		L.RegFunction("SetOverrideLayer", SetOverrideLayer);
		L.RegFunction("OnAddGameObject", OnAddGameObject);
		L.RegFunction("OnRemoveGameObject", OnRemoveGameObject);
		L.RegFunction("SetOnEnableCallBack", SetOnEnableCallBack);
		L.RegFunction("SetOnDisableCallBack", SetOnDisableCallBack);
		L.RegFunction("GetClipRect", GetClipRect);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTarget(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			UnityEngine.GameObject o = obj.GetTarget();
			ToLua.PushSealed(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetOverrideOrder(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 5);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
			int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);
			int arg3;
			obj.SetOverrideOrder(arg0, arg1, arg2, out arg3);
			LuaDLL.lua_pushinteger(L, arg3);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetNoOverrideOrder(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.SetNoOverrideOrder(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetIsGrey(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.SetIsGrey(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetIsSupportClip(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.SetIsSupportClip(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetIsSupportMask(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.SetIsSupportMask(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetOverrideLayer(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.SetOverrideLayer(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnAddGameObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			obj.OnAddGameObject(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnRemoveGameObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			obj.OnRemoveGameObject(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetOnEnableCallBack(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			obj.SetOnEnableCallBack(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetOnDisableCallBack(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.UI3DModel obj = (Game.UI3DModel)ToLua.CheckObject<Game.UI3DModel>(L, 1);
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			obj.SetOnDisableCallBack(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClipRect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Vector3 arg0 = ToLua.ToVector3(L, 1);
			UnityEngine.Vector3 arg1 = ToLua.ToVector3(L, 2);
			UnityEngine.Vector4 o = Game.UI3DModel.GetClipRect(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
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
}

