using Game.Gameplay;
using GamePlugins.Attributes;
using GamePlugins.Singleton;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Game.UI
{
	[DontDestroyOnLoadSingleton]
	public partial class UIManager : Singleton<UIManager>
	{
		[SerializeField]
		private PopupMessageUI popupMessageUI;

		[SerializeField]
		private bool loggingEnabled = false;

		private List<BaseViewUI> uiPages;
		private List<BaseWidget> widgets;

		public bool HasOpenedUI => uiPages.Count > 0;

		public UIManager()
		{
			uiPages = new List<BaseViewUI>();
			widgets = new List<BaseWidget>();
		}

		public BaseViewUI GetFrontPage()
		{
			if (uiPages.Count > 0)
				return uiPages[uiPages.Count - 1];
			return null;
		}

		private void AddPage(BaseViewUI view)
		{
			if (!uiPages.Contains(view))
				uiPages.Add(view);
		}

		private void RemovePage(BaseViewUI view)
		{
			if (uiPages.Contains(view))
				uiPages.Remove(view);
		}

		public void Open(BaseViewUI view)
		{
			LOG($"Open {view} Opened {uiPages.Count}");
			bool isPopup = view.Type == BaseViewUI.ViewType.Popup;
			if (uiPages.Count != 0)
			{
				var frontPage = GetFrontPage();

				if (frontPage == view)
				{
					frontPage.Refresh();
					return;
				}

				if (!isPopup)
				{
					frontPage.Hide();
				}
			}
			if (!isPopup)
				ToggleWidgets(false);
			AddPage(view);
			view.Show();
		}

		public void Close(BaseViewUI view)
		{
			LOG($"Close {view}. Opened {uiPages.Count}");
			if (GetFrontPage() == view)
			{
				ForceCloseFrontView();
			}
			else
			{
				Debug.LogError("The View about to be poped is not on the top! ");
				CloseView(view);
			}
		}

		[Button]
		public void ForceCloseFrontView()
		{
			LOG($"ForceCloseFrontView. Opened {uiPages.Count}");
			if (uiPages.Count > 0)
			{
				var view = GetFrontPage();
				view.Hide();
				RemovePage(view);
				HandlePageClose();
			}
		}

		[Button]
		public void TryCloseFrontView(out bool canClose)
		{
			canClose = true;
			LOG($"CloseFrontView. Opened {uiPages.Count}");
			if (uiPages.Count > 0)
			{
				var view = GetFrontPage();
				if (view.CanClose)
				{
					view.Hide();
					RemovePage(view);
					HandlePageClose();
				}
				else
				{
					canClose = false;
				}
			}
		}

		public void CloseView(BaseViewUI view)
		{
			LOG($"CloseView. {view.name} Opened {uiPages.Count}");
			if (uiPages.Count > 0 && uiPages.Contains(view))
			{
				view.Hide();
				RemovePage(view);
				HandlePageClose();
			}
		}

		private void HandlePageClose()
		{
			if (uiPages.Count != 0)
			{
				var peek = GetFrontPage();
				peek.Show();
			}
			else
			{
				ToggleWidgets(true);
			}
		}

		public void ToggleWidgets(bool show)
		{
			for (int i = 0; i < widgets.Count; i++)
			{
				var widget = widgets[i];
				if (widget != null && widget.gameObject != null)
				{
					if (show)
					{
						bool shouldShow = widget.IsShowingInternal;
						widget.MarkInternalVisibility(null);
						if (shouldShow)
						{
							widget.Show();
						}
					}
					else
					{
						bool visibility = widget.IsShowingInternal;
						widget.Hide();
						widget.MarkInternalVisibility(visibility);
					}
				}
				else
				{
					widgets.RemoveAt(i);
					i--;
				}
			}
		}

		public void RegisterWidgets(BaseWidget widget)
		{
			if (widget != null && widgets != null)
			{
				if (!widgets.Contains(widget))
				{
					if (uiPages.Count != 0)
					{
						bool visibility = widget.IsVisible();
						widget.Hide();
						widget.MarkInternalVisibility(visibility);
					}
					widgets.Add(widget);
				}
			}
		}

		public void ClearUI()
		{
			ClearWidgets();
			ClearPages();
		}

		private void ClearPages()
		{
			foreach (var page in uiPages)
			{
				if (page != null)
				{
					page.Hide();
				}
			}
			uiPages.Clear();
		}

		private void ClearWidgets()
		{
			widgets.Clear();
		}

		public void CloseAll()
		{
			ClearPages();
		}

		private void LOG(string msg)
		{
#if UNITY_EDITOR
			if (loggingEnabled)
				Debug.Log($"[UIManager] {msg}");
#endif
		}

		public void ShowPopupMessage(string title, string message, string buttonA, Action actionA, string buttonB = null, Action actionB = null)
		{
			popupMessageUI.ShowPopup(title, message, buttonA, actionA, buttonB, actionB);
			Open(popupMessageUI);
		}
	}
}
