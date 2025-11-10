using Game.Grid;
using System;
using Unity.Cinemachine;
using UnityEngine;
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
		private float movementSpeed;
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
			UpdatePlaneMovement(xMovement, zMovement);
			UpdateVerticalMovement(yMovement);

			if (xMovement == 0 && zMovement == 0)
			{
				ReadMouseMovementInputValues();
			}

			ReadRotationInputValues(out float angularMovement);
			UpdateRotation(angularMovement);
		}

		private void ReadMouseMovementInputValues()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				// The cursor is over UI — ignore input
				return;
			}

			var mouse = Mouse.current;
			if (mouse == null)
			{
				return;
			}

			bool dragPressed = (mouse.leftButton != null && mouse.leftButton.isPressed);
			bool dragDown = (mouse.leftButton != null && mouse.leftButton.wasPressedThisFrame);
			bool dragUp = (mouse.leftButton != null && mouse.leftButton.wasReleasedThisFrame);
			Vector2 offset = new Vector2(10, 10);

			isDragging = dragPressed;
			if (dragDown)
			{
				isDragging = false;
				dragStartBaseScreenPosition = Mouse.current.position.value;
				dragStartBaseWorldPosition = HexGridManager.Instance.GetScreenSpacePlaneIntersection(dragStartBaseScreenPosition);
				dragStartOffsetWorldPosition = HexGridManager.Instance.GetScreenSpacePlaneIntersection(dragStartBaseScreenPosition + offset);

				dragBasePosition = planeMovementTransform.position;
			}

			if (!dragPressed || dragUp)
			{
				isDragging = false;
				return;
			}

			if (isDragging)
			{
				Vector3 currentWorld = HexGridManager.Instance.MousePlanePosition;
				Vector3 worldDelta = dragStartBaseWorldPosition - currentWorld; // move rig opposite to cursor movement

				worldDelta = GetMovementDelta(Mouse.current.position.value, dragStartBaseScreenPosition, dragStartBaseScreenPosition + offset, dragStartBaseWorldPosition, dragStartOffsetWorldPosition);

				planeMovementTransform.position = dragBasePosition + (dragStartBaseWorldPosition - worldDelta);

				//// Only move along XZ
				//worldDelta.y = 0f;

				//// Convert world delta to local x/z movement units that, after speed*dt, produce the same delta
				//float dt = Mathf.Max(Time.deltaTime, 1e-6f);
				//float denom = Mathf.Max(movementSpeed * dt, 1e-6f);
				//Vector3 required = worldDelta / denom;

				//Vector3 right = planeMovementTransform.right;
				//right.y = 0f;
				//right.Normalize();
				//Vector3 forward = planeMovementTransform.forward;
				//forward.y = 0f;
				//forward.Normalize();

				//xMovement = Vector3.Dot(required, right);
				//zMovement = Vector3.Dot(required, forward);
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
			if (Keyboard.current.wKey.value > 0)
				zMovement += Keyboard.current.wKey.value;
			if (Keyboard.current.sKey.value > 0)
				zMovement -= Keyboard.current.sKey.value;

			yMovement = 0;
			if (Keyboard.current.rKey.value > 0)
				yMovement += Keyboard.current.rKey.value;
			if (Keyboard.current.fKey.value > 0)
				yMovement -= Keyboard.current.fKey.value;

			if (yMovement == 0)
			{
				bool scrollValue = Mouse.current.scroll.value != Vector2.zero;
				yMovement = Mouse.current.scroll.value.y * -1;
			}

			xMovement = 0;
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

		private void UpdatePlaneMovement(float xMovement, float zMovement)
		{
			Vector3 move = planeMovementTransform.right * xMovement + planeMovementTransform.forward * zMovement;
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
