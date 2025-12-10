using Game.Data;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Character
{
	[Serializable]
	public class CharacterSetup
	{
		[SerializeField]
		private CharacterInfoData characterInfo;
		[SerializeField]
		private CharacterEquipmentSetup equipmentData;

		public CharacterInfoData CharacterInfo => characterInfo;
		public CharacterEquipmentSetup EquipmentData => equipmentData;

		public CharacterSetup(CharacterInfoData actorInfo)
		{
			characterInfo = actorInfo;
			equipmentData = new CharacterEquipmentSetup();
		}
	}
}
