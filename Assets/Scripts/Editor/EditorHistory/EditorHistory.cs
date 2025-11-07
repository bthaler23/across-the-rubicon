using System;
using System.Collections.Generic;
using UnityEngine;
using GamePlugins.Utils;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
using Sirenix.Utilities.Editor;

namespace GamePlugins.Editor.History
{
	public class EditorHistory : ScriptableObject
	{
		#region Fields
		const string SelectionHistoryAssetPath = "Assets/Resources/Editor/EditorHistory";
		static EditorHistory instance;

		[HideInInspector]
		public List<UnityEngine.Object> pinned;
		[HideInInspector]
		public List<UnityEngine.Object> projectSelections;
		[HideInInspector]
		public List<UnityEngine.Object> sceneSelections;
		[HideInInspector]
		public EditorSeletionHistory histoySettings;
		#endregion

		#region Properties
		public static EditorHistory Settings
		{
			get
			{
				if (!instance)
				{
					instance = ScriptableObjectExtentions.LoadAsset<EditorHistory>(SelectionHistoryAssetPath, true);
				}
				return instance;
			}
		}
		#endregion

		#region Methods
		public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
		{
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
			for (int i = 0; i < guids.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
				if (asset != null)
				{
					assets.Add(asset);
				}
			}
			return assets;
		}
		#endregion
	}

	[Serializable]
	public class EditorSeletionHistory
	{
		[ListDrawerSettings(HideAddButton = true, DefaultExpandedState = true, OnBeginListElementGUI = "OnBeginPinnedDraw", OnEndListElementGUI = "OnEndPinnedDraw")]
		public List<UnityEngine.Object> pinned;
		[HorizontalGroup("Selections"), ListDrawerSettings(HideAddButton = true, DefaultExpandedState = true, OnBeginListElementGUI = "OnBeginProjectDraw", OnEndListElementGUI = "OnEndProjectDraw")]
		public List<UnityEngine.Object> projectSelections;
		[HorizontalGroup("Selections"), ShowIf("@this.showSceneSelections==true")]
		[ListDrawerSettings(HideAddButton = true, DefaultExpandedState = true, OnBeginListElementGUI = "OnBeginSceneDraw", OnEndListElementGUI = "OnEndSceneDraw")]
		public List<UnityEngine.Object> sceneSelections;

		[FoldoutGroup("Settings")]
		public int historySize = 20;
		[FoldoutGroup("Settings")]
		public bool showSceneSelections = true;

		[FoldoutGroup("Settings")]
		public Color sceneColor = new Color(252 / 255f, 203 / 255f, 53 / 255f);
		[FoldoutGroup("Settings")]
		public Color projectColor = new Color(195 / 255f, 232 / 255f, 60 / 255f);

		[FoldoutGroup("Settings")]
		[Button(ButtonSizes.Small)]
		public void ClearSelectionHistory()
		{
			projectSelections = new List<UnityEngine.Object>();
			sceneSelections = new List<UnityEngine.Object>();
		}

		[FoldoutGroup("Settings")]
		[Button(ButtonSizes.Small)]
		public void ClearAll()
		{
			ClearSelectionHistory();
			pinned = new List<UnityEngine.Object>();
		}

		public void AddProjectSelection(UnityEngine.Object currentSelection)
		{
			if (projectSelections == null)
				projectSelections = new List<UnityEngine.Object>();

			if (!projectSelections.Contains(currentSelection))
			{
				projectSelections.Insert(0, currentSelection);
			}
			else
			{
				if (projectSelections[0] != currentSelection)
				{
					projectSelections.Remove(currentSelection);
					projectSelections.Insert(0, currentSelection);
				}
			}

			if (projectSelections.Count > historySize)
				projectSelections.RemoveAt(projectSelections.Count - 1);
		}

		public void AddSceneSelection(UnityEngine.Object currentSelection)
		{
			if (sceneSelections == null)
				sceneSelections = new List<UnityEngine.Object>();

			if (!sceneSelections.Contains(currentSelection))
			{
				sceneSelections.Insert(0, currentSelection);
			}
			else
			{
				if (sceneSelections[0] != currentSelection)
				{
					sceneSelections.Remove(currentSelection);
					sceneSelections.Insert(0, currentSelection);
				}
			}

			if (sceneSelections.Count > historySize)
				sceneSelections.RemoveAt(sceneSelections.Count - 1);
		}

		public void OnBeginProjectDraw(int indx)
		{
			SirenixEditorGUI.BeginIndentedHorizontal();
			Color guiColor = GUI.color;
			GUI.color = Color.green;
			if (GUILayout.Button("Sel", GUILayout.Width(28)))
			{
				Selection.objects = new UnityEngine.Object[] { projectSelections[indx] };
			}
			GUI.color = guiColor;

		}

		public void OnEndProjectDraw(int indx)
		{
			Color guiColor = GUI.color;
			GUI.color = projectColor;
			if (GUILayout.Button("Pin", GUILayout.Width(30)))
			{
				if (pinned == null)
					pinned = new List<UnityEngine.Object>();

				pinned.Insert(0, projectSelections[indx]);
			}
			GUI.color = guiColor;
			SirenixEditorGUI.EndIndentedHorizontal();
		}

		public void OnBeginSceneDraw(int indx)
		{
			SirenixEditorGUI.BeginIndentedHorizontal();
		}

		public void OnEndSceneDraw(int indx)
		{
			Color guiColor = GUI.color;
			GUI.color = sceneColor;

			if (GUILayout.Button("Pin", GUILayout.Width(30)))
			{
				if (pinned == null)
					pinned = new List<UnityEngine.Object>();

				pinned.Insert(0, sceneSelections[indx]);
			}

			GUI.color = guiColor;
			SirenixEditorGUI.EndIndentedHorizontal();
		}

		public void OnBeginPinnedDraw(int indx)
		{
			SirenixEditorGUI.BeginIndentedHorizontal();
			if (pinned[indx] != null)
			{
				Color guiColor = GUI.color;
				GUI.color = Color.green;
				if (GUILayout.Button("Sel", GUILayout.Width(28)))
				{
					Selection.objects = new UnityEngine.Object[] { pinned[indx] };
				}
				GUI.color = guiColor;
			}
		}

		public void OnEndPinnedDraw(int indx)
		{
			Color guiColor = GUI.color;
			if (pinned[indx] != null)
			{
				if (pinned[indx] is GameObject)
				{
					GUI.color = sceneColor;
					GUILayout.Button("S", GUILayout.Width(20));
				}
				else
				{
					GUI.color = projectColor;
					GUILayout.Button("P", GUILayout.Width(20));
				}

				GUI.color = guiColor;
			}
			SirenixEditorGUI.EndIndentedHorizontal();
		}

		Color GetProjectColor()
		{
			return projectColor;
		}

		Color GetSceneColor()
		{
			return sceneColor;
		}
	}
#endif
}