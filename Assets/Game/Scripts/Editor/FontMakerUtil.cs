using System.Collections.Generic;
using System.IO;
using Game.Editor;
using UnityEditor;
using UnityEngine;

public class FontMakerUtil
{
	private const string FontDir = "Assets/Game/UIs/Fonts";
	private const string FontAtlasDir = "Assets/Game/UIs/FontAtlas";

	[MenuItem("Game/Font/生成字体")]
	public static void Build()
	{
		var guids = AssetDatabase.FindAssets("t:fontmaker", new[] {FontDir});
		var textureList = new List<Texture2D>();
		var fontMakerList = new List<FontMaker>();
		var dirList = new List<string>();
		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var fontMaker = AssetDatabase.LoadAssetAtPath<FontMaker>(path);
			if (!FetchAllTexture(fontMaker, textureList)) return;
			dirList.Add(Path.GetDirectoryName(path));
			fontMakerList.Add(fontMaker);
		}

		Rect[] rects;
		Material material;
		CreateMaterial(textureList, out rects, out material);

		var index = 0;
		for (int i = 0; i < fontMakerList.Count; i++)
		{
			var fontPath = $"{dirList[i]}/{fontMakerList[i].atlasName}";
			CreateTTF(fontPath, fontMakerList[i], material, rects, ref index);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private static bool FetchAllTexture(FontMaker fontMaker, List<Texture2D> textureList)
	{
		foreach (var font in fontMaker.fonts)
		{
			foreach (var glyph in font.Glyphs)
			{
				if (glyph.Image == null)
				{
					Debug.LogError($"The font {font.FontName} with graph: {glyph.Code} is missing texture.");
					return false;
				}
				textureList.Add(glyph.Image);
			}
		}
		return true;
	}

	private static void CreateMaterial(List<Texture2D> textureList, out Rect[] rects, out Material material)
	{
		var atlas = new Texture2D(10, 10, TextureFormat.RGBA32, false) {name = "Font Atlas"};
		rects = atlas.PackTextures(textureList.ToArray(), 2);

		var atlasPath = Path.Combine(FontAtlasDir, "FontAtlas.png");
		var matPath = Path.Combine(FontAtlasDir, "FontAtlas.mat");
		var bytes = atlas.EncodeToPNG();
		if (bytes != null)
		{
			var fileStream = File.OpenWrite(atlasPath);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Close();
			AssetDatabase.Refresh();
		}

		var atlasTex = AssetDatabase.LoadAssetAtPath<Texture>(atlasPath);
		material = AssetDatabase.LoadAssetAtPath<Material>(matPath);
		if (material == null)
		{
			material = new Material(Shader.Find("Game/UIDefault"));
			AssetDatabase.CreateAsset(material, matPath);
		}
		material.mainTexture = atlasTex;
		EditorUtility.SetDirty(material);
	}

	private static void CreateTTF(string fontPath, FontMaker fontMaker, Material material, Rect[] rects, ref int index)
	{
		foreach (var font in fontMaker.fonts)
		{
			var glyphs = font.Glyphs;
			if (glyphs.Length == 0) continue;
			var customFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
			if (customFont == null)
			{
				customFont = new Font();
				AssetDatabase.CreateAsset(customFont, fontPath);
			}
			var maxHeight = 0;
			var charInfos = new CharacterInfo[glyphs.Length];
			for (int i = 0; i < glyphs.Length; i++)
			{
				var glyph = glyphs[i];
				var image = glyph.Image;
				var rect = rects[index++];
				if (maxHeight < image.height) maxHeight = image.height;
				var info = new CharacterInfo();
				info.index = glyph.Code;
				var x = rect.x;
				var y = rect.y;
				var w = rect.width;
				var h = rect.height;
				info.uvBottomLeft = new Vector2(x, y);
				info.uvBottomRight = new Vector2(x + w, y);
				info.uvTopLeft = new Vector2(x, y + h);
				info.uvTopRight = new Vector2(x + w, y + h);
				info.minX = 0;
				info.minY = -image.height;
				info.maxX = image.width;
				info.maxY = 0;
				info.advance = image.width;
				charInfos[i] = info;
			}
			customFont.characterInfo = charInfos;
			customFont.material = material;

			var serObj = new SerializedObject(customFont);
			serObj.Update();
			serObj.FindProperty("m_LineSpacing").floatValue = maxHeight;
			serObj.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(customFont);
		}
	}
}