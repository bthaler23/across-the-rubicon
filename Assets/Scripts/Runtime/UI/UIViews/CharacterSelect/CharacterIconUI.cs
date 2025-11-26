using Game.Data;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.CharacterSelect
{
	public class CharacterIconUI : BaseCollectionElementUI<ActorInfo>
	{
		[SerializeField]
		public GameObject selectionGO;
		[SerializeField]
		public GameObject chosenGO;
		[SerializeField]
		private Image characterImage;

		public event Action<CharacterIconUI> OnCharacterIconClicked;

		private void HandleButtonClicked()
		{
			OnCharacterIconClicked?.Invoke(this);
		}

		public override void Show(ActorInfo data, Action<object> onSelected)
		{
			base.Show(data, onSelected);
			characterImage.SetIconSafe(data.CharacterAvatar);
			ToggleState(false, false);
		}

		public void ToggleState(bool isChosen, bool isSelected)
		{
			chosenGO.SetGameObjectActive(isChosen);
			selectionGO.SetGameObjectActive(isSelected);
		}
	}
}
