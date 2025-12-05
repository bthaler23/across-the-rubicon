using Game;
using Game.Grid;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour, IResource
{
	[SerializeField]
	private float cameraBoarderPadding = 2f;
	[SerializeField]
	private Camera gameplayCamera;
	[SerializeField]
	private Transform cameraTarget;
	[SerializeField]
	private CinemachineCamera cmGameplayCamera;
	[SerializeField]
	private CinemachineBrain cinemachineBrain;
	[SerializeField]
	private CinemachineImpulseListener cinemachineImpulseListener;

	[Title("Shake Definitions")]
	[SerializeField]
	private CameraShake smallShake;
	[SerializeField]
	private CameraShake bigShake;

	public Camera Camera => gameplayCamera;

	public void Initialize()
	{
	}

	public void Dispose()
	{
	}

	public float GetOrthographicSize()
	{
		return cmGameplayCamera.Lens.OrthographicSize;
	}

	public void SetOrthographicSize(float size)
	{
		cmGameplayCamera.Lens.OrthographicSize = size;
	}

	[BoxGroup("DEBUG")]
	[Button]
	public void CenterCameraOnDungeon(Vector2 worldCenter, Vector3 worldSize)
	{
		MoveToPosition(new Vector3(worldCenter.x, worldCenter.y));
		//change camera size to adjust to stage
		Vector3 bottomLeft = Camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector3 topRight = Camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

		float verticalDistance = topRight.y - bottomLeft.y;
		float horizontalDistance = topRight.x - bottomLeft.x;

		float neededVerticalDistance = worldSize.y + cameraBoarderPadding;
		float neededHorizontalDistance = worldSize.x + cameraBoarderPadding;

		float orhographicsSize = GetOrthographicSize();
		float targetOrtographicSizeX = neededVerticalDistance * orhographicsSize / verticalDistance;
		float targetOrtographicSizeY = neededHorizontalDistance * orhographicsSize / horizontalDistance;
		SetOrthographicSize(Math.Max(targetOrtographicSizeX, targetOrtographicSizeY));
	}

	private CinemachineImpulseManager.ImpulseEvent CreateAndReturnShakeEventActual(Unity.Cinemachine.CinemachineImpulseDefinition shakeDefinition, Vector3 position, Vector3 magnitude)
	{
		if (cinemachineImpulseListener.enabled)
		{
			var shakeEvent = shakeDefinition.CreateAndReturnEvent(position, magnitude);
			return shakeEvent;
		}
		return null;
	}

	[Button]
	public void SmallShake()
	{
		CreateAndReturnShakeEventActual(smallShake.shakeDef, bigShake.source, smallShake.magnitude);
	}

	[Button]
	public void BigShake()
	{
		CreateAndReturnShakeEventActual(bigShake.shakeDef, bigShake.source, bigShake.magnitude);
	}

	internal void MoveToPosition(Vector3 newPosition)
	{
		cameraTarget.position = newPosition;
	}

	[Serializable]
	public class CameraShake
	{
		[SerializeField]
		public Vector2 source;
		[SerializeField]
		public Vector2 magnitude;
		[SerializeField]
		public Unity.Cinemachine.CinemachineImpulseDefinition shakeDef;
	}
}
