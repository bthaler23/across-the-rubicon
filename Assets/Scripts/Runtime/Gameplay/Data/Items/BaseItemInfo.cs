using Sirenix.OdinInspector;
using UnityEngine;


namespace Game.Data
{
	public abstract class BaseItemInfo : ScriptableObject
	{
		[SerializeField]
		private int ID;
		[SerializeField]
		private string itemName;
		[SerializeField]
		[PreviewField(64, ObjectFieldAlignment.Left)]
		private Sprite icon;

		public Sprite Icon { get => icon; }
		public string ItemName { get => itemName; }
	}
}
