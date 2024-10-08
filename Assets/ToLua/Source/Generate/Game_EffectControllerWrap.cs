﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Game_EffectControllerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Game.EffectController), typeof(UnityEngine.MonoBehaviour));
		L.RegFunction("WaitFinish", WaitFinish);
		L.RegFunction("WaitFadeout", WaitFadeout);
		L.RegFunction("EstimateDuration", EstimateDuration);
		L.RegFunction("SimulateInit", SimulateInit);
		L.RegFunction("SimulateStart", SimulateStart);
		L.RegFunction("SimulateDelta", SimulateDelta);
		L.RegFunction("Simulate", Simulate);
		L.RegFunction("Play", Play);
		L.RegFunction("Pause", Pause);
		L.RegFunction("Resume", Resume);
		L.RegFunction("Stop", Stop);
		L.RegFunction("Reset", Reset);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("IsLooping", get_IsLooping, set_IsLooping);
		L.RegVar("Duration", get_Duration, set_Duration);
		L.RegVar("Fadeout", get_Fadeout, set_Fadeout);
		L.RegVar("IsPaused", get_IsPaused, null);
		L.RegVar("IsStopped", get_IsStopped, null);
		L.RegVar("IsNoScalable", get_IsNoScalable, null);
		L.RegVar("PlaybackSpeed", get_PlaybackSpeed, set_PlaybackSpeed);
		L.RegVar("FadeoutEvent", get_FadeoutEvent, set_FadeoutEvent);
		L.RegVar("FinishEvent", get_FinishEvent, set_FinishEvent);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WaitFinish(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			obj.WaitFinish(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WaitFadeout(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			System.Action arg0 = (System.Action)ToLua.CheckDelegate<System.Action>(L, 2);
			obj.WaitFadeout(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int EstimateDuration(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.EstimateDuration();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SimulateInit(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.SimulateInit();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SimulateStart(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.SimulateStart();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SimulateDelta(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 3);
			obj.SimulateDelta(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Simulate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.Simulate(arg0);
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
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.Play();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Pause(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.Pause();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Resume(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.Resume();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Stop(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.Stop();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reset(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			obj.Reset();
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
	static int get_IsLooping(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			bool ret = obj.IsLooping;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index IsLooping on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Duration(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			float ret = obj.Duration;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Duration on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Fadeout(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			float ret = obj.Fadeout;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Fadeout on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsPaused(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			bool ret = obj.IsPaused;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index IsPaused on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsStopped(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			bool ret = obj.IsStopped;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index IsStopped on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_IsNoScalable(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			bool ret = obj.IsNoScalable;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index IsNoScalable on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_PlaybackSpeed(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			float ret = obj.PlaybackSpeed;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index PlaybackSpeed on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FadeoutEvent(IntPtr L)
	{
		ToLua.Push(L, new EventObject(typeof(System.Action)));
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_FinishEvent(IntPtr L)
	{
		ToLua.Push(L, new EventObject(typeof(System.Action)));
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_IsLooping(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.IsLooping = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index IsLooping on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Duration(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.Duration = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Duration on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Fadeout(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.Fadeout = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Fadeout on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_PlaybackSpeed(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Game.EffectController obj = (Game.EffectController)o;
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.PlaybackSpeed = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index PlaybackSpeed on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_FadeoutEvent(IntPtr L)
	{
		try
		{
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			EventObject arg0 = null;

			if (LuaDLL.lua_isuserdata(L, 2) != 0)
			{
				arg0 = (EventObject)ToLua.ToObject(L, 2);
			}
			else
			{
				return LuaDLL.luaL_throw(L, "The event 'Game.EffectController.FadeoutEvent' can only appear on the left hand side of += or -= when used outside of the type 'Game.EffectController'");
			}

			if (arg0.op == EventOp.Add)
			{
				System.Action ev = (System.Action)arg0.func;
				obj.FadeoutEvent += ev;
			}
			else if (arg0.op == EventOp.Sub)
			{
				System.Action ev = (System.Action)arg0.func;
				obj.FadeoutEvent -= ev;
			}

			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_FinishEvent(IntPtr L)
	{
		try
		{
			Game.EffectController obj = (Game.EffectController)ToLua.CheckObject(L, 1, typeof(Game.EffectController));
			EventObject arg0 = null;

			if (LuaDLL.lua_isuserdata(L, 2) != 0)
			{
				arg0 = (EventObject)ToLua.ToObject(L, 2);
			}
			else
			{
				return LuaDLL.luaL_throw(L, "The event 'Game.EffectController.FinishEvent' can only appear on the left hand side of += or -= when used outside of the type 'Game.EffectController'");
			}

			if (arg0.op == EventOp.Add)
			{
				System.Action ev = (System.Action)arg0.func;
				obj.FinishEvent += ev;
			}
			else if (arg0.op == EventOp.Sub)
			{
				System.Action ev = (System.Action)arg0.func;
				obj.FinishEvent -= ev;
			}

			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

