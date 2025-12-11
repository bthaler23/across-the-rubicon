using Game.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay
{
	public abstract class TurnActionBase : MonoBehaviour
	{
		[SerializeField]
		protected AbilityInfo actionInfo;
		public AbilityInfo ActionInfo { get => actionInfo; }

		public event Action OnActionCompleted;

		public virtual void Initialize(AbilityInfo action, ITurnActor owner)
		{
			actionInfo = action;
		}
		public abstract bool IsActionStarted();

		public abstract bool IsAvailable();

		public abstract void ActivateAction();

		public abstract void DisableAction();

		protected void FireOnCompletedEvent()
		{
			OnActionCompleted?.Invoke();
		}

		public void UIInvokeExecute()
		{
			if (actionInfo.UseUIConfirmation)
				OnExecuteInvoked();
		}

		protected virtual void OnExecuteInvoked()
		{
		}

		public bool UseExecuteButton()
		{
			return actionInfo.UseUIConfirmation;
		}

		public virtual string GetDescription()
		{
			if (actionInfo.DescriptionOption != AbilityInfo.DescriptionType.StaticText)
			{
				Debug.LogError($"Action {actionInfo.name} has dynamic description but script does not override it!");
			}
			return actionInfo.Description;
		}

		public string GetName()
		{
			return actionInfo.ItemName;
		}

		public virtual Sprite GetIcon()
		{
			return actionInfo.Icon;
		}
	}
}
