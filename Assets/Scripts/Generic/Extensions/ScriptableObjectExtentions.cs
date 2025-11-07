using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamePlugins.Utils
{
	public static class ScriptableObjectExtentions
	{
		const string ResourcesPath = @"Assets/Resources";
		static string lastSaveFolder;
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"> relative to resources folder</param>
		/// <returns></returns>
		public static T LoadResource<T>(string path, bool create = true) where T : ScriptableObject
		{
			T instance = Resources.Load<T>(path);
			path = Path.Combine(ResourcesPath, path);
#if UNITY_EDITOR
			if (!instance && !Application.isPlaying)
			{
				instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path + ".asset");
			}
			if (!instance && create)
			{
				// creating asset
				instance = ScriptableObject.CreateInstance<T>();
				UnityEditor.AssetDatabase.CreateAsset(instance, path + ".asset");
				UnityEditor.AssetDatabase.SaveAssets();
				Debug.Log("Created " + path + ".asset");
			}
#endif
			if (!instance && create)
				throw new Exception("Missing <" + typeof(T) + "> Asset!");
			return instance;
		}

#if UNITY_EDITOR
		public static T LoadAsset<T>(string path, bool create = true) where T : ScriptableObject
		{
			if (!path.EndsWith(".asset"))
				path += ".asset";

			T instance = null;
			if (!instance)
			{
				instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
			}
			if (!instance && create)
			{
				// creating asset
				instance = ScriptableObject.CreateInstance<T>();
				UnityEditor.AssetDatabase.CreateAsset(instance, path);
				UnityEditor.AssetDatabase.SaveAssets();
				Debug.Log("Created " + path);
			}
			if (!instance && create)
				throw new Exception("Missing <" + typeof(T) + "> Asset!");
			return instance;
		}
#endif

		public static T CreateAssetAtPath<T>(string assetPath) where T : ScriptableObject
		{
			// Ensure the directory exists
			string directory = Path.GetDirectoryName(assetPath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			// Create the instance
			T asset = ScriptableObject.CreateInstance<T>();

			// Ensure the path has .asset extension
			if (!assetPath.EndsWith(".asset"))
				assetPath += ".asset";

			// Create the asset
			AssetDatabase.CreateAsset(asset, assetPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			// Ping and focus it
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;

			Debug.Log($"Created ScriptableObject at: {assetPath}");
			return asset;
		}

#if UNITY_EDITOR


		public static void SaveExisting<T>(this T obj) where T : ScriptableObject
		{
			EditorUtility.SetDirty(obj);
			AssetDatabase.SaveAssets();
		}

		public static void Save<T>(this T obj, string folderPath, bool saveAs) where T : ScriptableObject
		{
			if (!folderPath.EndsWith("/"))
				folderPath += "/";
			string assetName = folderPath + obj.name + ".asset";
			bool assetExists = AssetDatabase.GetMainAssetTypeAtPath(assetName) != null;

			if (!assetExists || saveAs)
			{
				if (assetExists)
					AssetDatabase.DeleteAsset(assetName);

				assetName = AssetDatabase.GenerateUniqueAssetPath(assetName);
				AssetDatabase.CreateAsset(obj, assetName);
				AssetDatabase.SaveAssets();
				Selection.activeObject = obj;
			}
			else
			{
				SaveExisting(obj);
			}
		}

		public static bool AssetExistsInProject(this ScriptableObject obj)
		{
			AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj.GetHashCode(), out string GUID, out long localID);
			return GUID != "00000000000000000000000000000000";
		}

		public static bool SaveAssetInProject(this ScriptableObject obj, string popupTitle, string popupMessage, string projectPath)
		{
			string filePath = EditorUtility.SaveFilePanelInProject(
						popupTitle,
						obj.name + ".asset",
						"asset",
						popupMessage,
						projectPath);
			if (!string.IsNullOrEmpty(filePath))
			{
				string path = Path.GetDirectoryName(filePath);
				string fileName = Path.GetFileName(filePath);
				Debug.Log("Save Path " + path + " name " + fileName);


				if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
				{
					lastSaveFolder = path;
					path = path.Replace(Application.dataPath, "Assets/");
					fileName = fileName.Replace(".asset", "");
					obj.name = fileName;
					obj.Save(path, true);
					return true;
				}
			}
			return false;
		}

		public static string GetEnvironmentPath()
		{
			if (string.IsNullOrEmpty(lastSaveFolder))
				return Path.Combine(Application.dataPath, "Content/Environment");
			return lastSaveFolder;
		}
#endif
	}
}
