using Game.Events;
using GamePlugins.Events;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
	public class TurnWidgetUI : MonoBehaviour
	{
		[SerializeField]
		private TurnActorWidgetUI turnActorWidgetUIPrefab;
		[SerializeField]
		private RectTransform contentParent;

		private List<TurnActorWidgetUI> turnActionUIs;

		public void Awake()
		{
			turnActionUIs = new List<TurnActorWidgetUI>();
			EventBus.Subscribe<TurnChangeEvent>(OnTurnChangeEvent);
		}

		private void OnTurnChangeEvent(TurnChangeEvent turnChangeParam)
		{
			if (turnChangeParam == null)
			{
				return;
			}

			var actors = turnChangeParam.actors;
			int neededCount = actors != null ? actors.Count : 0;

			// Ensure we have enough UI widgets
			while (turnActionUIs.Count < neededCount)
			{
				InstantiateTurnActorWidget();
			}

			// Populate and activate the required widgets
			for (int i = 0; i < neededCount; i++)
			{
				var ui = turnActionUIs[i];
				var actor = actors[i];
				bool isActive = turnChangeParam.selected != null && ReferenceEquals(actor, turnChangeParam.selected);
				ui.Show(actor, isActive);
			}

			// Deactivate any extra widgets
			for (int i = neededCount; i < turnActionUIs.Count; i++)
			{
				turnActionUIs[i].Hide();
			}
		}

		private TurnActorWidgetUI InstantiateTurnActorWidget()
		{
			var instance = Instantiate(turnActorWidgetUIPrefab, contentParent);
			turnActionUIs.Add(instance);
			return instance;
		}
	}
}
