using Game.Character;
using Game.Data;
using GamePlugins.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.CharacterSelect
{
	public class CharacterInfoUI : MonoBehaviour
	{
		[SerializeField]
		private Image characterImage;
		[SerializeField]
		private TextMeshProUGUI nameLabel;
		[SerializeField]
		private TextMeshProUGUI descriptionLabel;
		[SerializeField]
		private TextMeshProUGUI decriptionAbilityLabel;

		public void Show(CharacterInfoData characterData)
		{
			characterImage.SetIconSafe(characterData.CharacterAvatar);
			nameLabel.SetTextSafe(characterData.CharaterName);
			descriptionLabel.SetTextSafe(characterData.CharaterDescription);
			decriptionAbilityLabel.SetTextSafe(characterData.CharaterAbilityDescription);
		}
	}
}
