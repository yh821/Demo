using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	public class LuaSrcDiet
	{
		[MenuItem("Tools/Lua/CheckLocal %q")]
		public static void CheckLocal()
		{
			CheckLocalVar(2);
		}

		public static void CheckLocalVar(int level)
		{
			var executable = Path.GetFullPath("Tools/lua.exe");
			var workSpace = Path.GetFullPath("Tools/LuaSrcDiet-0.11.2");
			var luaDir = $"{Application.dataPath}/Game/Lua";
			var files = Directory.GetFiles(luaDir, "*.lua", SearchOption.AllDirectories);
			for (int i = 0, len = files.Length; i < len; i++)
			{
				var file = files[i];
				var shortFile = file.Replace(luaDir, "Lua").Replace("\\", "/");
				if (shortFile.StartsWith("Lua/config")) continue;
				var args = $"LuaSrcDiet.lua --plugin unused_local {file} --quiet --params {level}";
				RunProcess(executable, args, workSpace);
				var cancel = EditorUtility.DisplayCancelableProgressBar($"正在检查({i}/{len})", shortFile, (float) i / len);
				if (cancel) break;
			}
			EditorUtility.ClearProgressBar();
		}

		private static void RunProcess(string executable, string argument, string workingDirectory,
			Func<string, bool> filter = null)
		{
			var info = new ProcessStartInfo(executable, argument)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8,
				WorkingDirectory = workingDirectory
			};
			using var proc = Process.Start(info);
			if (proc == null) return;
			proc.OutputDataReceived += (sender, e) =>
			{
				var text = e.Data.Trim();
				if (string.IsNullOrEmpty(text)) return;
				if (filter != null)
				{
					if (filter(text)) UnityEngine.Debug.Log(text);
				}
				else UnityEngine.Debug.Log(text);
			};
			proc.ErrorDataReceived += (sender, e) =>
			{
				var text = e.Data.Trim();
				if (string.IsNullOrEmpty(text)) return;
				if (filter != null)
				{
					if (filter(text)) UnityEngine.Debug.LogError(text);
				}
				else UnityEngine.Debug.LogError(text);
			};
			proc.BeginOutputReadLine();
			proc.WaitForExit();
			proc.Close();
		}
	}
}