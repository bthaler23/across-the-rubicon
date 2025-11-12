using Game.Grid;
using Sirenix.OdinInspector;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game.Navigation
{
	public class SimpleNavigation : MonoBehaviour
	{
		[SerializeField]
		private Transform planeMovementTransform;
		[SerializeField]
		private Transform verticalMovementTransform;
		[SerializeField]
		private Axis secondaryAxis = Axis.Y;
		[SerializeField]
		private float movementSpeed;
		[SerializeField]
		private bool allowAngularMovement = false;
		[ShowIf("@this.allowAngularMovement==true")]
		[SerializeField]
		private float angularSpeed;

		[SerializeField]
		private float defaultZoom;
		[SerializeField]
		private Vector3 minZoom;
		[SerializeField]
		private Vector3 maxZoom;

		[SerializeField]
		private float zoomSpeed = 1f;

		private float currentZoom;

		[SerializeField]
		private CinemachineThirdPersonFollow thirdPersonFollow;

		// Drag state
		private bool isDragging = false;
		private Vector2 dragStartBaseScreenPosition;
		private Vector3 dragStartBaseWorldPosition;
		private Vector3 dragStartOffsetWorldPosition;
		private Vector3 dragStartPos;

		private Vector3 dragBasePosition;

		private void Awake()
		{
			currentZoom = defaultZoom;
		}

		void Update()
		{
			float xMovement = 0;
			float yMovement = 0;
			float zMovement = 0;

			ReadMovementInputValues(out xMovement, out yMovement, out zMovement);
			UpdatePlaneMovement(xMovement, yMovement, zMovement);
			UpdateVerticalMovement(yMovement);

			if (allowAngularMovement)
			{
				ReadRotationInputValues(out float angularMovement);
				UpdateRotation(angularMovement);
			}
		}

		private Vector3 GetMovementDelta(Vector2 screenPos, Vector2 screenPosStart, Vector2 screenRefencePos, Vector3 worldStart, Vector3 worldReferencePos)
		{
			// We have two corresponding points:
			// screenPosStart (XY)  <-> worldStart (XZ)
			// screenRefencePos (XY) <-> worldReferencePos (XZ)
			// Treat screen X mapping independently to world X, and screen Y to world Z.
			// Compute per-axis scale factors and apply them to the screen delta.

			Vector2 screenDelta = screenPos - screenPosStart;
			Vector2 screenRefDelta = screenRefencePos - screenPosStart;
			Vector3 worldRefDelta = worldReferencePos - worldStart;

			float scaleX = 0f;
			float scaleZ = 0f;

			// Avoid division by zero; if zero, no movement along that axis.
			if (Mathf.Abs(screenRefDelta.x) > 1e-6f)
				scaleX = worldRefDelta.x / screenRefDelta.x;
			if (Mathf.Abs(screenRefDelta.y) > 1e-6f)
				scaleZ = worldRefDelta.z / screenRefDelta.y;

			float worldX = worldStart.x + screenDelta.x * scaleX;
			float worldZ = worldStart.z + screenDelta.y * scaleZ;

			return new Vector3(worldX, worldStart.y, worldZ);
		}

		private void ReadMovementInputValues(out float xMovement, out float yMovement, out float zMovement)
		{
			zMovement = 0;
			yMovement = 0;
			xMovement = 0;

			if (secondaryAxis == Axis.Z)
			{
				if (Keyboard.current.wKey.value > 0)
					zMovement += Keyboard.current.wKey.value;
				if (Keyboard.current.sKey.value > 0)
					zMovement -= Keyboard.current.sKey.value;
			}
			else
			{
				if (Keyboard.current.wKey.value > 0)
					yMovement += Keyboard.current.wKey.value;
				if (Keyboard.current.sKey.value > 0)
					yMovement -= Keyboard.current.sKey.value;
			}

			if (Keyboard.current.aKey.value > 0)
				xMovement -= Keyboard.current.aKey.value;
			if (Keyboard.current.dKey.value > 0)
				xMovement += Keyboard.current.dKey.value;
		}

		private void ReadRotationInputValues(out float angularMovement)
		{
			angularMovement = 0;
			if (Keyboard.current.qKey.value > 0)
				angularMovement += Keyboard.current.qKey.value;
			if (Keyboard.current.eKey.value > 0)
				angularMovement -= Keyboard.current.eKey.value;
		}

		private void UpdatePlaneMovement(float xMovement, float yMovement, float zMovement)
		{
			Vector3 move = planeMovementTransform.right * xMovement + planeMovementTransform.forward * zMovement + planeMovementTransform.up * yMovement;
			Vector3 newPositionOffset = move * movementSpeed * Time.deltaTime;
			planeMovementTransform.position += newPositionOffset;
		}

		private void UpdateVerticalMovement(float verticalMovement)
		{
			if (verticalMovementTransform)
			{
				Vector3 move = verticalMovementTransform.forward * verticalMovement;
				Vector3 newPositionOffset = move * movementSpeed * Time.deltaTime;
				verticalMovementTransform.position += newPositionOffset;
			}
			else if (thirdPersonFollow)
			{
				thirdPersonFollow.ShoulderOffset = GetZoomValue(verticalMovement);
			}
		}

		private Vector3 GetZoomValue(float verticalMovement)
		{
			currentZoom = Mathf.Clamp01(currentZoom + verticalMovement * zoomSpeed * Time.deltaTime);
			return Vector3.Lerp(minZoom, maxZoom, currentZoom);
		}

		private void UpdateRotation(float angularMovement)
		{
			float angleOffset = angularMovement * angularSpeed * Time.deltaTime;
			Quaternion rotation = Quaternion.AngleAxis(angleOffset, Vector3.up);
			planeMovementTransform.rotation *= rotation;
		}
	}
}
