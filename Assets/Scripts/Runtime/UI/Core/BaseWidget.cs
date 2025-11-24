using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
	/// <summary>
	/// Base class for all UI widgets.
	/// </summary>
	public abstract class BaseWidget : BaseElementUI
	{
		#region Fields
		bool? internalVisibilityStatus = null;
		bool shouldRefresh = false;
		#endregion

		#region Properties
		public bool IsShowingInternal
		{
			get
			{
				if (internalVisibilityStatus.HasValue)
					return internalVisibilityStatus.Value;
				return IsVisible();
			}
		}

		protected bool? InternalVisibilityStatus => internalVisibilityStatus;
		#endregion

		#region Mono Behaviour
		protected virtual void Update()
		{
			if (!isInitialized)
				return;

			if (shouldRefresh)
			{
				shouldRefresh = false;
				RefreshActual();
			}
		}
		#endregion

		#region Methods
		protected override void InitializeActual()
		{
			base.InitializeActual();
			internalVisibilityStatus = null;
			shouldRefresh = false;
		}

		public sealed override void Show()
		{
			if (internalVisibilityStatus.HasValue)
			{
				internalVisibilityStatus = true;
				return;
			}

			if (isInitialized && IsVisible())
				return;

			ShowActual();
		}

		public sealed override void Hide()
		{
			if (internalVisibilityStatus.HasValue)
			{
				internalVisibilityStatus = false;
				return;
			}

			if (!IsVisible())
				return;

			HideActual();
		}

		public sealed override void Refresh()
		{
			shouldRefresh = true;
		}

		public virtual void ShowActual()
		{
			base.Show();
			RefreshActual();
		}

		public virtual void HideActual()
		{
			base.Hide();
		}

		public virtual void RefreshActual()
		{
		}

		public override bool IsVisible()
		{
			return gameObject.activeSelf;
		}

		public void MarkInternalVisibility(bool? value)
		{
			internalVisibilityStatus = value;
		}
		#endregion
	}
}
