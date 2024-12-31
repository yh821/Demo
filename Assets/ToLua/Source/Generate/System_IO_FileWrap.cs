﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class System_IO_FileWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("File");
		L.RegFunction("OpenText", OpenText);
		L.RegFunction("CreateText", CreateText);
		L.RegFunction("AppendText", AppendText);
		L.RegFunction("Copy", Copy);
		L.RegFunction("Create", Create);
		L.RegFunction("Delete", Delete);
		L.RegFunction("Exists", Exists);
		L.RegFunction("Open", Open);
		L.RegFunction("SetCreationTime", SetCreationTime);
		L.RegFunction("SetCreationTimeUtc", SetCreationTimeUtc);
		L.RegFunction("GetCreationTime", GetCreationTime);
		L.RegFunction("GetCreationTimeUtc", GetCreationTimeUtc);
		L.RegFunction("SetLastAccessTime", SetLastAccessTime);
		L.RegFunction("SetLastAccessTimeUtc", SetLastAccessTimeUtc);
		L.RegFunction("GetLastAccessTime", GetLastAccessTime);
		L.RegFunction("GetLastAccessTimeUtc", GetLastAccessTimeUtc);
		L.RegFunction("SetLastWriteTime", SetLastWriteTime);
		L.RegFunction("SetLastWriteTimeUtc", SetLastWriteTimeUtc);
		L.RegFunction("GetLastWriteTime", GetLastWriteTime);
		L.RegFunction("GetLastWriteTimeUtc", GetLastWriteTimeUtc);
		L.RegFunction("GetAttributes", GetAttributes);
		L.RegFunction("SetAttributes", SetAttributes);
		L.RegFunction("OpenRead", OpenRead);
		L.RegFunction("OpenWrite", OpenWrite);
		L.RegFunction("ReadAllText", ReadAllText);
		L.RegFunction("WriteAllText", WriteAllText);
		L.RegFunction("ReadAllBytes", ReadAllBytes);
		L.RegFunction("WriteAllBytes", WriteAllBytes);
		L.RegFunction("ReadAllLines", ReadAllLines);
		L.RegFunction("ReadLines", ReadLines);
		L.RegFunction("WriteAllLines", WriteAllLines);
		L.RegFunction("AppendAllText", AppendAllText);
		L.RegFunction("AppendAllLines", AppendAllLines);
		L.RegFunction("Replace", Replace);
		L.RegFunction("Move", Move);
		L.RegFunction("Encrypt", Encrypt);
		L.RegFunction("Decrypt", Decrypt);
		L.RegFunction("ReadAllTextAsync", ReadAllTextAsync);
		L.RegFunction("WriteAllTextAsync", WriteAllTextAsync);
		L.RegFunction("ReadAllBytesAsync", ReadAllBytesAsync);
		L.RegFunction("WriteAllBytesAsync", WriteAllBytesAsync);
		L.RegFunction("ReadAllLinesAsync", ReadAllLinesAsync);
		L.RegFunction("WriteAllLinesAsync", WriteAllLinesAsync);
		L.RegFunction("AppendAllTextAsync", AppendAllTextAsync);
		L.RegFunction("AppendAllLinesAsync", AppendAllLinesAsync);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenText(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.StreamReader o = System.IO.File.OpenText(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CreateText(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.StreamWriter o = System.IO.File.CreateText(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendText(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.StreamWriter o = System.IO.File.AppendText(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Copy(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.IO.File.Copy(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				System.IO.File.Copy(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.Copy");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Create(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.IO.FileStream o = System.IO.File.Create(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
				System.IO.FileStream o = System.IO.File.Create(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
				System.IO.FileOptions arg2 = (System.IO.FileOptions)ToLua.CheckObject(L, 3, typeof(System.IO.FileOptions));
				System.IO.FileStream o = System.IO.File.Create(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.Create");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Delete(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.File.Delete(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Exists(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			bool o = System.IO.File.Exists(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Open(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.IO.FileMode arg1 = (System.IO.FileMode)ToLua.CheckObject(L, 2, typeof(System.IO.FileMode));
				System.IO.FileStream o = System.IO.File.Open(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.IO.FileMode arg1 = (System.IO.FileMode)ToLua.CheckObject(L, 2, typeof(System.IO.FileMode));
				System.IO.FileAccess arg2 = (System.IO.FileAccess)ToLua.CheckObject(L, 3, typeof(System.IO.FileAccess));
				System.IO.FileStream o = System.IO.File.Open(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.IO.FileMode arg1 = (System.IO.FileMode)ToLua.CheckObject(L, 2, typeof(System.IO.FileMode));
				System.IO.FileAccess arg2 = (System.IO.FileAccess)ToLua.CheckObject(L, 3, typeof(System.IO.FileAccess));
				System.IO.FileShare arg3 = (System.IO.FileShare)ToLua.CheckObject(L, 4, typeof(System.IO.FileShare));
				System.IO.FileStream o = System.IO.File.Open(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.Open");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCreationTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime arg1 = StackTraits<System.DateTime>.Check(L, 2);
			System.IO.File.SetCreationTime(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetCreationTimeUtc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime arg1 = StackTraits<System.DateTime>.Check(L, 2);
			System.IO.File.SetCreationTimeUtc(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCreationTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime o = System.IO.File.GetCreationTime(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCreationTimeUtc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime o = System.IO.File.GetCreationTimeUtc(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastAccessTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime arg1 = StackTraits<System.DateTime>.Check(L, 2);
			System.IO.File.SetLastAccessTime(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastAccessTimeUtc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime arg1 = StackTraits<System.DateTime>.Check(L, 2);
			System.IO.File.SetLastAccessTimeUtc(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastAccessTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime o = System.IO.File.GetLastAccessTime(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastAccessTimeUtc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime o = System.IO.File.GetLastAccessTimeUtc(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastWriteTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime arg1 = StackTraits<System.DateTime>.Check(L, 2);
			System.IO.File.SetLastWriteTime(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetLastWriteTimeUtc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime arg1 = StackTraits<System.DateTime>.Check(L, 2);
			System.IO.File.SetLastWriteTimeUtc(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastWriteTime(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime o = System.IO.File.GetLastWriteTime(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLastWriteTimeUtc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.DateTime o = System.IO.File.GetLastWriteTimeUtc(arg0);
			ToLua.PushValue(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetAttributes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.FileAttributes o = System.IO.File.GetAttributes(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetAttributes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.FileAttributes arg1 = (System.IO.FileAttributes)ToLua.CheckObject(L, 2, typeof(System.IO.FileAttributes));
			System.IO.File.SetAttributes(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenRead(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.FileStream o = System.IO.File.OpenRead(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OpenWrite(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.FileStream o = System.IO.File.OpenWrite(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllText(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string o = System.IO.File.ReadAllText(arg0);
				LuaDLL.lua_pushstring(L, o);
				return 1;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 2);
				string o = System.IO.File.ReadAllText(arg0, arg1);
				LuaDLL.lua_pushstring(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.ReadAllText");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllText(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.IO.File.WriteAllText(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.IO.File.WriteAllText(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.WriteAllText");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllBytes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			byte[] o = System.IO.File.ReadAllBytes(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllBytes(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			byte[] arg1 = ToLua.CheckByteBuffer(L, 2);
			System.IO.File.WriteAllBytes(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllLines(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string[] o = System.IO.File.ReadAllLines(arg0);
				ToLua.Push(L, o);
				return 1;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 2);
				string[] o = System.IO.File.ReadAllLines(arg0, arg1);
				ToLua.Push(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.ReadAllLines");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadLines(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> o = System.IO.File.ReadLines(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 2);
				System.Collections.Generic.IEnumerable<string> o = System.IO.File.ReadLines(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.ReadLines");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllLines(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<string[]>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				string[] arg1 = ToLua.ToStringArray(L, 2);
				System.IO.File.WriteAllLines(arg0, arg1);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Collections.Generic.IEnumerable<string>>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.ToObject(L, 2);
				System.IO.File.WriteAllLines(arg0, arg1);
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string[], System.Text.Encoding>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				string[] arg1 = ToLua.ToStringArray(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.ToObject(L, 3);
				System.IO.File.WriteAllLines(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Collections.Generic.IEnumerable<string>, System.Text.Encoding>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.ToObject(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.ToObject(L, 3);
				System.IO.File.WriteAllLines(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.WriteAllLines");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendAllText(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.IO.File.AppendAllText(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.IO.File.AppendAllText(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.AppendAllText");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendAllLines(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.IO.File.AppendAllLines(arg0, arg1);
				return 0;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.IO.File.AppendAllLines(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.AppendAllLines");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Replace(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				string arg2 = ToLua.CheckString(L, 3);
				System.IO.File.Replace(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				string arg2 = ToLua.CheckString(L, 3);
				bool arg3 = LuaDLL.luaL_checkboolean(L, 4);
				System.IO.File.Replace(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.Replace");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Move(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			string arg1 = ToLua.CheckString(L, 2);
			System.IO.File.Move(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Encrypt(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.File.Encrypt(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Decrypt(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			System.IO.File.Decrypt(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllTextAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Threading.Tasks.Task<string> o = System.IO.File.ReadAllTextAsync(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Threading.CancellationToken>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Threading.CancellationToken arg1 = StackTraits<System.Threading.CancellationToken>.To(L, 2);
				System.Threading.Tasks.Task<string> o = System.IO.File.ReadAllTextAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Text.Encoding>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.ToObject(L, 2);
				System.Threading.Tasks.Task<string> o = System.IO.File.ReadAllTextAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.Check(L, 3);
				System.Threading.Tasks.Task<string> o = System.IO.File.ReadAllTextAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.ReadAllTextAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllTextAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllTextAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Threading.CancellationToken>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.To(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllTextAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Text.Encoding>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.ToObject(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllTextAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.Threading.CancellationToken arg3 = StackTraits<System.Threading.CancellationToken>.Check(L, 4);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllTextAsync(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.WriteAllTextAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllBytesAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Threading.Tasks.Task<byte[]> o = System.IO.File.ReadAllBytesAsync(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Threading.CancellationToken arg1 = StackTraits<System.Threading.CancellationToken>.Check(L, 2);
				System.Threading.Tasks.Task<byte[]> o = System.IO.File.ReadAllBytesAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.ReadAllBytesAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllBytesAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				byte[] arg1 = ToLua.CheckByteBuffer(L, 2);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllBytesAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				byte[] arg1 = ToLua.CheckByteBuffer(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.Check(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllBytesAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.WriteAllBytesAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReadAllLinesAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Threading.Tasks.Task<string[]> o = System.IO.File.ReadAllLinesAsync(arg0);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Threading.CancellationToken>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Threading.CancellationToken arg1 = StackTraits<System.Threading.CancellationToken>.To(L, 2);
				System.Threading.Tasks.Task<string[]> o = System.IO.File.ReadAllLinesAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Text.Encoding>(L, 2))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.ToObject(L, 2);
				System.Threading.Tasks.Task<string[]> o = System.IO.File.ReadAllLinesAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Text.Encoding arg1 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.Check(L, 3);
				System.Threading.Tasks.Task<string[]> o = System.IO.File.ReadAllLinesAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.ReadAllLinesAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int WriteAllLinesAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllLinesAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Threading.CancellationToken>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.To(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllLinesAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Text.Encoding>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.ToObject(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllLinesAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.Threading.CancellationToken arg3 = StackTraits<System.Threading.CancellationToken>.Check(L, 4);
				System.Threading.Tasks.Task o = System.IO.File.WriteAllLinesAsync(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.WriteAllLinesAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendAllTextAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllTextAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Threading.CancellationToken>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.To(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllTextAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Text.Encoding>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.ToObject(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllTextAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string arg1 = ToLua.CheckString(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.Threading.CancellationToken arg3 = StackTraits<System.Threading.CancellationToken>.Check(L, 4);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllTextAsync(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.AppendAllTextAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AppendAllLinesAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllLinesAsync(arg0, arg1);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Threading.CancellationToken>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Threading.CancellationToken arg2 = StackTraits<System.Threading.CancellationToken>.To(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllLinesAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Text.Encoding>(L, 3))
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.ToObject(L, 3);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllLinesAsync(arg0, arg1, arg2);
				ToLua.PushObject(L, o);
				return 1;
			}
			else if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				System.Collections.Generic.IEnumerable<string> arg1 = (System.Collections.Generic.IEnumerable<string>)ToLua.CheckObject<System.Collections.Generic.IEnumerable<string>>(L, 2);
				System.Text.Encoding arg2 = (System.Text.Encoding)ToLua.CheckObject<System.Text.Encoding>(L, 3);
				System.Threading.CancellationToken arg3 = StackTraits<System.Threading.CancellationToken>.Check(L, 4);
				System.Threading.Tasks.Task o = System.IO.File.AppendAllLinesAsync(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: System.IO.File.AppendAllLinesAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

