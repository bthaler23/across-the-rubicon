using Game.Character;
using Game.UI;
using GamePlugins.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyWordInfoUI : BaseCollectionElementUI<KeywordLogic>
{
	[SerializeField]
	private TextMeshProUGUI countLabel;
	[SerializeField]
	private Image itemIcon;

	public override void Show(KeywordLogic data, Action<object> onSelected)
	{
		base.Show(data, onSelected);
		if (data != null)
		{
			countLabel.SetTextSafe(data.stackCount.ToString());
			itemIcon.SetIconSafe(data.keyword.Icon);
		}
	}
}
