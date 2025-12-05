using Game.Gameplay;
using UnityEngine;

namespace Game.Gameplay
{
	public class CameraLookAt : MonoBehaviour
	{
		[SerializeField]
		private Camera cam;

		private void Update()
		{
			var gameplayCamera = cam;
			transform.LookAt(gameplayCamera.transform, Vector3.up);
		}
	}
}
