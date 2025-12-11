using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
		private Image iconImage;
		[SerializeField]
		private TextMeshProUGUI textField;

		private Color defaultTextColor;
		private Vector3 movementDirection;
		private float activeTimer;

		protected override void Awake()
		{
			base.Awake();
			if (textField)
				defaultTextColor = textField.color;
		}

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

		public void ShowText(Vector3 position, Camera gameplayCamera, string text)
		{
			SetFollowTarget(position, gameplayCamera, deactivateDelay);
			ShowText(deactivateDelay, text, defaultTextColor, null);
		}

		public void ShowTextAndIcon(Transform targetXform, Camera gameplayCamera, string text, Color? textColor, Sprite sprite)
		{
			SetFollowTarget(targetXform, gameplayCamera, deactivateDelay);
			ShowText(deactivateDelay, text, textColor.HasValue ? textColor.Value : defaultTextColor, sprite);
		}

		private void ShowText(float deactivateTime, string text, Color color, Sprite sprite)
		{
			rTransform.localPosition = Vector3.zero;
			activeTimer = 0;

			iconImage.SetGameObjectActive(sprite != null);
			iconImage.SetIconSafe(sprite);

			textField.SetGameObjectActive(!string.IsNullOrEmpty(text));
			textField.SetTextSafe(text);
			textField.SetTextColorSafe(color);

			float randomAngle = UnityEngine.Random.Range(randomDirectionRange.x, randomDirectionRange.y);
			movementDirection = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;
			FollowTarget();
			enableFollow = false;
		}
	}
}