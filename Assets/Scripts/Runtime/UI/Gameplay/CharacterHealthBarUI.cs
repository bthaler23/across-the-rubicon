using Game.Character;
using Game.Gameplay;
using Game.Stats;
using GamePlugins.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class CharacterHealthBarUI : MonoBehaviour
	{
		[SerializeField]
		private Image healthProgressBarImage;
		[SerializeField]
		private TextMeshProUGUI healthProgressLabel;

		private CharacterBehaviour owner;

		public void Initialize(CharacterBehaviour characterBehaviour)
		{
			this.owner = characterBehaviour;
			ShowHealth(characterBehaviour);
			characterBehaviour.OnHealthChanged += OnHealthChanged;
		}

		private void OnHealthChanged()
		{
			ShowHealth(owner);
		}

		private void ShowHealth(CharacterBehaviour actor)
		{
			float health = actor.GetStatValueFloat(StatType.Health);
			float maxhealth = actor.GetStatValueFloat(StatType.MaxHealth);
			float healthPercent = (float)health / (float)maxhealth;
			if (healthProgressBarImage)
				healthProgressBarImage.fillAmount = healthPercent;
			healthProgressLabel.SetTextSafe($"{health} / {maxhealth}");
			gameObject.SetGameObjectActive(actor.IsAlive());
		}
	}
}