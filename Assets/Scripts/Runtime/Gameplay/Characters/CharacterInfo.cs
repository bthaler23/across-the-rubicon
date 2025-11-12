using UnityEngine;

namespace Game.Character
{
	[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Rubicon/CharacterInfo")]
	public class CharacterInfo : ScriptableObject
	{
		[SerializeField]
		private string charaterName;
		[SerializeField]
		private Sprite characterSprite;
		[SerializeField]
		private CharacterController characterPrefab;

		[SerializeField]
		private int health;
		[SerializeField]
		private int attack;
		[SerializeField]
		private int movementRange;

		public CharacterController CharacterPrefab => characterPrefab;

		public int MovementRange => movementRange;
	}
}
