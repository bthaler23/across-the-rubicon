using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
	public class RandomDirectionFadeWidget : FloatingUIWidget
	{
		[SerializeField]
		private float movementSpeedMultiplier = 2f;
		[SerializeField]
		private AnimationCurve movementSpeed;
		[SerializeField]
		[MinMaxSlider(0, 360)]
		private Vector2Int randomDirectionRange;
		[SerializeField]
		private TextMeshProUGUI textField;
		[SerializeField]
		private bool controlColor = true;
		[ShowIf("@this.controlColor==true")]
		[SerializeField]
		private Color defaultTextColor;
		[ShowIf("@this.controlColor==true")]
		[SerializeField]
		private Color customTextColor;

		private Vector3 movementDirection;
		private float activeTimer;

		protected override void Update()
		{
			activeTimer += Time.deltaTime;
			base.Update();
			UpdateFadeAwayPosition();
		}

		private void UpdateFadeAwayPosition()
		{
			rTransform.localPosition += movementDirection * Time.deltaTime * movementSpeed.Evaluate(activeTimer) * movementSpeedMultiplier;
		}

		public void ShowText(Vector3 position, Camera gameplayCamera, float deactivateTime, string text, bool useCustomColor = false)
		{
			SetFollowTarget(position, gameplayCamera, deactivateTime);
			ShowText(deactivateTime, text, useCustomColor);
		}

		public void ShowText(Transform targetXform, Camera gameplayCamera, float deactivateTime, string text, bool useCustomColor = false)
		{
			SetFollowTarget(targetXform, gameplayCamera, deactivateTime);
			ShowText(deactivateTime, text, useCustomColor);
		}

		private void ShowText(float deactivateTime, string text, bool useCustomColor = false)
		{
			rTransform.localPosition = Vector3.zero;
			activeTimer = 0;
			textField.SetTextSafe(text);
			if (controlColor)
				textField.SetTextColorSafe(useCustomColor ? customTextColor : defaultTextColor);
			float randomAngle = UnityEngine.Random.Range(randomDirectionRange.x, randomDirectionRange.y);
			movementDirection = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;
			FollowTarget();
			enableFollow = false;
		}
	}
}