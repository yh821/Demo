﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class GameRootWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(GameRoot), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("SetLogEnable", SetLogEnable);
		L.RegFunction("IsAndroid64", IsAndroid64);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("ProjCode", get_ProjCode, null);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLogEnable(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				GameRoot obj = (GameRoot)ToLua.CheckObject(L, 1, typeof(GameRoot));
				obj.SetLogEnable();
				return 0;
			}
			else if (count == 2)
			{
				GameRoot obj = (GameRoot)ToLua.CheckObject(L, 1, typeof(GameRoot));
				bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
				obj.SetLogEnable(arg0);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: GameRoot.SetLogEnable");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsAndroid64(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 0);
			bool o = GameRoot.IsAndroid64();
			LuaDLL.lua_pushboolean(L, o);
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

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ProjCode(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, GameRoot.ProjCode);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushSealed(L, GameRoot.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

