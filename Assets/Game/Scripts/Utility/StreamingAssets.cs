using System.IO;
using UnityEngine;

namespace Game
{
	public static class StreamingAssets
	{
		public static string ReadAllText(string filePath)
		{
			var path = Path.Combine(Application.streamingAssetsPath, filePath);
			return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
		}

		public static bool Existed(string filePath)
		{
			return File.Exists(Path.Combine(Application.streamingAssetsPath, filePath));
		}
	}
}