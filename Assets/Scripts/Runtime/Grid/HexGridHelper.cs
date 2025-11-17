using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
	public static class HexGridHelper
	{
		public enum HexSide
		{
			T1 = 0,
			T2 = 1,
			T3 = 2,
			T4 = 3,
			T5 = 4,
			T6 = 5,
		}

		public struct HexSegmentID
		{
			public Vector2Int hexIndex;
			public int sideIndex;

			public override bool Equals(object obj)
			{
				return obj is HexSegmentID triangle &&
					   hexIndex.Equals(triangle.hexIndex) &&
					   sideIndex == triangle.sideIndex;
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(hexIndex, sideIndex);
			}

			public static bool operator ==(HexSegmentID a, HexSegmentID b) => a.hexIndex == b.hexIndex && a.sideIndex == b.sideIndex;
			public static bool operator !=(HexSegmentID a, HexSegmentID b) => !(a == b);

			public override string ToString()
			{
				return $"({hexIndex} / {sideIndex})";
			}
		}

		public static readonly Dictionary<HexSide, Vector2Int> NeighbourOffsetMap = new()
		{
			{ HexSide.T1, new Vector2Int(-1, 0) },
			{ HexSide.T2, new Vector2Int(-1, 1) },
			{ HexSide.T3, new Vector2Int(0, 1) },
			{ HexSide.T4, new Vector2Int(1, 0) },
			{ HexSide.T5, new Vector2Int(1, -1) },
			{ HexSide.T6, new Vector2Int(0, -1) }

		};

		public static readonly Dictionary<HexSide, HexSide> NeighbourMap = new()
		{
			{ HexSide.T1, HexSide.T4 },
			{ HexSide.T2, HexSide.T5 },
			{ HexSide.T3, HexSide.T6 },
			{ HexSide.T4, HexSide.T1 },
			{ HexSide.T5, HexSide.T2 },
			{ HexSide.T6, HexSide.T3 },
		};

		public static readonly List<Vector2Int> NeighbourOffsetXEVEN = new()
		{
			{ new Vector2Int(+1,  0) },
			{ new Vector2Int(+1, -1) },
			{ new Vector2Int( 0, -1) },
			{ new Vector2Int(-1,  0) },
			{ new Vector2Int( 0, +1) },
			{ new Vector2Int(+1, +1) }
		};

		public static readonly List<Vector2Int> NeighbourOffsetXODD = new()
		{
			{ new Vector2Int(+1,  0) },
			{ new Vector2Int( 0, -1) },
			{ new Vector2Int(-1, -1) },
			{ new Vector2Int(-1,  0) },
			{ new Vector2Int(-1, +1) },
			{ new Vector2Int( 0, +1) }
		};

		public static Vector2Int GetSideNeighbourIndex(Vector2Int cellIndex, HexSide hexSide)
		{
			return cellIndex + NeighbourOffsetMap[hexSide];
		}

		public static HexSide GetSideNeighbourSide(HexSide hexSide)
		{
			return NeighbourMap[hexSide];
		}

		public static HexSegmentID GetNeighbourCellTriangle(HexSegmentID hexTriangle)
		{
			var neighbourIndex = HexGridHelper.GetSideNeighbourIndex(hexTriangle.hexIndex, (HexSide)hexTriangle.sideIndex);
			var newNighbourSide = HexGridHelper.GetSideNeighbourSide((HexSide)hexTriangle.sideIndex);
			return new HexSegmentID() { hexIndex = neighbourIndex, sideIndex = (int)newNighbourSide };
		}

		public static List<Vector2Int> GetAllNeighbourPositions(Vector2Int cellIndex)
		{
			List<Vector2Int> neigh = new List<Vector2Int>();
			foreach (var i in NeighbourOffsetMap)
			{
				neigh.Add(cellIndex + i.Value);
			}
			return neigh;
		}

		public static List<HexSegmentID> GetTriangleNeighbours(HexSegmentID current, bool isHexInterconnected)
		{
			List<HexSegmentID> neighbours = new List<HexSegmentID>();
			if (!isHexInterconnected)
			{
				neighbours.Add(new HexSegmentID() { hexIndex = current.hexIndex, sideIndex = GetSideIndex(current.sideIndex + 1) });
				neighbours.Add(new HexSegmentID() { hexIndex = current.hexIndex, sideIndex = GetSideIndex(current.sideIndex - 1) });
			}
			else
			{
				for (int i = 0; i < 6; i++)
					if (current.sideIndex != i)
						neighbours.Add(new HexSegmentID() { hexIndex = current.hexIndex, sideIndex = i });
			}
			neighbours.Add(HexGridHelper.GetNeighbourCellTriangle(current));
			return neighbours;
		}

		private static int GetSideIndex(int v)
		{
			if (v >= 6) return 0;
			if (v < 0) return 5;
			return v;
		}

		public static int GetRotationOffset(Vector3 rightVector)
		{
			float angle = Vector3.SignedAngle(Vector3.right, rightVector, Vector3.up);
			angle = (angle + 360f) % 360f;
			int rotationOffset = Mathf.RoundToInt(angle) / 60;
			return rotationOffset;
		}

		public static int GetSideRelativeIndex(HexSide tileSide, int rotationOffset)
		{
			//positive modulo
			//rotation in list is different from rotation in transform, need to invert the offset
			return (((int)tileSide + (rotationOffset * -1) % 6) + 6) % 6;
		}

		public static int GetSideShiftedIndex(HexSide tileSide, int rotationOffset)
		{
			//positive modulo
			//rotation in list is different from rotation in transform, need to invert the offset
			return (((int)tileSide + (rotationOffset * 1) % 6) + 6) % 6;
		}

		// New helper: all positions (including center) within range on an infinite hex grid using BFS levels.
		public static List<Vector2Int> GetPositionsInRange(Vector2Int center, int range)
		{
			var result = new List<Vector2Int>();
			if (range < 0) return result;
			var visited = new HashSet<Vector2Int> { center };
			var frontier = new List<Vector2Int> { center };

			//Debug.Log($"Start BFS at : {center}");

			for (int depth = 0; depth < range; depth++)
			{
				//Debug.Log($"--BFS Depth: {depth}");
				var next = new List<Vector2Int>();
				foreach (var pos in frontier)
				{
					var neighbourMap = pos.y % 2 == 0 ? NeighbourOffsetXEVEN : NeighbourOffsetXODD;
					foreach (var off in NeighbourOffsetMap.Values)
					{
						var neigh = pos + off;
						if (visited.Add(neigh))
						{
							result.Add(neigh);
							next.Add(neigh);
							//Debug.Log($"-- --BFS Pos: {neigh}");
						}
					}
				}
				frontier = next;
				if (frontier.Count == 0) break;
			}
			return result;
		}
	}
}
