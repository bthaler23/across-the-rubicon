using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Rubicon/CharacterInfo")]
	public class ActorInfo : ScriptableObject
	{
		[SerializeField]
		private string charaterName;
		[SerializeField]
		private Sprite characterSprite;
		[SerializeField]
		private ActorController characterPrefab;

		[SerializeField]
		private int health;
		[SerializeField]
		private int attackRange;
		[SerializeField]
		private int movementRange;

		public ActorController CharacterPrefab => characterPrefab;
		public int MovementRange => movementRange;
		public Sprite CharacterSprite { get => characterSprite; }
		public int AttackRange { get => attackRange; }
	}
}
