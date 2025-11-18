using Game.Events;
using Game.Gameplay;
using GamePlugins.Events;
using GamePlugins.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ActorWidgetUI : MonoBehaviour
	{
		[SerializeField]
		private ActionSelectUI actionSelectUIPrefab;
		[SerializeField]
		private RectTransform actionsParent;
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

		private List<ActionSelectUI> actionSelectUIs;

		public void Awake()
		{
			EventBus.Subscribe<ActiveActorRefreshEvent>(OnTurnChangeEvent);
			actionSelectUIs = new List<ActionSelectUI>();
		}

		private void OnTurnChangeEvent(ActiveActorRefreshEvent eventParams)
		{
			Show(eventParams.active);
		}

		public void Show(ITurnActor actor)
		{
			if (actor != null)
			{
				gameObject.SetGameObjectActive(true);
				iconFrameImage.SetIconColorSafe(actor.GetTeamColor());
				iconImage.SetIconSafe(actor.GetActorIcon());
				healthProgressBarImage.fillAmount = actor.GetHealthNormalized();
				actorNameLabel.SetText(actor.ID);
				PopulateActionItems(actor);
			}
			else
			{
				gameObject.SetGameObjectActive(false);
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
		}

		private ActionSelectUI InstantiateTurnActorWidget()
		{
			var instance = Instantiate(actionSelectUIPrefab, actionsParent);
			actionSelectUIs.Add(instance);
			return instance;
		}
	}
}
