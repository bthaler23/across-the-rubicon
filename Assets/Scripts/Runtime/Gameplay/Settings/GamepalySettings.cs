using Game.Grid;
using UnityEngine;

namespace Game.Settings
{
	[CreateAssetMenu(fileName = "GamepalySettings", menuName = "Rubicon/GamepalySettings")]
	public class GamepalySettings : ScriptableObject
	{
		[SerializeField]
		private GridTileInstance gridTilePrefab;
	}
}
