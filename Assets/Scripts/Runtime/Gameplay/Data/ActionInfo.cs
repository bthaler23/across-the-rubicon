using Game.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "ActionInfo", menuName = "Rubicon/ActionInfo")]
	public class ActionInfo : ScriptableObject
	{
		public enum DescriptionType
		{
			StaticText,
			DynamicByScript
		}


		[SerializeField]
		private string actionName;
		[SerializeField]
		private DescriptionType descriptionType;
		[SerializeField]
		[ShowIf("@this.descriptionType==DescriptionType.StaticText")]
		private string description;
		[SerializeField]
		private Color color;
		[SerializeField]
		[InfoBox("If icon not set, Action name will be displayed during gameplay", InfoMessageType.None, "ShowIconInfo")]
		private Sprite icon;
		[SerializeField]
		private TurnActionBase actionPrefab;
		[SerializeField]
		[InfoBox("Display 'Use' Button on Action UI ?", InfoMessageType.None)]
		private bool useUIConfirmation;

		public string ActionName { get => actionName; }
		public string Description { get => description; }
		public Color Color { get => color; }
		public Sprite Icon { get => icon; }
		public TurnActionBase ActionPrefab { get => actionPrefab; }
		public bool UseUIConfirmation { get => useUIConfirmation; }
		public DescriptionType DescriptionOption { get => descriptionType; }

		private bool ShowIconInfo()
		{
			return icon == null;
		}

	}
}
