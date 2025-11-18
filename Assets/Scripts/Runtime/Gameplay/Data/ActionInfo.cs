using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "ActionInfo", menuName = "Rubicon/ActionInfo")]
	public class ActionInfo : ScriptableObject
	{
		[SerializeField]
		private string actionName;
		[SerializeField]
		private Color color;
		[SerializeField]
		private Sprite icon;

		public string ActionName { get => actionName; }
		public Color Color { get => color; }
		public Sprite Icon { get => icon; }
	}
}
