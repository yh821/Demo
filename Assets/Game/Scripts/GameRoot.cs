using System;
using UnityEngine;
using System.Collections;
using System.IO;
using LuaInterface;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameRoot : MonoBehaviour
{
	public static GameRoot Instance { get; private set; }
	[NoToLua] public LuaState LuaState { get; private set; }

	public const string ProjCode = "Demo";

	[SerializeField] private GameObject loadingPrefab;

	private bool checkedVersion = false;
	private static bool isEnterGame;
	private LuaBundleLoader luaLoader;
	private LuaLooper looper;
	private LuaFunction luaFocus;
	private LuaFunction luaPause;

	private void Awake()
	{
		Debugger.Log("game root awake");
		SetLogEnable();

		Instance = this;
		DontDestroyOnLoad(gameObject);

		var loading = Instantiate(loadingPrefab);
		loading.name = loading.name.Replace("(Clone)", string.Empty);
		DontDestroyOnLoad(loading);
	}

	private void Start()
	{
		CheckPackageVersion();

		StartGame();
	}

	private void StartGame()
	{
		Debugger.Log("game root start");
		// Graphic.defaultGraphicMaterial.shader = Shader.Find("Yif/UIDefault");

		luaLoader = new LuaBundleLoader();
		luaLoader.LoadAliasResPathMap();

		LuaState = new LuaState();
		LuaState.OpenLibs(LuaDLL.luaopen_struct);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
		LuaState.OpenLibs(LuaDLL.luaopen_bit);
#endif
		OpenLuaSocket();
		OpenCJson();
		LuaLog.OpenLibs(LuaState);

		LuaState.LuaPushFunction(AddLuaBundle);
		LuaState.LuaSetGlobal("AddLuaBundle");
		LuaState.LuaPushFunction(CreateCfgUd);
		LuaState.LuaSetGlobal("create_cfg_ud");
		LuaState.LuaSetTop(0);

		LuaBinder.Bind(LuaState);
		DelegateFactory.Init();
		LuaCoroutine.Register(LuaState, this);

		looper = gameObject.AddComponent<LuaLooper>();
		looper.luaState = LuaState;

#if UNITY_EDITOR
		SetGlobalBoolean("UNITY_EDITOR", true);
		var simulateAssetBundle = EditorPrefs.GetInt("SimulateAssetBundle", 1) == 1;
		SetGlobalBoolean("GAME_ASSET_BUNDLE", !simulateAssetBundle);
#endif

#if UNITY_ANDROID
		SetGlobalBoolean("UNITY_ANDROID", true);
#endif

#if UNITY_IOS
		SetGlobalBoolean("UNITY_IOS", true);
#endif

#if UNITY_STANDALONE
		SetGlobalBoolean("UNITY_STANDALONE", true);
#endif

#if UNITY_STANDALONE_WIN
		SetGlobalBoolean("UNITY_STANDALONE_WIN", true);
#endif

		LuaState.Start();
		luaLoader.SetupLuaLoader(LuaState);

		try
		{
			LuaState.DoFile("Main.lua");
			LuaState.DoFile("logic/Game.lua");

			luaFocus = LuaState.GetFunction("GameFocus");
			luaPause = LuaState.GetFunction("GamePause");
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	private void SetGlobalBoolean(string key, bool value)
	{
		LuaState.LuaPushBoolean(value);
		LuaState.LuaSetGlobal(key);
	}

	private void OpenCJson()
	{
		LuaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
		LuaState.OpenLibs(LuaDLL.luaopen_cjson);
		LuaState.LuaSetField(-2, "cjson");

		LuaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
		LuaState.LuaSetField(-2, "cjson.safe");
	}

	private void OpenLuaSocket()
	{
		LuaConst.openLuaSocket = true;
		LuaState.BeginPreLoad();
		LuaState.RegFunction("mime.core", LuaOpen_Mime_Core);
		LuaState.EndPreLoad();
	}

	private void CheckPackageVersion()
	{
		if (checkedVersion) return;
		checkedVersion = true;

		if (string.IsNullOrEmpty(Application.version))
		{
			Debug.LogErrorFormat("Application Version is null: {0} {1}", Application.productName,
				Application.installerName);
			return;
		}

		var version = PlayerPrefs.GetString("GameRootPackageVersion");
		if (string.IsNullOrEmpty(version) || version != Application.version)
		{
			var needRestart = false;
			var success = true;
			try
			{
				needRestart = DeleteLuaCache();
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("删除lua缓存失败 Message{0}, OldVersion:{1} NewVersion:{2} ProductName{3}",
					e.Message, version, Application.version, Application.productName);
				success = false;
			}

			if (success)
			{
				PlayerPrefs.SetString("GameRootPackageVersion", Application.version);
				if (needRestart)
				{
					Debug.LogErrorFormat("删除lua缓存重启 OldVersion:{0} NewVersion:{1} ProductName{2}",
						version, Application.version, Application.productName);
				}
			}
		}
	}

	private bool DeleteLuaCache()
	{
		var path = Path.Combine(Application.persistentDataPath, "BundleCache/LuaAssetBundle/LuaAssetBundle.lua");
		if (File.Exists(path))
		{
			try
			{
				File.Delete(path);
			}
			catch (Exception e)
			{
				throw e;
			}

			return true;
		}

		return false;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (LuaState != null) luaFocus.Call(hasFocus);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (LuaState != null) luaPause.Call(pauseStatus);
	}

	private void OnApplicationQuit()
	{
#if UNITY_EDITOR
		if (LuaState != null)
		{
			LuaState.Dispose();
			LuaState = null;
		}
#endif
	}

	public void SetLogEnable(bool isForceClose = false)
	{
#if !UNITY_EDITOR && UNITY_STANDALONE
		Debug.unityLogger.logEnabled = true;
		return;
#endif

		if (isForceClose)
		{
			Debug.unityLogger.logEnabled = false;
			return;
		}

		if (!Debug.isDebugBuild)
		{
			Debug.unityLogger.filterLogType = LogType.Error;
			return;
		}

		Debug.unityLogger.filterLogType = LogType.Log;
	}


	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int LuaOpen_Mime_Core(IntPtr l)
	{
		return LuaDLL.luaopen_mime_core(l);
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AddLuaBundle(IntPtr l)
	{
		var luaFile = LuaDLL.luaL_checklstring(l, 1, out _);
		var bundleName = LuaDLL.luaL_checklstring(l, 1, out _);
		Instance.luaLoader.AddLuaBundle(luaFile, bundleName);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CreateCfgUd(IntPtr l)
	{
		LuaDLL.lua_newuserdata(l, 0);
		LuaDLL.lua_insert(l, 1);
		LuaDLL.lua_setmetatable(l, 1);
		return 1;
	}

	public static bool IsAndroid64()
	{
		return IntPtr.Size == 8;
	}
}