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
		private TextMeshProUGUI actorNameLabel;
		[SerializeField]
		private Image iconImage;
		[SerializeField]
		private Image iconFrameImage;
		[SerializeField]
		private Image healthProgressBarImage;
		[SerializeField]
		private TextMeshProUGUI healthProgressLabel;

		public void Awake()
		{
			EventBus.Subscribe<ActiveActorRefreshEvent>(OnTurnChangeEvent);
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
			}
			else
			{
				gameObject.SetGameObjectActive(false);
			}
		}
	}
}
