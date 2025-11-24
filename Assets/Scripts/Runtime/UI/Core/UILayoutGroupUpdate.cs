using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class UILayoutGroupUpdate : UIBehaviour
	{
		[Serializable]
		class UiElementContainer
		{
			public RectTransform transform;
			public HorizontalOrVerticalLayoutGroup layoutGroup;
			public ContentSizeFitter contentSizeFitter;

			public UiElementContainer(RectTransform rectTransform)
			{
				transform = rectTransform;
			}
		}

		#region Fields
		[SerializeField]
		private bool refreshContentSizeFitters = true;

		[SerializeField, ReadOnly, InfoBox("List of all elements that will be updated - Populates at RUNTIME", InfoMessageType.None)]
		private List<UiElementContainer> layoutGroups;

		private bool initialized = false;
		private bool needsRefresh = false;
		private bool skipLayoutRebuild = false;
		private bool disabling = false;
		#endregion

		#region Properties

		#endregion

		#region Events

		#endregion

		#region MonoBehaviour
		protected override void Awake()
		{
			FindLayoutGroupComponentsInChildren();
			skipLayoutRebuild = disabling = needsRefresh = false;
		}

		protected override void OnEnable()
		{
			if (!disabling)
				skipLayoutRebuild = true;
			MarkRefresh();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			disabling = true;
		}

		private void LateUpdate()
		{
			if (needsRefresh)
				Refresh();
			disabling = skipLayoutRebuild = needsRefresh = false;
		}

		public void MarkRefresh()
		{
			needsRefresh = true;
			skipLayoutRebuild = false;
		}

		[Button, ShowIf("@UnityEngine.Application.isPlaying==true")]
		private void Refresh()
		{
			FindLayoutGroupComponentsInChildren();
			if (layoutGroups != null && layoutGroups.Count > 0)
			{
				for (int i = 0; i < layoutGroups.Count; i++)
				{
					var aux = layoutGroups[i];
					ContentSizeFitter csf = aux.contentSizeFitter;
					if (csf)
					{
						csf.enabled = false;
						csf.enabled = true;
						csf.SetLayoutHorizontal();
						csf.SetLayoutVertical();
					}

					HorizontalOrVerticalLayoutGroup lg = aux.layoutGroup;
					if (lg)
					{
						lg.enabled = false;
						lg.enabled = true;
						lg.CalculateLayoutInputVertical();
						lg.CalculateLayoutInputHorizontal();
						lg.SetLayoutHorizontal();
						lg.SetLayoutVertical();
					}
					LayoutRebuilder.MarkLayoutForRebuild(aux.transform);
				}
			}

			if (!skipLayoutRebuild)
				LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

		}
		#endregion

		#region Methods
		//Order: Static > Abstract > Virtual > Override > Simple Methods > Eventhandlers

		[Button, ShowIf("@UnityEngine.Application.isPlaying==true")]
		private void FindLayoutGroupComponentsInChildren()
		{
			if (layoutGroups == null || layoutGroups.Count == 0)
			{
				if (!initialized)
				{
					if (transform is RectTransform)
					{
						layoutGroups = new List<UiElementContainer>();
						FindComponentsRec(transform);
						layoutGroups.Reverse();
					}
					else
					{
						layoutGroups = null;
						Debug.LogError("UILayoutGroupUpdate can only be used on Gameobjects with a RectTransform Component");
					}
				}
			}
			initialized = true;
		}

		[Button, ShowIf("@UnityEngine.Application.isPlaying==true||layoutGroups!=null||layoutGroups.Count>0")]
		private void ClearRefreshContent()
		{
			layoutGroups = null;
			initialized = false;
		}

		private void FindComponentsRec(Transform xForm)
		{
			if (xForm == null)
				return;

			UiElementContainer container = null;

			var lG = xForm.GetComponent<HorizontalOrVerticalLayoutGroup>();
			if (lG != null)
			{
				container = new UiElementContainer(xForm as RectTransform);
				container.layoutGroup = lG;
			}
			/*else*/
			if (refreshContentSizeFitters)
			{
				var csf = xForm.GetComponent<ContentSizeFitter>();
				if (csf != null)
				{
					if (container == null)
						container = new UiElementContainer(xForm as RectTransform);
					container.contentSizeFitter = csf;
				}
			}

			if (container != null)
				layoutGroups.Add(container);

			for (int i = 0; i < xForm.childCount; i++)
			{
				Transform childXForm = xForm.GetChild(i);
				FindComponentsRec(childXForm);
			}
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			MarkRefresh();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			MarkRefresh();
		}

		protected void OnTransformChildrenChanged()
		{
			MarkRefresh();
		}
		#endregion
	}
}
