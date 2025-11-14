using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
	[Serializable]
	public class TeamInfo
	{
		[SerializeField]
		private string teamID;
		[SerializeField]
		private Color teamColor;
		[SerializeField]
		private List<Character.CharacterInfo> characters;

		public string TeamID { get => teamID; }
		public Color TeamColor { get => teamColor; }
		public IReadOnlyList<Character.CharacterInfo> Characters { get => characters; }
	}
}