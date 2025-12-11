using Game.Data;
using Game.Gameplay;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "ActionInfo", menuName = "Rubicon/ActionInfo")]
	public class AbilityInfo : BaseItemInfo
	{
		public enum DescriptionType
		{
			StaticText,
			DynamicByScript
		}

		public enum RangeBehaviour
		{
			AdditionToCharacterRange,
			OverrideCharacterRange,
		}

		public enum ExecutionType
		{
			GridSelect,
			ButtonPress,
		}
		[SerializeField]
		private Color color;
		[SerializeField]
		private DescriptionType descriptionType;
		[SerializeField]
		[ShowIf("@this.descriptionType==DescriptionType.StaticText")]
		private string description;
		[SerializeField]
		private bool overrideWithWeaponIcon;
		[SerializeField]
		private TurnActionBase actionPrefab;
		[SerializeField]
		private ExecutionType executionType;
		[Title("Range")]
		[SerializeField]
		private int manaCost;
		[Title("Range")]
		[SerializeField]
		private int range;
		[SerializeField]
		private RangeBehaviour rangeBehaviour;

		[SerializeField]
		private KeywordAllocation[] keywordAllocations;

		public string Description { get => description; }
		public Color Color { get => color; }
		public TurnActionBase ActionPrefab { get => actionPrefab; }
		public bool UseUIConfirmation { get => executionType == ExecutionType.ButtonPress; }
		public DescriptionType DescriptionOption { get => descriptionType; }
		public int Range { get => range; }
		public RangeBehaviour RangeBehaviourType { get => rangeBehaviour; }
		public KeywordAllocation[] KeywordAllocations { get => keywordAllocations; }
		public bool OverrideWithWeaponIcon { get => overrideWithWeaponIcon; }

		private bool ShowIconInfo()
		{
			return Icon == null;
		}
	}

	public enum ItemAllocationTarget
	{
		Self,
		Ally,
		Enemy,
	}

	[Serializable]
	public class KeywordAllocation
	{
		[SerializeField]
		private KeywordInfo keyword;
		[SerializeField]
		private int count;
		[SerializeField]
		private ItemAllocationTarget target;

		public KeywordInfo Keyword { get => keyword; }
		public int Count { get => count; }
		public ItemAllocationTarget Target { get => target; }
	}
}
