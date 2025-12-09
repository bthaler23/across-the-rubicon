using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using CharacterSetupData = Game.Character.CharacterSetupData;

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
		private List<CharacterSetupData> characters;

		public string TeamID { get => teamID; }
		public Color TeamColor { get => teamColor; }
		public IReadOnlyList<CharacterSetupData> Characters { get => characters; }

		public TeamInfo(string teamID, Color teamColor)
		{
			this.teamID = teamID;
			this.teamColor = teamColor;
			this.characters = new List<CharacterSetupData>();
		}

		public void AddCharacter(CharacterSetupData character)
		{
			characters ??= new List<CharacterSetupData>();
			characters.Add(character);
		}
	}
}