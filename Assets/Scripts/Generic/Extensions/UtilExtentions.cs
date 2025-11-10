using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlugins.Utils
{
	public static class UtilExtentions
	{
		public static void SetGameObjectActive(this Component component, bool active)
		{
			if (!component || !component.gameObject) return;

			component.gameObject.SetGameObjectActive(active);
		}

		public static void SetGameObjectActive(this GameObject gameObject, bool active)
		{
			//It appears Unity is smart enough not to do any extra work if gameObject.activeSelf == active.
			if (!gameObject) return;
			gameObject.SetActive(active);
		}

		public static void SetGameObjectPosition(this GameObject gameObject, Vector3 position)
		{
			if (!gameObject) return;
			gameObject.transform.position = position;
		}

		public static void SetTextSafe(this TextMeshProUGUI textField, string text)
		{
			if (!textField || string.IsNullOrEmpty(text)) return;

			textField.SetText(text);
		}

		public static void SetTextColorSafe(this TextMeshProUGUI textField, Color color)
		{
			if (!textField) return;

			textField.color = color;
		}

		public static void SetIconSafe(this Image image, Sprite icon)
		{
			if (!image || !icon) return;

			image.sprite = icon;
		}

		public static void SetIconSafeAllowEmpty(this Image image, Sprite icon)
		{
			if (!image) return;

			image.sprite = icon;
		}

		public static void SetIconColorSafe(this Image image, Color color)
		{
			if (!image) return;
			image.color = color;
		}

		public static void SetIconColorSafeNoAlpha(this Image image, Color color)
		{
			if (!image) return;
			image.color = new Color(color.r, color.g, color.b, image.color.a);
		}

		public static string ToStringLoop(this Vector2 vector)
		{
			return $"[{vector.x},{vector.y}]";
		}

		public static string ToStringLoop(this Vector3 vector)
		{
			return $"[{vector.x},{vector.y},{vector.z}]";
		}

		public static string ToRGBHex(this Color c)
		{
			return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
		}

		private static byte ToByte(float f)
		{
			f = Mathf.Clamp01(f);
			return (byte)(f * 255);
		}

		public static T[] RemoveAt<T>(this T[] source, int index)
		{
			T[] dest = new T[source.Length - 1];
			if (index > 0)
				Array.Copy(source, 0, dest, 0, index);

			if (index < source.Length - 1)
				Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

			return dest;
		}

		public static int ToInt(this Enum enumValue)
		{
			return Convert.ToInt32(enumValue);
		}

		public static void SetRendererColor(Renderer renderer, MaterialPropertyBlock propertyBlock, Color color, string propertyName = "_Color")
		{
			if (renderer == null || propertyBlock == null) return;

			renderer.GetPropertyBlock(propertyBlock);
			propertyBlock.SetColor(propertyName, color);
			renderer.SetPropertyBlock(propertyBlock);
		}

		public static T[] ShiftRight<T>(this T[] array, int positions)
		{
			int length = array.Length;
			if (length == 0) return array;

			// normalize positions to avoid extra loops
			positions = ((positions % length) + length) % length;

			T[] result = new T[length];

			for (int i = 0; i < length; i++)
			{
				int newIndex = (i + positions) % length;
				result[newIndex] = array[i];
			}
			return result;
		}

		public static T DeepCopy<T>(this T value)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, value);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}

		public static int CountFlags<T>(T value) where T : Enum
		{
			long bits = Convert.ToInt64(value);
			int count = 0;

			while (bits != 0)
			{
				bits &= (bits - 1); // clears the lowest set bit
				count++;
			}

			return count;
		}

		public static void Shuffle<T>(this List<T> target, System.Random random)
		{
			for (int i = target.Count - 1; i > 0; i--)
			{
				int j = random.Next(i + 1);
				T tmp = target[i];
				target[i] = target[j];
				target[j] = tmp;
			}
		}

		public static void RandomAdd<T>(this List<T> target, T value, int count, System.Random random)
		{
			for (int i = count; i > 0; i--)
			{
				int j = random.Next(0, target.Count);
				target.Insert(j, value);
			}
		}

		public static void RandomRemove<T>(this List<T> target, int count, System.Random random)
		{
			for (int i = count; i > 0 && target.Count > 0; i--)
			{
				int j = random.Next(0, target.Count);
				target.RemoveAt(j);
			}
		}
	}
}
