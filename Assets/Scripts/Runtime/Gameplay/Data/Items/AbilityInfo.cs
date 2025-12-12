using Game.Data;
using Game.Gameplay;
using Game.Stats;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static Game.Data.AbilityInfo;

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

		public enum ValueModifierType
		{
			Additional = 0,
			Override = 1,
			Multiply = 2
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
		[Multiline]
		private string description;
		[SerializeField]
		private bool overrideWithWeaponIcon;
		[SerializeField]
		private TurnActionBase actionPrefab;
		[SerializeField]
		private ExecutionType executionType;
		[SerializeField]
		private TargetCategory target;
		[SerializeField]
		[ShowIf("@this.target!=TargetCategory.None")]
		private TargetCategoryMode targetCategoryMode;

		[HorizontalGroup("Range")]
		[ShowIf("@this.executionType==ExecutionType.GridSelect")]
		[SerializeField]
		private int range;
		[HideLabel]
		[HorizontalGroup("Range")]
		[ShowIf("@this.executionType==ExecutionType.GridSelect")]
		[SerializeField]
		private ValueModifierType rangeBehaviour;

		[Title("Cost")]
		[SerializeField]
		private bool consumeAllMana;
		[ShowIf("@this.consumeAllMana==false")]
		[SerializeField]
		private int manaCost;
		[SerializeField]
		private KeywordAllocation[] keywordCost;

		[BoxGroup("Keywords")]
		[SerializeField]
		private KeywordAllocation[] selfKeywordAllocations;
		[BoxGroup("Keywords")]
		[SerializeField]
		private KeywordAllocation[] keywordAllocations;

		[BoxGroup("Attack")]
		[SerializeField]
		private bool dealSelfAttack;
		[SerializeField]
		[BoxGroup("Attack")]
		[ShowIf("@this.dealSelfAttack==true")]
		private AttackDefinition selfAttack;
		[BoxGroup("Attack")]
		[SerializeField]
		private bool dealAttack;
		[BoxGroup("Attack")]
		[ShowIf("@this.dealAttack==true")]
		[SerializeField]
		private AttackDefinition attacks;

		[BoxGroup("Heal")]
		[SerializeField]
		private bool healSelf;
		[ShowIf("@this.healSelf==true")]
		private int healSealfAmount;

		[BoxGroup("Ability Modifiers")]
		[SerializeField]
		private StatModifierInfo[] statModifiers;

		public string Description { get => description; }
		public Color Color { get => color; }
		public TurnActionBase ActionPrefab { get => actionPrefab; }
		public ExecutionType ExecutionMode { get => executionType; }
		public DescriptionType DescriptionOption { get => descriptionType; }
		public int Range { get => range; }
		public ValueModifierType RangeBehaviourType { get => rangeBehaviour; }
		public KeywordAllocation[] KeywordAllocations { get => keywordAllocations; }
		public KeywordAllocation[] SelfkeywordAllocations { get => selfKeywordAllocations; }
		public bool OverrideWithWeaponIcon { get => overrideWithWeaponIcon; }
		public bool ConsumeAllMana { get => consumeAllMana; }
		public int ManaCost { get => manaCost; }
		public KeywordAllocation[] KeywordCost { get => keywordCost; }
		public TargetCategory Target { get => target; }
		public TargetCategoryMode TargetCategoryMode { get => targetCategoryMode; }
		public bool DealSelfAttack { get => dealSelfAttack; }
		public AttackDefinition Attack { get => attacks; }
		public bool DealAttack { get => dealAttack; }
		public AttackDefinition SelfAttack { get => selfAttack; }
		public bool HealSelf { get => healSelf; }
		public int HealSelfAmount { get => healSealfAmount; }
		public StatModifierInfo[] StatModifiers { get => statModifiers; }

		private bool ShowIconInfo()
		{
			return Icon == null;
		}
	}

	public enum TargetCategoryMode
	{
		Single,
		All,
	}

	public enum TargetCategory
	{
		None = 0,
		Any,
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

		public KeywordInfo Keyword { get => keyword; }
		public int Count { get => count; }
	}

	[Serializable]
	public class AttackDefinition
	{
		[SerializeField]
		private ValueModifierType damageModifierType;
		[SerializeField]
		private int attackDamage;
		//not sure if we need this here
		//[HorizontalGroup("Range")]
		//[SerializeField]
		//private ValueModifierType attackRangeModifierType;
		//[HorizontalGroup("Range")]
		//[SerializeField]
		//private int attackRange;

		//public ValueModifierType AttackRangeModifierType { get => attackRangeModifierType; }
		//public int AttackRange { get => attackRange; }
		public ValueModifierType DamageModifierType { get => damageModifierType; }
		public int AttackDamage { get => attackDamage; }

		internal int CalculateAttackDamage(int damage)
		{
			if (damageModifierType == ValueModifierType.Additional)
			{
				return damage + attackDamage;
			}
			if (damageModifierType == ValueModifierType.Multiply)
			{
				return damage * attackDamage;
			}
			else // OverrideCharacterRange
			{
				return attackDamage;
			}
		}
	}
}
