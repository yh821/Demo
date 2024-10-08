using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureAssetImporter : AssetPostprocessor
{
	private static readonly HashSet<string> tempIgnoreAssets = new HashSet<string>();

	private static Dictionary<TextureImporterType, TextureImporterFormat> compressRules =
		new Dictionary<TextureImporterType, TextureImporterFormat>
		{
			{TextureImporterType.NormalMap, TextureImporterFormat.ASTC_8x8}
		};

	private void OnPreprocessTexture()
	{
		var importer = (TextureImporter) assetImporter;
		if (importer.hideFlags == HideFlags.NotEditable) return;
		if (!importer.assetPath.StartsWith("Assets/")) return;
		if (importer.assetPath.StartsWith("Assets/.")) return;
		if (importer.assetPath.Contains("/Editor/")) return;
		if (tempIgnoreAssets.Contains(importer.assetPath)) return;

		PreprocessTextureType(importer);
		PreprocessReadable(importer);
		PreprocessFilterMode(importer);
		PreprocessAdvanced(importer);
		PreprocessPlatform(importer);
	}

	// 规定纹理格式
	private void PreprocessTextureType(TextureImporter importer)
	{
		var path = importer.assetPath.ToLower();
		if (path.Contains(GamePath.RawImageFlag)
		    || path.StartsWith(GamePath.FontAtlasDir))
			importer.textureType = TextureImporterType.Default;
		else if (path.StartsWith(GamePath.IconsDir)
		         || path.StartsWith(GamePath.FontsDir)
		         || path.StartsWith(GamePath.ViewDir))
			importer.textureType = TextureImporterType.Sprite;
	}

	// 规定纹理可读写
	private void PreprocessReadable(TextureImporter importer)
	{
		if (importer.assetPath.StartsWith(GamePath.FontsDir))
			importer.isReadable = true;
		else
			importer.isReadable = false;
	}

	private void PreprocessFilterMode(TextureImporter importer)
	{
		if (importer.textureType == TextureImporterType.Sprite)
			importer.filterMode = FilterMode.Bilinear;
	}

	private void PreprocessAdvanced(TextureImporter importer)
	{
		var path = importer.assetPath.ToLower();
		if (path.Contains(GamePath.RawImageFlag))
		{
			importer.alphaIsTransparency = false;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.wrapMode = TextureWrapMode.Clamp;
		}
	}

	private void PreprocessPlatform(TextureImporter importer)
	{
		if (importer.textureType == TextureImporterType.Sprite)
			return;
		var noCompress = importer.textureType == TextureImporterType.GUI
		                 || importer.textureType == TextureImporterType.SingleChannel;
		var defaultSettings = importer.GetDefaultPlatformTextureSettings();
		if (noCompress || defaultSettings.format == TextureImporterFormat.Alpha8)
		{
			importer.ClearPlatformTextureSettings("Standalone");
			importer.ClearPlatformTextureSettings("Android");
			importer.ClearPlatformTextureSettings("iPhone");
			if (noCompress)
				importer.textureCompression = TextureImporterCompression.Uncompressed;
		}
		else
		{
			importer.textureCompression = TextureImporterCompression.CompressedHQ;
			importer.SetPlatformTextureSettings(defaultSettings);

			var winSettings = importer.GetPlatformTextureSettings("Standalone");
			if (PreprocessUnifyPlatform(importer, winSettings))
				importer.SetPlatformTextureSettings(winSettings);
			var iosSettings = importer.GetPlatformTextureSettings("iPhone");
			if (PreprocessUnifyPlatform(importer, iosSettings) || PreprocessTextureMaxSize(importer, iosSettings))
				importer.SetPlatformTextureSettings(iosSettings);
			var andSettings = importer.GetPlatformTextureSettings("Android");
			if (PreprocessUnifyPlatform(importer, andSettings) || PreprocessTextureMaxSize(importer, andSettings))
				importer.SetPlatformTextureSettings(andSettings);
		}
	}

	private static TextureImporterFormat GetCompressType(TextureImporterType type)
	{
		if (compressRules.TryGetValue(type, out var format))
			return format;
		return TextureImporterFormat.ASTC_6x6;
	}

	private bool PreprocessUnifyPlatform(TextureImporter importer, TextureImporterPlatformSettings settings)
	{
		var format = GetCompressType(importer.textureType);
		return false;
	}

	private bool PreprocessTextureMaxSize(TextureImporter importer, TextureImporterPlatformSettings settings)
	{
		var dirty = importer.textureType == TextureImporterType.NormalMap
		            || importer.assetPath.StartsWith(GamePath.ActorDir);
		if (dirty)
			dirty = settings.maxTextureSize != 1024;
		if (dirty)
			settings.maxTextureSize = 1024;
		return dirty;
	}


	private void OnPostprocessTexture(Texture2D texture)
	{
		var importer = (TextureImporter) assetImporter;
		if (tempIgnoreAssets.Contains(importer.assetPath)) return;
		if (!importer.assetPath.StartsWith("Assets/")) return;
		if (importer.assetPath.StartsWith("Assets/.")) return;

		PostprocessResize2POT(importer, texture);
		ProcessNoPackTips(importer, texture);
	}

	private void PostprocessResize2POT(TextureImporter importer, Texture2D texture)
	{
		if (string.IsNullOrEmpty(importer.spritePackingTag))
			return;
		var w_mod = texture.width % 4;
		var h_mod = texture.height % 4;
		if (w_mod == 0 && h_mod == 0)
			return;
		tempIgnoreAssets.Add(importer.assetPath);
		importer.isReadable = true;
		importer.SaveAndReimport();
		{
			var new_texture = new Texture2D(texture.width + 4 - w_mod, texture.height + 4 - h_mod);
			for (int x = 0, wlen = new_texture.width; x < wlen; x++)
			for (int y = 0, hlen = new_texture.height; y < hlen; y++)
				new_texture.SetPixel(x, y, new Color(0, 0, 0, 0));
			new_texture.SetPixels32(w_mod > 0 ? 1 : 0, h_mod > 0 ? 1 : 0, texture.width, texture.height,
				texture.GetPixels32());
			File.WriteAllBytes(importer.assetPath, new_texture.EncodeToPNG());
		}
		importer.isReadable = false;
		importer.SaveAndReimport();
		tempIgnoreAssets.Remove(importer.assetPath);
	}

	private void ProcessNoPackTips(TextureImporter importer, Texture texture)
	{
		var path = importer.assetPath;
		if (!path.StartsWith(GamePath.UIsDir)
		    || importer.textureType != TextureImporterType.Sprite
		    || path.ToLower().Contains(GamePath.NoPackFlag))
			return;
		if (texture.width > 1024 || texture.height > 1024)
			Debug.LogError($"{path} <color=yellow>图片宽或高大于1024</color>, 请放入带有nopack前缀的文件夹里.");
		else if (texture.width * texture.height > 400 * 400)
			Debug.LogError($"{path} <color=yellow>图片尺寸大于400*400</color>, 请放入带有nopack前缀的文件夹里.");
	}
}