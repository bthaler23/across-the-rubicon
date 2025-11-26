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
		private Sprite characerAvatar;
		[SerializeField]
		private Sprite characterFullAvatarSprite;
		[SerializeField]
		private ActorController characterPrefab;

		[SerializeField]
		private int health;
		[SerializeField]
		private int damage;
		[SerializeField]
		private int attackRange;
		[SerializeField]
		private int movementRange;

		public ActorController CharacterPrefab => characterPrefab;
		public int MovementRange => movementRange;
		public Sprite CharacterAvatar { get => characerAvatar; }
		public Sprite CharacterIconSprite { get => characterFullAvatarSprite; }
		public int AttackRange { get => attackRange; }
		public int Health { get => health; }
		public int Damage { get => damage; }
		public string CharaterName { get => charaterName; }
		public string CharaterDescription { get => charaterDescription; }
		public string CharaterAbilityDescription { get => charaterAbilityDescription; }
	}
}
