using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
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

	public float GetOrthographicSize()
	{
		return cmGameplayCamera.Lens.OrthographicSize;
	}

	public void SetOrthographicSize(float size)
	{
		cmGameplayCamera.Lens.OrthographicSize = size;
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
