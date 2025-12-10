using Game.Character;
using Game.Data;
using Game.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Game.Data
{
	[CreateAssetMenu(fileName = "AttackModifierKeyword", menuName = "Rubicon/Items/AttackModifierKeyword")]
	public class AttackModifierKeyword : KeywordInfo
	{
		[Title("Params")]
		[SerializeField]
		private float dealtDMGModifier;
		[SerializeField]
		private float receivedDMGModifier;

		public override KeywordLogic GetLogic()
		{
			return new AttackModifierKLogic(dealtDMGModifier, receivedDMGModifier, this);
		}
	}
}