using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;


namespace LuaFramework
{
	public sealed class GameManager : Manager
	{
		private static bool initialize = false;
		private List<string> downloadFiles = new List<string>();

		/// <summary>
		/// 初始化游戏管理器
		/// </summary>
		private void Awake()
		{
			Init();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		private void Init()
		{
			DontDestroyOnLoad(gameObject); //防止销毁自己

			CheckExtractResource(); //释放资源
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Application.targetFrameRate = AppConst.GameFrameRate;
		}

		/// <summary>
		/// 释放资源
		/// </summary>
		public void CheckExtractResource()
		{
			var isExists = Directory.Exists(Util.DataPath) &&
			               Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
			if (isExists || AppConst.DebugMode)
			{
				StartCoroutine(OnUpdateResource());
				return; //文件已经解压过了，自己可添加检查文件列表逻辑
			}

			StartCoroutine(OnExtractResource()); //启动释放协成
		}

		private IEnumerator OnExtractResource()
		{
			var dataPath = Util.DataPath; //数据目录
			var resPath = Util.AppContentPath(); //游戏包资源目录

			if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
			Directory.CreateDirectory(dataPath);

			var infile = resPath + "files.txt";
			var outfile = dataPath + "files.txt";
			if (File.Exists(outfile)) File.Delete(outfile);

			var message = "正在解包文件:>files.txt";
			Debug.Log(infile);
			Debug.Log(outfile);
			if (Application.platform == RuntimePlatform.Android)
			{
				var www = new WWW(infile);
				yield return www;

				if (www.isDone)
				{
					File.WriteAllBytes(outfile, www.bytes);
				}

				yield return 0;
			}
			else File.Copy(infile, outfile, true);

			yield return new WaitForEndOfFrame();

			//释放所有文件到数据目录
			var files = File.ReadAllLines(outfile);
			foreach (var file in files)
			{
				var fs = file.Split('|');
				infile = resPath + fs[0]; //
				outfile = dataPath + fs[0];

				message = "正在解包文件:>" + fs[0];
				Debug.Log("正在解包文件:>" + infile);
				facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

				var dir = Path.GetDirectoryName(outfile);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				if (Application.platform == RuntimePlatform.Android)
				{
					var www = new WWW(infile);
					yield return www;

					if (www.isDone)
					{
						File.WriteAllBytes(outfile, www.bytes);
					}

					yield return 0;
				}
				else
				{
					if (File.Exists(outfile))
					{
						File.Delete(outfile);
					}

					File.Copy(infile, outfile, true);
				}

				yield return new WaitForEndOfFrame();
			}

			message = "解包完成!!!";
			facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
			yield return new WaitForSeconds(0.1f);

			message = string.Empty;
			//释放完成，开始启动更新资源
			StartCoroutine(OnUpdateResource());
		}

		/// <summary>
		/// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
		/// </summary>
		private IEnumerator OnUpdateResource()
		{
			if (!AppConst.UpdateMode)
			{
				OnResourceInited();
				yield break;
			}

			var dataPath = Util.DataPath; //数据目录
			var url = AppConst.WebUrl;
			var message = string.Empty;
			var random = DateTime.Now.ToString("yyyymmddhhmmss");
			var listUrl = url + "files.txt?v=" + random;
			Debug.LogWarning("LoadUpdate---->>>" + listUrl);

			var www = new WWW(listUrl);
			yield return www;
			if (www.error != null)
			{
				OnUpdateFailed(string.Empty);
				yield break;
			}

			if (!Directory.Exists(dataPath))
			{
				Directory.CreateDirectory(dataPath);
			}

			File.WriteAllBytes(dataPath + "files.txt", www.bytes);
			var filesText = www.text;
			var files = filesText.Split('\n');

			for (var i = 0; i < files.Length; i++)
			{
				if (string.IsNullOrEmpty(files[i])) continue;
				var keyValue = files[i].Split('|');
				var f = keyValue[0];
				var localfile = (dataPath + f).Trim();
				var path = Path.GetDirectoryName(localfile);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				var fileUrl = url + f + "?v=" + random;
				var canUpdate = !File.Exists(localfile);
				if (!canUpdate)
				{
					var remoteMd5 = keyValue[1].Trim();
					var localMd5 = Util.md5file(localfile);
					canUpdate = !remoteMd5.Equals(localMd5);
					if (canUpdate) File.Delete(localfile);
				}

				if (canUpdate)
				{
					//本地缺少文件
					Debug.Log(fileUrl);
					message = "downloading>>" + fileUrl;
					facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
					/*
					www = new WWW(fileUrl); yield return www;
					if (www.error != null) {
					    OnUpdateFailed(path);   //
					    yield break;
					}
					File.WriteAllBytes(localfile, www.bytes);
					 */
					//这里都是资源文件，用线程下载
					BeginDownload(fileUrl, localfile);
					while (!(IsDownOK(localfile)))
					{
						yield return new WaitForEndOfFrame();
					}
				}
			}

			yield return new WaitForEndOfFrame();

			message = "更新完成!!";
			facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

			OnResourceInited();
		}

		private void OnUpdateFailed(string file)
		{
			var message = "更新失败!>" + file;
			facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
		}

		/// <summary>
		/// 是否下载完成
		/// </summary>
		private bool IsDownOK(string file)
		{
			return downloadFiles.Contains(file);
		}

		/// <summary>
		/// 线程下载
		/// </summary>
		private void BeginDownload(string url, string file)
		{
			//线程下载
			var param = new object[2] {url, file};

			var ev = new ThreadEvent();
			ev.Key = NotiConst.UPDATE_DOWNLOAD;
			ev.evParams.AddRange(param);
			ThreadManager.AddEvent(ev, OnThreadCompleted); //线程下载
		}

		/// <summary>
		/// 线程完成
		/// </summary>
		/// <param name="data"></param>
		private void OnThreadCompleted(NotiData data)
		{
			switch (data.evName)
			{
				case NotiConst.UPDATE_EXTRACT: //解压一个完成
					//
					break;
				case NotiConst.UPDATE_DOWNLOAD: //下载一个完成
					downloadFiles.Add(data.evParam.ToString());
					break;
			}
		}

		/// <summary>
		/// 资源初始化结束
		/// </summary>
		public void OnResourceInited()
		{
#if ASYNC_MODE
			ResManager.Initialize(AppConst.AssetDir, delegate()
			{
				Debug.Log("Initialize OK!!!");
				this.OnInitialize();
			});
#else
			// ResManager.Initialize();
			OnInitialize();
#endif
		}

		private void OnInitialize()
		{
			LuaManager.InitStart(); //加载Main.lua
#if UNITY_EDITOR
			LuaManager.SetGlobalBoolean("UNITY_EDITOR", true);
#endif
			LuaManager.DoFile("Logic/Game"); //加载Game.lua
			// LuaManager.DoFile("Logic/Network"); //加载Network.lua
			// NetManager.OnInit(); //初始化网络
			Util.CallMethod("Game", "Start"); //初始化游戏

			initialize = true;
		}

		/// <summary>
		/// 析构函数
		/// </summary>
		private void OnDestroy()
		{
			Util.CallMethod("Game", "OnDestroy");

			// if (NetManager != null)
			// {
			// 	NetManager.Unload();
			// }

			if (LuaManager != null)
			{
				LuaManager.Close();
			}
		}
	}
}