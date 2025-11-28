using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
		private List<ActorInfo> characters;

		public string TeamID { get => teamID; }
		public Color TeamColor { get => teamColor; }
		public IReadOnlyList<ActorInfo> Characters { get => characters; }

		public void AddCharacter(ActorInfo character)
		{
			characters ??= new List<ActorInfo>();
			characters.Add(character);
		}
	}
}