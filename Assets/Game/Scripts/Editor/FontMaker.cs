using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	[CreateAssetMenu(fileName = "FontMaker", menuName = "Game/UI/FontMaker")]
	public class FontMaker : ScriptableObject
	{
		[SerializeField] private int maxAtlasSize = 2048;
		[SerializeField] public string atlasName;
		[SerializeField] private int padding;
		[SerializeField] public CustomFont[] fonts;

		internal void Split()
		{
			for (int i = 0; i < fonts.Length; i++)
			{
				var fontMaker = CreateInstance<FontMaker>();
				fontMaker.atlasName = fonts[i].FontName;
				fontMaker.padding = 1;
				fontMaker.fonts = new[]
				{
					new CustomFont
					{
						FontName = fontMaker.atlasName,
						Glyphs = fonts[i].Glyphs
					}
				};
				var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this)) + "/" + fontMaker.atlasName;
				if (AssetDatabase.IsValidFolder(path))
					AssetDatabase.CreateAsset(fontMaker, $"{path}/{fontMaker.atlasName}.asset");
				else
					Debug.LogError("文件夹不存在 " + path);
			}
		}

		public void Build()
		{
			using var progress = new ProgressIndicator("Create Font");
			Build(progress);
		}

		private void Build(ProgressIndicator progress)
		{
			progress.Show("Process font image...");
			var texture2DList = new List<Texture2D>();
			foreach (var font in fonts)
			{
				foreach (var glyph in font.Glyphs)
				{
					if (glyph.Image == null)
					{
						Debug.LogError($"The font {font.FontName} wit graph: {glyph.Code} is missing texture.");
						continue;
					}
					var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(glyph.Image)) as TextureImporter;
					if (importer == null) continue;
					var isDirty = false;
					if (!importer.isReadable)
					{
						importer.isReadable = true;
						isDirty = true;
					}
					if (importer.textureCompression != TextureImporterCompression.Uncompressed)
					{
						importer.textureCompression = TextureImporterCompression.Uncompressed;
						isDirty = true;
					}
					if (importer.mipmapEnabled)
					{
						importer.mipmapEnabled = false;
						isDirty = true;
					}
					if (importer.npotScale != TextureImporterNPOTScale.None)
					{
						importer.npotScale = TextureImporterNPOTScale.None;
						isDirty = true;
					}
					if (isDirty)
					{
						importer.hideFlags = HideFlags.NotEditable;
						importer.SaveAndReimport();
					}
					texture2DList.Add(glyph.Image);
				}
			}
			progress.SetProgress(0.25f);
			progress.Show("Build font atlas...");
			var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false);
			tex.name = "Font Atlas";
			var rectArray = tex.PackTextures(texture2DList.ToArray(), padding, maxAtlasSize, false);
			var directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
			var atlasPath = Path.Combine(directoryName, atlasName + ".png");
			var png = tex.EncodeToPNG();
			var fileStream = File.OpenWrite(atlasPath);
			fileStream.Write(png, 0, png.Length);
			fileStream.Close();
			AssetDatabase.Refresh();
			var atlasImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
			if (atlasImporter != null)
			{
				var isDirty = false;
				if (atlasImporter.isReadable)
				{
					atlasImporter.isReadable = false;
					isDirty = true;
				}
				if (!atlasImporter.alphaIsTransparency)
				{
					atlasImporter.alphaIsTransparency = true;
					isDirty = true;
				}
				if (isDirty) atlasImporter.SaveAndReimport();
			}
			progress.SetProgress(0.35f);
			progress.Show("Build font material...");
			var matPath = Path.Combine(directoryName, atlasName + ".mat");
			var material = AssetDatabase.LoadAssetAtPath<Material>(matPath);
			if (material == null)
			{
				material = new Material(Shader.Find("UI/Default"));
				AssetDatabase.CreateAsset(material, matPath);
			}
			material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(atlasPath);
			EditorUtility.SetDirty(material);
			AssetDatabase.SaveAssets();
			progress.SetProgress(0.7f);
			progress.Show("Build font settings...");
			var num1 = 0;
			foreach (var font in fonts)
			{
				var glyphs = font.Glyphs;
				if (glyphs.Length != 0)
				{
					var fontName = font.FontName + ".fontsettings";
					var fontPath = Path.Combine(directoryName, fontName);
					var font2 = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
					if (font2 == null)
					{
						font2 = new Font();
						AssetDatabase.CreateAsset(font2, fontPath);
					}
					var num2 = 0;
					var characterInfos = new CharacterInfo[glyphs.Length];
					for (int i = 0; i < glyphs.Length; i++)
					{
						var glyph = glyphs[i];
						var image = glyph.Image;
						var rect = rectArray[num1++];
						if (num2 < image.height)
							num2 = image.height;
						var characterInfo = new CharacterInfo();
						characterInfo.index = glyph.Code;
						var x = rect.x;
						var y = rect.y;
						var w = rect.width;
						var h = rect.height;
						characterInfo.uvBottomLeft = new Vector2(x, y);
						characterInfo.uvBottomRight = new Vector2(x + w, y);
						characterInfo.uvTopLeft = new Vector2(x, y + h);
						characterInfo.uvTopRight = new Vector2(x + w, y + h);
						characterInfo.minX = 0;
						characterInfo.minY = -image.height;
						characterInfo.maxX = image.width;
						characterInfo.maxY = 0;
						characterInfo.advance = image.width;
						characterInfos[i] = characterInfo;
					}
					font2.characterInfo = characterInfos;
					font2.material = material;
					var serializedObj = new SerializedObject(font2);
					serializedObj.Update();
					serializedObj.FindProperty("m_LineSpacing").floatValue = num2;
					serializedObj.ApplyModifiedPropertiesWithoutUndo();
					EditorUtility.SetDirty(font2);
				}
			}
			progress.SetProgress(0.95f);
			progress.Show("Save and refresh assets");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[Serializable]
		public class CustomFont
		{
			[SerializeField] private string fontName;
			[SerializeField] private CustomGlyph[] glyphs;

			public string FontName
			{
				get => fontName;
				set => fontName = value;
			}

			public CustomGlyph[] Glyphs
			{
				get => glyphs;
				set => glyphs = value;
			}
		}

		[Serializable]
		public class CustomGlyph
		{
			[SerializeField] private int code;
			[SerializeField] private Texture2D image;

			public int Code => code;

			public Texture2D Image => image;
		}
	}
}