using Game.Character;
using Game.Data;
using Game.UI;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.CharacterSelect
{
	public class CharacterSlotUI : BaseCollectionElementUI<CharacterInfoData>
	{
		[SerializeField]
		private Image characterImage;
		[SerializeField]
		private GameObject emptyGO;

		public override void Show(CharacterInfoData data, Action<object> onSelected)
		{
			base.Show(data, onSelected);
			bool isEmpty = data == null;
			characterImage.SetGameObjectActive(!isEmpty);
			emptyGO.SetGameObjectActive(isEmpty);

			if (!isEmpty)
			{
				characterImage.SetIconSafe(data.CharacterAvatar);
			}
		}
	}
}
