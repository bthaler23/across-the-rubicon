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

		/// <summary>
		/// Generates a roughly rectangular axial hex grid of given width (q axis) and height (r axis) with:
		/// - All cells height = 0
		/// - Random edge variation (cells removed on perimeter)
		/// - Optional sparse interior holes (ensuring overall connectivity)
		/// The resulting shape approximates the requested dimensions but has organic, noisy borders.
		/// </summary>
		/// <param name="width">Number of columns along q axis.</param>
		/// <param name="height">Number of rows along r axis.</param>
		/// <param name="edgeRemovalChance">Chance (0..1) to remove each perimeter cell (filtered to keep connectivity).</param>
		/// <param name="interiorHoleChance">Chance (0..1) to remove a non-edge cell to create a hole (very small recommended).</param>
		/// <param name="rng">Random source.</param>
		public static List<HexCell> GenerateVariableRectHexGrid(int width, int height, float edgeRemovalChance, float interiorHoleChance, System.Random rng)
		{
			if (width <= 0 || height <= 0) return new List<HexCell>();

			// Build full rectangular set in axial coordinates (0..width-1, 0..height-1)
			var allPositions = new HashSet<Vector2Int>();
			for (int q = 0; q < width; q++)
				for (int r = 0; r < height; r++)
					allPositions.Add(new Vector2Int(q, r));

			// Collect edge positions
			var edgePositions = allPositions.Where(p => p.x == 0 || p.x == width - 1 || p.y == 0 || p.y == height - 1).ToList();
			Shuffle(edgePositions, rng);

			// Directions for neighbor lookup
			var directions = new Vector2Int[]
			{
				new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
				new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
			};

			bool IsConnected(HashSet<Vector2Int> positions)
			{
				if (positions.Count == 0) return true;
				var start = positions.First();
				var visited = new HashSet<Vector2Int>();
				var queue = new Queue<Vector2Int>();
				queue.Enqueue(start);
				visited.Add(start);
				while (queue.Count > 0)
				{
					var cur = queue.Dequeue();
					foreach (var d in directions)
					{
						var n = cur + d;
						if (!positions.Contains(n) || visited.Contains(n)) continue;
						visited.Add(n);
						queue.Enqueue(n);
					}
				}
				return visited.Count == positions.Count;
			}

			// Attempt edge removals
			foreach (var edge in edgePositions)
			{
				if (!allPositions.Contains(edge)) continue; // may have been removed
				if (rng.NextDouble() > edgeRemovalChance) continue;
				// Tentatively remove and test connectivity
				allPositions.Remove(edge);
				if (!IsConnected(allPositions))
				{
					allPositions.Add(edge); // revert if disconnects
				}
			}

			// Interior holes (avoid edges)
			var interior = allPositions.Where(p => p.x > 0 && p.x < width - 1 && p.y > 0 && p.y < height - 1).ToList();
			Shuffle(interior, rng);
			foreach (var pos in interior)
			{
				if (rng.NextDouble() > interiorHoleChance) continue;
				allPositions.Remove(pos);
				if (!IsConnected(allPositions))
				{
					allPositions.Add(pos); // revert
				}
			}

			// Create HexCells (height always 0)
			var result = allPositions.Select(p => new HexCell { Position = p, Height = 0 }).ToList();
			return result;
		}

		/// <summary>
		/// Generates a circular cluster of hex cells centered at (0,0) using axial coordinates.
		/// The circle is approximated using Euclidean distance in pointy-top axial-to-world space.
		/// Supports optional edge jitter and interior holes while preserving connectivity.
		/// </summary>
		/// <param name="radius">Radius in hex-world units (roughly tile size = 1).</param>
		/// <param name="edgeRemovalChance">Chance (0..1) to remove each perimeter cell (keeps connectivity).</param>
		/// <param name="interiorHoleChance">Chance (0..1) to remove interior cells (keeps connectivity).</param>
		/// <param name="rng">Random source.</param>
		public static List<HexCell> GenerateCircularHexGrid(int radius, float edgeRemovalChance, float interiorHoleChance, System.Random rng)
		{
			if (radius <= 0) return new List<HexCell>();

			// Directions for neighbor lookup
			var directions = new Vector2Int[]
			{
				new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
				new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
			};

			bool IsConnected(HashSet<Vector2Int> positions)
			{
				if (positions.Count == 0) return true;
				var start = positions.First();
				var visited = new HashSet<Vector2Int>();
				var queue = new Queue<Vector2Int>();
				queue.Enqueue(start);
				visited.Add(start);
				while (queue.Count > 0)
				{
					var cur = queue.Dequeue();
					foreach (var d in directions)
					{
						var n = cur + d;
						if (!positions.Contains(n) || visited.Contains(n)) continue;
						visited.Add(n);
						queue.Enqueue(n);
					}
				}
				return visited.Count == positions.Count;
			}

			List<Vector2Int> GetEdgePositions(HashSet<Vector2Int> positions)
			{
				var edges = new List<Vector2Int>();
				foreach (var p in positions)
				{
					foreach (var d in directions)
					{
						if (!positions.Contains(p + d)) { edges.Add(p); break; }
					}
				}
				return edges;
			}

			// Build base set inside a circle (using axial -> world euclidean distance)
			var allPositions = new HashSet<Vector2Int>();
			float rWorld = radius; // assume hex size = 1
			for (int q = -radius; q <= radius; q++)
			{
				for (int r = -radius; r <= radius; r++)
				{
					var p = new Vector2Int(q, r);
					var w = AxialToWorld(p, 1f);
					if (w.sqrMagnitude <= rWorld * rWorld + 1e-3f)
					{
						allPositions.Add(p);
					}
				}
			}

			// Edge jitter
			var edgesList = GetEdgePositions(allPositions);
			Shuffle(edgesList, rng);
			foreach (var edge in edgesList)
			{
				if (!allPositions.Contains(edge)) continue;
				if (rng.NextDouble() > edgeRemovalChance) continue;
				allPositions.Remove(edge);
				if (!IsConnected(allPositions))
					allPositions.Add(edge);
			}

			// Interior holes (exclude current edges)
			var currentEdges = new HashSet<Vector2Int>(GetEdgePositions(allPositions));
			var interior = allPositions.Where(p => !currentEdges.Contains(p)).ToList();
			Shuffle(interior, rng);
			foreach (var pos in interior)
			{
				if (rng.NextDouble() > interiorHoleChance) continue;
				allPositions.Remove(pos);
				if (!IsConnected(allPositions))
					allPositions.Add(pos);
			}

			return allPositions.Select(p => new HexCell { Position = p, Height = 0 }).ToList();
		}

		/// <summary>
		/// Generates an equilateral triangular hex grid region of given size using axial coordinates.
		/// Region is defined by q >= 0, r >= 0, and q + r < size, producing a 60-degree triangle.
		/// Supports optional edge jitter and interior holes while preserving connectivity.
		/// </summary>
		/// <param name="size">Triangle side length in cells.</param>
		/// <param name="edgeRemovalChance">Chance (0..1) to remove each perimeter cell (keeps connectivity).</param>
		/// <param name="interiorHoleChance">Chance (0..1) to remove interior cells (keeps connectivity).</param>
		/// <param name="rng">Random source.</param>
		public static List<HexCell> GenerateTriangularHexGrid(int size, float edgeRemovalChance, float interiorHoleChance, System.Random rng)
		{
			if (size <= 0) return new List<HexCell>();

			var directions = new Vector2Int[]
			{
				new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
				new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
			};

			bool IsConnected(HashSet<Vector2Int> positions)
			{
				if (positions.Count == 0) return true;
				var start = positions.First();
				var visited = new HashSet<Vector2Int>();
				var queue = new Queue<Vector2Int>();
				queue.Enqueue(start);
				visited.Add(start);
				while (queue.Count > 0)
				{
					var cur = queue.Dequeue();
					foreach (var d in directions)
					{
						var n = cur + d;
						if (!positions.Contains(n) || visited.Contains(n)) continue;
						visited.Add(n);
						queue.Enqueue(n);
					}
				}
				return visited.Count == positions.Count;
			}

			List<Vector2Int> GetEdgePositions(HashSet<Vector2Int> positions)
			{
				var edges = new List<Vector2Int>();
				foreach (var p in positions)
				{
					foreach (var d in directions)
					{
						if (!positions.Contains(p + d)) { edges.Add(p); break; }
					}
				}
				return edges;
			}

			// Base triangle region
			var allPositions = new HashSet<Vector2Int>();
			for (int q = 0; q < size; q++)
			{
				for (int r = 0; r < size; r++)
				{
					if (q + r < size)
						allPositions.Add(new Vector2Int(q, r));
				}
			}

			// Edge jitter
			var edgesList = GetEdgePositions(allPositions);
			Shuffle(edgesList, rng);
			foreach (var edge in edgesList)
			{
				if (!allPositions.Contains(edge)) continue;
				if (rng.NextDouble() > edgeRemovalChance) continue;
				allPositions.Remove(edge);
				if (!IsConnected(allPositions))
					allPositions.Add(edge);
			}

			// Interior holes
			var currentEdges = new HashSet<Vector2Int>(GetEdgePositions(allPositions));
			var interior = allPositions.Where(p => !currentEdges.Contains(p)).ToList();
			Shuffle(interior, rng);
			foreach (var pos in interior)
			{
				if (rng.NextDouble() > interiorHoleChance) continue;
				allPositions.Remove(pos);
				if (!IsConnected(allPositions))
					allPositions.Add(pos);
			}

			return allPositions.Select(p => new HexCell { Position = p, Height = 0 }).ToList();
		}

		private static void Shuffle<T>(IList<T> list, System.Random rng)
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = rng.Next(i + 1);
				(var listI, var listJ) = (list[i], list[j]);
				list[i] = listJ;
				list[j] = listI;
			}
		}

		private static Vector2 AxialToWorld(Vector2Int axial, float size)
		{
			// Pointy-top axial to world (x,z) mapping
			float x = size * (Mathf.Sqrt(3f) * (axial.x + axial.y * 0.5f));
			float z = size * (1.5f * axial.y);
			return new Vector2(x, z);
		}
	}

}
