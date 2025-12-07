using Game.Events;
using Game.Gameplay;
using Game.Stats;
using GamePlugins.Events;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ActorWidgetUI : MonoBehaviour
	{
		[Title("Actions")]
		[SerializeField]
		private ActionSelectUI actionSelectUIPrefab;
		[SerializeField]
		private RectTransform actionsParent;
		[SerializeField]
		private TextMeshProUGUI actionNameLabel;
		[SerializeField]
		private TextMeshProUGUI actionDescriptionLabel;
		[SerializeField]
		private Button actionExecuteButton;

		[Title("Character")]
		[SerializeField]
		private TextMeshProUGUI actorNameLabel;
		[SerializeField]
		private Image iconImage;
		[SerializeField]
		private Image iconFrameImage;
		[SerializeField]
		private Image healthProgressBarImage;
		[SerializeField]
		private TextMeshProUGUI healthProgressLabel;

		private TurnActionBase activeAction;
		private List<ActionSelectUI> actionSelectUIs;

		public void Awake()
		{
			EventBus.Subscribe<ActiveActorRefreshEvent>(OnActiveCharacterChange);
			actionSelectUIs = new List<ActionSelectUI>();
			gameObject.SetGameObjectActive(false);
			actionExecuteButton.onClick.AddListener(OnActionExecuteButtonClick);
		}

		private void OnActionExecuteButtonClick()
		{
			if (activeAction)
				activeAction.UIInvokeExecute();
		}

		private void OnActiveCharacterChange(ActiveActorRefreshEvent eventParams)
		{
			if (eventParams.active != null)
				Show(eventParams.active);
			else
				gameObject.SetGameObjectActive(false);
		}

		public void Show(ITurnActor actor)
		{
			if (actor != null)
			{
				gameObject.SetGameObjectActive(true);
				iconFrameImage.SetIconColorSafe(actor.GetTeamColor());
				iconImage.SetIconSafe(actor.GetActorIcon());
				ShowHealth(actor);
				actorNameLabel.SetText(actor.ID);
				PopulateActionItems(actor);
				ShowActiveActionInfo(actor);
			}
			else
			{
				gameObject.SetGameObjectActive(false);
			}
		}

		private void ShowActiveActionInfo(ITurnActor actor)
		{
			activeAction = actor.GetActiveAction();
			actionNameLabel.SetTextSafe(activeAction.GetName());
			actionDescriptionLabel.SetTextSafe(activeAction.GetDescription());
			actionExecuteButton.SetGameObjectActive(activeAction.UseExecuteButton());
		}

		private void ShowHealth(ITurnActor actor)
		{
			var health = actor.GetStat(StatType.Health) as HealthStats;
			if (health != null)
			{
				float healthPercent = (float)health.CurrentHealth / (float)health.MaxHealth;
				healthProgressBarImage.fillAmount = healthPercent;
				healthProgressLabel.SetText($"{health.CurrentHealth} / {health.MaxHealth}");
			}
		}

		private void PopulateActionItems(ITurnActor actor)
		{
			if (actor == null)
			{
				return;
			}

			var actions = actor.GetActions();
			var activeAction = actor.GetActiveAction();
			int neededCount = actions != null ? actions.Count : 0;

			// Ensure we have enough UI widgets
			while (actionSelectUIs.Count < neededCount)
			{
				InstantiateTurnActorWidget();
			}

			// Populate and activate the required widgets
			for (int i = 0; i < neededCount; i++)
			{
				var ui = actionSelectUIs[i];
				var action = actions[i];
				ui.Show(action, activeAction == action,
				() =>
				{
					actor.SetActiveAction(action);
				});
			}

			// Deactivate any extra widgets
			for (int i = neededCount; i < actionSelectUIs.Count; i++)
			{
				actionSelectUIs[i].Hide();
			}

			actionsParent.position = GetScreenPosition(actor.GetUIXform().position);
		}

		private ActionSelectUI InstantiateTurnActorWidget()
		{
			var instance = Instantiate(actionSelectUIPrefab, actionsParent);
			actionSelectUIs.Add(instance);
			return instance;
		}

		protected Vector3 GetScreenPosition(Vector3 worldPosition)
		{
			var cameraController = ResourceManager.Instance.RequestResource<CameraController>();
			if (cameraController)
			{
				Vector3 screenPos = cameraController.Camera.WorldToScreenPoint(worldPosition);
				return screenPos;
			}
			Debug.LogError("No CameraController Present!");
			return Vector3.zero;
		}
	}
}
