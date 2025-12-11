using Game.UI;
using GamePlugins.ObjectPool;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class FloatingUIWidget : BaseWidget
	{
		[SerializeField]
		protected bool returnToObjectPool;
		[SerializeField]
		protected CanvasGroup canvasGroup;
		[SerializeField]
		protected float deactivateDelay = 0f;
		[SerializeField, ShowIf("@this.canvasGroup!=null")]
		protected float disappearSpeed = 3f;

		protected float dissappearTimer;

		protected Camera gameplayCamera;
		protected Transform targetXform;
		protected float yOffset;

		protected Vector3 lastTargetPosition;
		protected RectTransform rTransform;
		protected bool enableFollow;

		protected override void Awake()
		{
			enableFollow = false;
			rTransform = GetComponent<RectTransform>();
		}

		protected override void Update()
		{
			base.Update();
			FollowTarget();
			FadeAwayPopup();
		}

		public void SetFollowTarget(Vector3 target, Camera gameplayCamera, float deactivateTime = 0)
		{
			lastTargetPosition = target;
			SetFollowTarget(gameplayCamera, deactivateTime);
		}

		public void SetFollowTarget(Transform target, Camera gameplayCamera, float deactivateTime = 0)
		{
			targetXform = target;
			SetFollowTarget(gameplayCamera, deactivateTime);
		}

		private void SetFollowTarget(Camera gameplayCamera, float deactivateTime = 0)
		{
			yOffset = 0;
			if (canvasGroup)
				canvasGroup.alpha = 1;
			dissappearTimer = deactivateTime;
			enableFollow = true;
			this.gameplayCamera = gameplayCamera;
			Show();
		}

		protected virtual void FollowTarget()
		{
			if (enableFollow && gameplayCamera)
			{
				Vector3 worldTargetPosition = GetTargetPosition() + GetFollowOffset();
				Vector3 screenPos = GetScreenPosition(worldTargetPosition);
				rTransform.position = new Vector3(screenPos.x, screenPos.y);
				LayoutRebuilder.MarkLayoutForRebuild(rTransform);
			}
		}

		protected virtual Vector3 GetScreenPosition(Vector3 worldPosition)
		{
			Vector3 screenPos = gameplayCamera.WorldToScreenPoint(worldPosition);
			return screenPos;
		}

		protected virtual Vector3 GetTargetPosition()
		{
			if (targetXform)
			{
				lastTargetPosition = targetXform.position;
				return lastTargetPosition;
			}
			return lastTargetPosition;
		}

		private Vector3 GetFollowOffset()
		{
			return Vector3.up * yOffset;
		}

		private void FadeAwayPopup()
		{
			dissappearTimer -= Time.deltaTime;
			if (dissappearTimer < 0)
			{
				if (canvasGroup)
				{
					float dissappearSpeed = 3f;
					canvasGroup.alpha -= dissappearSpeed * Time.deltaTime;
					if (canvasGroup.alpha <= 0)
					{
						Deactivate();
					}
				}
				else
				{
					Deactivate();
				}
			}
		}

		private void Deactivate()
		{
			if (returnToObjectPool)
			{
				ObjectPool.ReturnObject(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}

