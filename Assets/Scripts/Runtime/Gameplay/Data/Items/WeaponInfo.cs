using Game.Data;
using Game.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "WeaponInfo", menuName = "Rubicon/Items/WeaponInfo")]
	public class WeaponInfo : BaseItemInfo
	{
		[SerializeField]
		private int matterSlots;
		[Title("Stats")]
		[SerializeField]
		private StatModifierInfo[] statModifiers;

		public StatModifierInfo[] StatModifiers => statModifiers;

		public int MatterSlots { get => matterSlots; }
	}
}
