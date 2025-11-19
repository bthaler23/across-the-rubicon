using Game.Gameplay;
using Game.Stats;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

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

	public void Show(ITurnActor actor, bool isActiveTurn)
	{
		gameObject.SetGameObjectActive(true);
		iconImage.SetIconSafe(actor.GetActorIcon());
		activeTurnGO.SetActive(isActiveTurn);
		iconFrameImage.SetIconColorSafe(actor.GetTeamColor());
		ShowHealth(actor);
	}

	private void ShowHealth(ITurnActor actor)
	{
		var health = actor.GetStatValue(StatType.Health) as HealthStats;
		if (health != null)
		{
			float healthPercent = (float)health.CurrentHealth / (float)health.MaxHealth;
			healthProgressBarImage.fillAmount = healthPercent;
		}
	}

	public void Hide()
	{
		gameObject.SetGameObjectActive(false);
	}
}
