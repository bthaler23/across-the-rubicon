using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RM.Utils
{
	public static class ArrayExtension
	{
		public static bool Contains<T>(this T[] arr, T target)
		{
			for (int i = arr.Length - 1; i >= 0; i--)
			{
				if (arr[i].Equals(target))
					return true;
			}
			return false;
		}

		public static int IndexOf<T>(this T[] arr, T target)
		{
			for (int i = arr.Length - 1; i >= 0; i--)
			{
				if (arr[i].Equals(target))
					return i;
			}
			return -1;
		}

		public static bool ContainsAny<T>(this T[] arr, T[] target)
		{
			for (int j = target.Length - 1; j >= 0; j--)
			{
				for (int i = arr.Length - 1; i >= 0; i--)
				{
					if (target[j].Equals(arr[i]))
						return true;
				}
			}
			return false;
		}

		public static bool ContainsAny<T>(this T[] arr, T[] target, ref T match)
		{
			for (int j = target.Length - 1; j >= 0; j--)
			{
				for (int i = arr.Length - 1; i >= 0; i--)
				{
					if (target[j].Equals(arr[i]))
					{
						match = target[j];
						return true;
					}
				}
			}
			return false;
		}

		public static T[] Shuffle<T>(this T[] original)
		{
			System.Random rnd = new System.Random();
			T[] shuffled = original.OrderBy(x => rnd.Next()).ToArray();
			return shuffled;
		}

		public static bool IsOrdered<T>(this T[] original, T firstVal, T secondVal) where T : struct
		{
			for (int i = 0; i < original.Length; i++)
			{
				T val = original[i];
				if (val.Equals(firstVal))
					return true;
				if (val.Equals(secondVal))
					return false;

			}
			return false;
		}
	}
}
