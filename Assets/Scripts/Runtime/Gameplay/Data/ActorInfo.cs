using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Rubicon/CharacterInfo")]
	public class ActorInfo : ScriptableObject
	{
		[SerializeField]
		private string charaterName;
		[SerializeField]
		private string charaterDescription;
		[SerializeField]
		private string charaterAbilityDescription;
		[SerializeField]
		[PreviewField(32, ObjectFieldAlignment.Left)]
		private Sprite characerAvatar;
		[SerializeField]
		[PreviewField(64, ObjectFieldAlignment.Left)]
		private Sprite characterFullAvatarSprite;
		[SerializeField]
		private ActorController characterPrefab;

		[Title("Stats")]
		[SerializeField]
		private int health;
		[SerializeField]
		private int manaPool;
		[SerializeField]
		[HorizontalGroup("Attack")]
		private int minAttack;
		[SerializeField]
		[HorizontalGroup("Attack")]
		private int maxAttack;
		[SerializeField]
		private int attackRange;
		[SerializeField]
		private int movementRange;
		[SerializeField]
		private int speed;

		public ActorController CharacterPrefab => characterPrefab;
		public Sprite CharacterAvatar { get => characerAvatar; }
		public Sprite CharacterIconSprite { get => characterFullAvatarSprite; }
		public int MinAttack { get => minAttack; }
		public int MaxAttack { get => maxAttack; }
		public int Health { get => health; }
		public int MovementRange => movementRange;
		public int AttackRange => attackRange;
		public int Speed => speed;
		public string CharaterName { get => charaterName; }
		public string CharaterDescription { get => charaterDescription; }
		public string CharaterAbilityDescription { get => charaterAbilityDescription; }
	}
}
