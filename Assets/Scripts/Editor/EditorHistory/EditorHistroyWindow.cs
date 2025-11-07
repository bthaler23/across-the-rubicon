using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.OdinInspector;

namespace GamePlugins.Editor.History
{
	public class EditorHistroyWindow : OdinEditorWindow
	{
		[InlineProperty, HideLabel]
		public EditorSeletionHistory selections;
		public UnityEngine.Object prevSelection = null;

		protected override void OnImGUI()
		{
			UnityEngine.Object currentSelection = Selection.activeObject;
			UnityEngine.Object currentSceneSelection = Selection.activeGameObject;
			if (currentSelection != null && prevSelection != currentSelection)
			{
				if (currentSceneSelection != null && ((GameObject)currentSceneSelection).scene.IsValid())
				{
					if (selections.showSceneSelections)
						selections.AddSceneSelection(currentSelection);
				}
				else
				{
					selections.AddProjectSelection(currentSelection);
				}

				EditorHistory.Settings.histoySettings = selections;

				EditorUtility.SetDirty(EditorHistory.Settings);
				prevSelection = currentSelection;
			}

			base.OnImGUI();
		}

		protected override void Initialize()
		{
			base.Initialize();
			prevSelection = null; ;

			if (EditorHistory.Settings != null)
			{
				if (EditorHistory.Settings.histoySettings == null)
					EditorHistory.Settings.histoySettings = new EditorSeletionHistory();
				selections = EditorHistory.Settings.histoySettings;
				EditorUtility.SetDirty(EditorHistory.Settings);
			}
		}

		[MenuItem("Custom/Editor History")]
		public static void OpenWindow()
		{
			var window = GetWindow<EditorHistroyWindow>();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 800);
		}
	}
}