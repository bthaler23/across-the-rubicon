using Game.Gameplay;
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
		healthProgressBarImage.fillAmount = actor.GetHealthNormalized();
		activeTurnGO.SetActive(isActiveTurn);
		iconFrameImage.SetIconColorSafe(actor.GetTeamColor());
	}

	public void Hide()
	{
		gameObject.SetGameObjectActive(false);
	}
}
