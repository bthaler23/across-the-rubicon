using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Grid
{
	public class HexCell
	{
		public Vector2Int Position;
		public int Height;
	}

	[Serializable]
	public struct HeighInfluanceData
	{
		public float influenceStrength;
		public int influenceDirection;

		internal float GetInfluence()
		{
			return influenceStrength * influenceDirection;
		}
	}

	public static class ProceduralGridGenerator
	{
		public static List<HexCell> GenerateRandomHexGrid(
			int cellCount,
			System.Random rng)
		{
			return GenerateRandomHexGrid(cellCount, 0, 0, new Dictionary<int, float> { { 0, 1f } }, rng);
		}

		public static List<HexCell> GenerateRandomHexGrid(
			int cellCount,
			int minHeight,
			int maxHeight,
			Dictionary<int, float> heightChances, // e.g. {0:0.2, 1:0.5, 2:0.3}
			System.Random rng)
		{
			var directions = new Vector2Int[]
			{
				new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
				new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
			};

			var grid = new Dictionary<Vector2Int, HexCell>();
			var frontier = new List<Vector2Int>();

			// Start at (0,0)
			var start = new Vector2Int(0, 0);
			grid[start] = new HexCell { Position = start, Height = GetRandomHeight(heightChances, rng) };
			frontier.Add(start);

			while (grid.Count < cellCount)
			{
				// Pick a random cell from the frontier (bias towards center if desired)
				var idx = rng.Next(frontier.Count);
				var current = frontier[idx];

				// Find unoccupied neighbors
				var available = directions
					.Select(d => current + d)
					.Where(pos => !grid.ContainsKey(pos))
					.ToList();

				if (available.Count == 0)
				{
					// No available neighbors, remove from frontier
					frontier.RemoveAt(idx);
					continue;
				}

				// Pick a random neighbor to expand
				var next = available[rng.Next(available.Count)];
				grid[next] = new HexCell { Position = next, Height = GetRandomHeight(heightChances, rng) };
				frontier.Add(next);

				// Optionally, remove current from frontier if all its neighbors are filled
				if (directions.All(d => grid.ContainsKey(current + d)))
					frontier.RemoveAt(idx);
			}

			return grid.Values.ToList();
		}

		// Helper to pick a height based on weighted chances
		private static int GetRandomHeight(Dictionary<int, float> heightChances, System.Random rng)
		{
			float total = heightChances.Values.Sum();
			float roll = (float)rng.NextDouble() * total;
			float accum = 0f;
			foreach (var kvp in heightChances)
			{
				accum += kvp.Value;
				if (roll <= accum)
					return kvp.Key;
			}
			// Fallback
			return heightChances.Keys.First();
		}
	}

}
