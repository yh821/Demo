﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Game_AudioItemWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Game.AudioItem), typeof(UnityEngine.ScriptableObject));
		L.RegFunction("PlaySubItem", PlaySubItem);
		L.RegFunction("Play", Play);
		L.RegFunction("IsValid", IsValid);
		L.RegFunction("New", _CreateGame_AudioItem);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Delay", get_Delay, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateGame_AudioItem(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Game.AudioItem obj = new Game.AudioItem();
				ToLua.Push(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Game.AudioItem.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlaySubItem(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			Game.AudioItem obj = (Game.AudioItem)ToLua.CheckObject<Game.AudioItem>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			UnityEngine.AudioSource arg1 = (UnityEngine.AudioSource)ToLua.CheckObject(L, 3, typeof(UnityEngine.AudioSource));
			obj.PlaySubItem(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Play(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.AudioItem obj = (Game.AudioItem)ToLua.CheckObject<Game.AudioItem>(L, 1);
			Game.AudioSourcePool arg0 = (Game.AudioSourcePool)ToLua.CheckObject(L, 2, typeof(Game.AudioSourcePool));
			Game.IAudioController o = obj.Play(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsValid(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.AudioItem obj = (Game.AudioItem)ToLua.CheckObject<Game.AudioItem>(L, 1);
			bool o = obj.IsValid();
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
	static int get_Delay(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.AudioItem obj = (Game.AudioItem)o;
			float ret = obj.Delay;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Delay on a nil value");
		}
	}
}

