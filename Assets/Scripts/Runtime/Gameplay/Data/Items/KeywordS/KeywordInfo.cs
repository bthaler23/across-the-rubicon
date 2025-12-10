using Game.Character;
using Game.Data;
using Game.Gameplay;
using System;
using UnityEngine;


namespace Game.Data
{
	public abstract class KeywordInfo : BaseItemInfo
	{
		[SerializeReference]
		private bool stackable = true;

		public bool IsStackable()
		{
			return stackable;
		}

		public abstract KeywordLogic GetLogic();
	}
}
