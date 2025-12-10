using Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	[Serializable]
	public struct WeaponSetup
	{
		[SerializeField]
		internal WeaponInfo weapon;
		[SerializeField]
		internal List<MatterInfo> matters;

		public WeaponSetup(WeaponInfo weapon, MatterInfo[] matters)
		{
			this.weapon = weapon;
			this.matters = new List<MatterInfo>();
			for(int i = 0; i < matters.Length && i < weapon.MatterSlots; i++)
			{
				this.matters.Add(matters[i]);
			}	
		}

		internal bool IsEmpty()
		{
			return weapon == null;
		}
	}
}
