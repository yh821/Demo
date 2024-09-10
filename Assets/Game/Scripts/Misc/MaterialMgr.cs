using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class MaterialMgr : Singleton<MaterialMgr>
	{
		private struct SyncMaterialItem
		{
			public Material material;
			public string[] excludeKeywords;
			public string[] excludeProperties;
		}

		private const string InstanceSuffix = "(Instance)";
		Dictionary<Renderer, Material[]> sharedMaterialsDic = new Dictionary<Renderer, Material[]>();
		private bool isLimitMaterialClone;
		private readonly bool isNeedSyncMaterial = false;

		private readonly Dictionary<Material, HashSet<SyncMaterialItem>> syncMaterialsDic =
			new Dictionary<Material, HashSet<SyncMaterialItem>>();

		public void OnGameStop()
		{
			syncMaterialsDic.Clear();
			sharedMaterialsDic.Clear();
		}

		public void Update()
		{
			UpdateDelInvalidRender();
#if UNITY_EDITOR
			if (isNeedSyncMaterial) UpdateSyncMaterials();
#endif
		}

		private void UpdateDelInvalidRender()
		{
			using var iter = sharedMaterialsDic.GetEnumerator();
			while (iter.MoveNext())
			{
				if (!iter.Current.Key)
				{
					sharedMaterialsDic.Remove(iter.Current.Key);
					break;
				}
			}
		}

		public static bool IsPbrMaterial(Material material)
		{
			if (material == null || material.shader == null) return false;
			var name = ObjectNameMgr.Instance.GetObjectName(material.shader);
			return name.Contains("Pbr");
		}

		//获得克隆材质球，缓存原始材质球，unity的materials会确保只有一次克隆
		public Material[] GetClonedMaterials(Renderer render)
		{
			if (!render) return new Material[0];
			// if (isLimitMaterialClone) return render.sharedMaterials;
			//特色处理，仅仅使用原始的材质数量，额外添加部分不作为Clone返回
			if (sharedMaterialsDic.TryGetValue(render, out var sourceMaterials))
			{
				var materials = render.sharedMaterials;
				if (materials.Length == sourceMaterials.Length) return materials;
				var cloneMaterials = new Material[Mathf.Min(materials.Length, sourceMaterials.Length)];
				for (var i = 0; i < cloneMaterials.Length; i++)
					cloneMaterials[i] = materials[i];
				return cloneMaterials;
			}
			else
			{
				//要确保sharedMaterials完全是原始的需考虑在此处之前已经通过sharedMaterials克隆过
				var sharedMaterials = render.sharedMaterials;
				sharedMaterialsDic.Add(render, sharedMaterials);
				//克隆
				var cloneMaterials = new Material[sharedMaterials.Length];
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					if (sharedMaterials[i])
						cloneMaterials[i] = CloneMaterial(sharedMaterials[i]);
				}
				render.materials = cloneMaterials;
				return cloneMaterials;
			}
		}

		//获得共享材质球，如果之前没克隆过，则直接通过sharedMaterials拿
		public Material[] GetSharedMaterials(Renderer render)
		{
			if (render == null) return new Material[0];
			if (sharedMaterialsDic.ContainsKey(render)) return sharedMaterialsDic[render];
			return render.sharedMaterials;
		}

		//设置共享材质球，设置之前需清理之前克隆出来的
		private void InternalSetSharedMaterials(Renderer render, Material[] sharedMaterials)
		{
			if (!render) return;
			if (sharedMaterialsDic.TryGetValue(render, out var materialArray))
			{
				var cloneMaterials = render.sharedMaterials;
				var maxCount = Mathf.Min(materialArray.Length, sharedMaterials.Length);
				for (int i = 0; i < maxCount; i++)
				{
					materialArray[i] = sharedMaterials[i];
					//重新克隆
					if (cloneMaterials[i]) Object.Destroy(cloneMaterials[i]);
					cloneMaterials[i] = materialArray[i] ? CloneMaterial(materialArray[i]) : null;
				}
				render.materials = cloneMaterials;
			}
			else render.sharedMaterials = sharedMaterials;
		}

		private static Material CloneMaterial(Material source)
		{
			if (source.name.EndsWith(InstanceSuffix))
			{
				Debug.LogError($"Tried to copy material {source.name} more than once!");
				return source;
			}
			var clone = new Material(source);
			clone.name += InstanceSuffix;
			return clone;
		}

		public void ResumeSharedMaterials(Renderer render, bool dispose = true)
		{
			if (!render) return;
			if (sharedMaterialsDic.TryGetValue(render, out var materials))
			{
				var sharedMaterials = render.sharedMaterials;
				var maxCount = Mathf.Min(sharedMaterials.Length, materials.Length);
				if (dispose)
				{
					for (int i = 0; i < maxCount; i++)
					{
						if (sharedMaterials[i] && sharedMaterials[i] != materials[i])
							Object.Destroy(sharedMaterials[i]);
						render.sharedMaterials = materials;
						sharedMaterialsDic.Remove(render);
					}
				}
				else
				{
					for (int i = 0; i < maxCount; i++)
					{
						if (!materials[i] || !sharedMaterials[i]) continue;
						sharedMaterials[i].CopyPropertiesFromMaterial(materials[i]);
					}
				}
			}
		}

		public void SetSharedMaterials(Renderer render, Material[] sharedMaterials)
		{
			if (!render || sharedMaterials == null || sharedMaterials.Length <= 0) return;
			InternalSetSharedMaterials(render, sharedMaterials);
		}

		public void ResumeMaterialsKeywordsAndRenderQueue(Renderer render)
		{
			if (!render) return;
			if (sharedMaterialsDic.TryGetValue(render, out var sourceMats))
			{
				var renderMats = render.sharedMaterials;
				var maxCount = Mathf.Min(sourceMats.Length, renderMats.Length);
				for (int i = 0; i < maxCount; i++)
				{
					var renderMat = renderMats[i];
					var sourceMat = sourceMats[i];
					if (renderMat && sourceMat)
					{
#if UNITY_EDITOR || UNITY_STANDALONE
						var name1 = renderMat.name;
						var name2 = sourceMat.name;
						if (name1 != $"{name2} {InstanceSuffix}")
							Debug.LogError(
								$"[MaterialMgr] Big Bug!!! ResumeMaterialsKeywordsAndRenderQueue error, {name1}, {name2}");
#endif
						renderMat.renderQueue = sourceMat.renderQueue;
						renderMat.shaderKeywords = sourceMat.shaderKeywords;
					}
				}
			}
		}

		public Material GetClonedMaterial(Renderer render)
		{
			if (!render) return null;
			// if (isLimitMaterialClone) return render.sharedMaterials;
			//特色处理，仅仅使用原始的材质数量，额外添加部分不作为Clone返回
			if (!sharedMaterialsDic.ContainsKey(render))
			{
				//要确保sharedMaterials完全是原始的需考虑在此处之前已经通过sharedMaterials克隆过
				var sharedMaterials = render.sharedMaterials;
				sharedMaterialsDic.Add(render, sharedMaterials);
				//克隆
				var cloneMaterials = new Material[sharedMaterials.Length];
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					if (sharedMaterials[i])
						cloneMaterials[i] = CloneMaterial(sharedMaterials[i]);
				}
				render.materials = cloneMaterials;
			}
			return render.sharedMaterial;
		}

		public Material GetSharedMaterial(Renderer render)
		{
			if (!render) return null;
			if (sharedMaterialsDic.TryGetValue(render, out var materials))
				return materials.Length > 0 ? materials[0] : null;
			return render.sharedMaterial;
		}

		#region SyncMaterial

		public bool IsNeedSyncMaterial()
		{
			return isNeedSyncMaterial;
		}

		private void UpdateSyncMaterials()
		{
			foreach (var kv in syncMaterialsDic)
			{
				var oldMat = kv.Key;
				if (oldMat == null) continue;
				foreach (var item in kv.Value)
					SyncMaterialProperties(oldMat, item.material, item.excludeProperties, item.excludeKeywords);
			}
		}

		private void SyncMaterialProperties(Material oldMat, Material newMat, string[] excludeProperties,
			string[] excludeKeywords)
		{
			var oldRenderQueue = newMat.renderQueue;

			//过滤出不拷贝的属性
			var keepVector4Properties = new Dictionary<string, Vector4>();
			foreach (var property in excludeProperties)
			{
				if (newMat.HasProperty(property))
				{
					var value = newMat.GetVector(property);
					keepVector4Properties.Add(property, value);
				}
			}

			//过滤要拷贝的keyword
			var keepKeywordSet = new HashSet<string>(oldMat.shaderKeywords);
			foreach (var keyword in excludeKeywords)
			{
				if (newMat.IsKeywordEnabled(keyword))
					keepKeywordSet.Add(keyword);
				else
					keepKeywordSet.Remove(keyword);
			}

			//同步所有属性(包括keyword)
			newMat.CopyPropertiesFromMaterial(oldMat);

			//恢复过滤的属性
			foreach (var (key, value) in keepVector4Properties)
				newMat.SetVector(key, value);
			newMat.renderQueue = oldRenderQueue;

			//恢复过滤的keyword
			newMat.shaderKeywords = new string[0];
			foreach (var keyword in keepKeywordSet)
				newMat.EnableKeyword(keyword);
		}

		public void RegisterSyncMaterials(Renderer render, string[] excludeKeywords, string[] excludeProperties) { }

		#endregion
	}
}