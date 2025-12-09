using Game;
using Game.Gameplay;
using Game.Stats;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class TurnActorWidgetUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject activeTurnGO;
		[SerializeField]
		private Image iconImage;
		[SerializeField]
		private Image iconFrameImage;
		[SerializeField]
		private Image healthProgressBarImage;
		[SerializeField]
		private Image manaProgressBarImage;
		[SerializeField]
		private Image turnMeterProgressBarImage;

		public void Show(ITurnActor actor, bool isActiveTurn)
		{
			gameObject.SetGameObjectActive(true);
			iconImage.SetIconSafe(actor.GetActorIcon());
			activeTurnGO.SetActive(isActiveTurn);
			iconFrameImage.SetIconColorSafe(actor.GetTeamColor());
			ShowHealth(actor);
			ShowTurnMeter(actor);
		}

		private void ShowTurnMeter(ITurnActor actor)
		{
			var turnmanager = ResourceManager.Instance.RequestResource<TurnManager>();
			var turnMeterValue = turnmanager.GetTurnMeter(actor);
			int turnMeterThreshold = turnmanager.TurnMeterThreshold; // This should ideally come from a config or the turn manager
			float healthPercent = (float)turnMeterValue / (float)turnMeterThreshold;
			turnMeterProgressBarImage.fillAmount = healthPercent;
		}

		private void ShowHealth(ITurnActor actor)
		{
			float health = actor.GetStatValueFloat(StatType.Health);
			float maxhealth = actor.GetStatValueFloat(StatType.MaxHealth);
			float healthPercent = (float)health / (float)maxhealth;
			healthProgressBarImage.fillAmount = healthPercent;
		}

		public void Hide()
		{
			gameObject.SetGameObjectActive(false);
		}
	}
}