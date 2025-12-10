using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using CharacterSetup = Game.Character.CharacterSetup;

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
		private List<CharacterSetup> characters;

		public string TeamID { get => teamID; }
		public Color TeamColor { get => teamColor; }
		public IReadOnlyList<CharacterSetup> Characters { get => characters; }

		public TeamInfo(string teamID, Color teamColor)
		{
			this.teamID = teamID;
			this.teamColor = teamColor;
			this.characters = new List<CharacterSetup>();
		}

		public void AddCharacter(CharacterSetup character)
		{
			characters ??= new List<CharacterSetup>();
			characters.Add(character);
		}
	}
}