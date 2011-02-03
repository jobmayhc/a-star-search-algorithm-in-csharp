using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A_star_Demo.A_star_Algorithm
{
	partial class WorldMap : IEnumerable<WorldMap.BlockInfo>
	{
		[Flags]
		private enum AddedNeighbours
		{
			None = 0,
			HigherX = 1,
			LowerX = 1 << 1,
			HigherY = 1 << 2,
			LowerY = 1 << 3,
			HigherZ = 1 << 4,
			LowerZ = 1 << 5,
			Diagonal2DHigherXHigherY = 1 << 6,
			Diagonal2DHigherXLowerY = 1 << 7,
			Diagonal2DLowerXHigherY = 1 << 8,
			Diagonal2DLowerXLowerY = 1 << 9,
			Diagonal2DHigherXHigherZ = 1 << 10,
			Diagonal2DHigherXLowerZ = 1 << 11,
			Diagonal2DLowerXHigherZ = 1 << 12,
			Diagonal2DLowerXLowerZ = 1 << 13,
			Diagonal2DHigherYHigherZ = 1 << 14,
			Diagonal2DHigherYLowerZ = 1 << 15,
			Diagonal2DLowerYHigherZ = 1 << 16,
			Diagonal2DLowerYLowerZ = 1 << 17
		}

		public WorldMap(int xSize, int ySize, int zSize, Tuple<int,int,int> whatStart = null, Tuple<int,int,int> whatGoal = null)
		{
			_start = whatStart;
			_goal = whatGoal;
			canBarelyPassBlocks = false;

			xSizeCache = xSize;
			ySizeCache = ySize;
			zSizeCache = zSize;

			//Create the buckets for each possible x value
			for (int a = 0; a < xSize; a++)
			{
				actualMap.Add(new List<List<MapNode>>());

				//Create the buckets for each possible y value
				for (int b = 0; b < ySize; b++)
				{
					actualMap[a].Add(new List<MapNode>());

					//Create the actual nodes for each possible z value
					for (int c = 0; c < zSize; c++)
					{
						actualMap[a][b].Add(new MapNode(a, b, c, whatGoal));
					}
				}
			}
			path = new LinkedList<BlockInfo>();
		}

		/// <summary>
		/// Gets the list of neighbours for a provided node
		/// </summary>
		/// <param name="whatNode">the node to search the neighbours for</param>
		/// <returns>A list of neighbour nodes</returns>
		public List<Tuple<MapNode, int>> getNeighbours(MapNode whatNode)
		{
			if (whatNode.x > xSizeCache - 1 || whatNode.y > ySizeCache - 1 || whatNode.z > zSizeCache - 1)
				throw new ArgumentException("The provided node is not within this map", "whatNode");

			List<Tuple<MapNode, int>> neighbours = new List<Tuple<MapNode, int>>();
			MapNode cache;
			AddedNeighbours addedCache = AddedNeighbours.None;

			#region Straight neighbours
			//Add straight x neighbours
			if (whatNode.x != xSizeCache - 1)
			{
				cache = actualMap[whatNode.x + 1][whatNode.y][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.BLOCK_DISTANCE));
					addedCache |= AddedNeighbours.HigherX;
				}
			}
			if (whatNode.x != 0)
			{
				cache = actualMap[whatNode.x - 1][whatNode.y][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.BLOCK_DISTANCE));
					addedCache |= AddedNeighbours.LowerX;
				}
			}

			//Add straight y neighbours
			if (whatNode.y != ySizeCache - 1)
			{
				cache = actualMap[whatNode.x][whatNode.y + 1][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.BLOCK_DISTANCE));
					addedCache |= AddedNeighbours.HigherY;
				}
			}
			if (whatNode.y != 0)
			{
				cache = actualMap[whatNode.x][whatNode.y - 1][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.BLOCK_DISTANCE));
					addedCache |= AddedNeighbours.LowerY;
				}
			}

			//Add straight z neighbours
			if (whatNode.z != zSizeCache - 1)
			{
				cache = actualMap[whatNode.x][whatNode.y][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.BLOCK_DISTANCE));
					addedCache |= AddedNeighbours.HigherZ;
				}
			}
			if (whatNode.z != 0)
			{
				cache = actualMap[whatNode.x][whatNode.y][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.BLOCK_DISTANCE));
					addedCache |= AddedNeighbours.LowerZ;
				}
			}
			#endregion Straight neighbours

			#region Diagonal XY plane

			//Higher X higher Y
			if (whatNode.x != xSizeCache - 1 && whatNode.y != ySizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.HigherX | AddedNeighbours.HigherY)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y + 1][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DHigherXHigherY;
				}
			}

			//Higher X lower Y
			if (whatNode.x != xSizeCache - 1 && whatNode.y != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.HigherX | AddedNeighbours.LowerY)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y - 1][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DHigherXLowerY;
				}
			}

			//Lower X higher Y
			if (whatNode.x != 0 && whatNode.y != ySizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.LowerX | AddedNeighbours.HigherY)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y + 1][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DLowerXHigherY;
				}
			}

			//Lower X lower Y
			if (whatNode.x != 0 && whatNode.y != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.LowerX | AddedNeighbours.LowerY)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y - 1][whatNode.z];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DLowerXLowerY;
				}
			}
			#endregion Diagonal XY plane

			#region Diagonal XZ plane

			//Higher X higher Z
			if (whatNode.x != xSizeCache - 1 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.HigherX | AddedNeighbours.HigherZ)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DHigherXHigherZ;
				}
			}

			//Higher X lower Z
			if (whatNode.x != xSizeCache - 1 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.HigherX | AddedNeighbours.LowerZ)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DHigherXLowerZ;
				}
			}

			//Lower X higher Z
			if (whatNode.x != 0 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.LowerX | AddedNeighbours.HigherZ)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DLowerXHigherZ;
				}
			}

			//Lower X lower Z
			if (whatNode.x != 0 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.LowerX | AddedNeighbours.LowerZ)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DLowerXLowerZ;
				}
			}
			#endregion Diagonal XZ plane

			#region Diagonal YZ plane

			//Higher Y higher Z
			if (whatNode.y != xSizeCache - 1 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.HigherY | AddedNeighbours.HigherZ)))
			{
				cache = actualMap[whatNode.x][whatNode.y + 1][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DHigherYHigherZ;
				}
			}

			//Higher Y lower Z
			if (whatNode.y != xSizeCache - 1 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.HigherY | AddedNeighbours.LowerZ)))
			{
				cache = actualMap[whatNode.x][whatNode.y + 1][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DHigherYLowerZ;
				}
			}

			//Lower Y higher Z
			if (whatNode.y != 0 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.LowerY | AddedNeighbours.HigherZ)))
			{
				cache = actualMap[whatNode.x][whatNode.y - 1][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DLowerYHigherZ;
				}
			}

			//Lower Y lower Z
			if (whatNode.y != 0 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(AddedNeighbours.LowerY | AddedNeighbours.LowerZ)))
			{
				cache = actualMap[whatNode.x][whatNode.y - 1][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_2D_DIAGONAL));
					addedCache |= AddedNeighbours.Diagonal2DLowerYLowerZ;
				}
			}
			#endregion Diagonal YZ plane

			#region 3D walks higher x plane

			//Higher X higher Y higher Z
			if (whatNode.x != xSizeCache - 1 && whatNode.y != ySizeCache - 1 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DHigherXHigherY | 
				AddedNeighbours.Diagonal2DHigherXHigherZ |
				AddedNeighbours.Diagonal2DHigherYHigherZ)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y + 1][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			//Higher X higher Y lower Z
			if (whatNode.x != xSizeCache - 1 && whatNode.y != ySizeCache - 1 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DHigherXHigherY |
				AddedNeighbours.Diagonal2DHigherXLowerZ |
				AddedNeighbours.Diagonal2DHigherYLowerZ)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y + 1][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			//Higher X lower Y higher Z
			if (whatNode.x != xSizeCache - 1 && whatNode.y != 0 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DHigherXLowerY |
				AddedNeighbours.Diagonal2DHigherXHigherZ |
				AddedNeighbours.Diagonal2DLowerYHigherZ)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y - 1][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			//Higher X lower Y lower Z
			if (whatNode.x != xSizeCache - 1 && whatNode.y != 0 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DHigherXLowerY |
				AddedNeighbours.Diagonal2DHigherXLowerZ |
				AddedNeighbours.Diagonal2DLowerYLowerZ)))
			{
				cache = actualMap[whatNode.x + 1][whatNode.y - 1][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			#endregion 3D walks higher x plane

			#region 3D walks lower x plane

			//Lower X higher Y higher Z
			if (whatNode.x != 0 && whatNode.y != ySizeCache - 1 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DLowerXHigherY |
				AddedNeighbours.Diagonal2DLowerXHigherZ |
				AddedNeighbours.Diagonal2DHigherYHigherZ)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y + 1][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			//Lower X higher Y lower Z
			if (whatNode.x != 0 && whatNode.y != ySizeCache - 1 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DLowerXHigherY |
				AddedNeighbours.Diagonal2DLowerXLowerZ |
				AddedNeighbours.Diagonal2DHigherYLowerZ)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y + 1][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			//Lower X lower Y higher Z
			if (whatNode.x != 0 && whatNode.y != 0 && whatNode.z != zSizeCache - 1 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DLowerXLowerY |
				AddedNeighbours.Diagonal2DLowerXHigherZ |
				AddedNeighbours.Diagonal2DLowerYHigherZ)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y - 1][whatNode.z + 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			//Lower X lower Y lower Z
			if (whatNode.x != 0 && whatNode.y != 0 && whatNode.z != 0 &&
				(canBarelyPassBlocks || addedCache.HasFlag(
				AddedNeighbours.Diagonal2DLowerXLowerY |
				AddedNeighbours.Diagonal2DLowerXLowerZ |
				AddedNeighbours.Diagonal2DLowerYLowerZ)))
			{
				cache = actualMap[whatNode.x - 1][whatNode.y - 1][whatNode.z - 1];
				if (!cache.currentStatus.HasFlag(MapNode.Status.isWall))
				{
					neighbours.Add(new Tuple<MapNode, int>(cache, MapNode.APPROX_3D_DIAGONAL));
				}
			}

			#endregion 3D walks lower x plane

			return neighbours;
		}


		/// <summary>
		/// Resets the map and recalculates the minimal distance to the goal on each node if a new goal was given.
		/// </summary>
		/// <param name="newGoal">The new goal</param>
		public void reinitializeMap(Tuple<int,int,int> newGoal = null, bool clearMap = false)
		{
			foreach (List<List<MapNode>> xBucket in actualMap)
			{
				foreach (List<MapNode> yBucket in xBucket)
				{
					foreach (MapNode zNode in yBucket)
					{
						zNode.initializeNode(goal, clearMap);
					}
				}
			}
			path.Clear();
		}

		public IEnumerator<WorldMap.BlockInfo> GetEnumerator()
		{
			foreach (List<List<MapNode>> xBucket in actualMap)
			{
				foreach (List<MapNode> yBucket in xBucket)
				{
					foreach (MapNode zNode in yBucket)
						yield return new WorldMap.BlockInfo(zNode.x, zNode.y, zNode.z, zNode.currentStatus);
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Start point. Reinitializes the map if it is set to a new non-null value.
		/// </summary>
		public Tuple<int, int, int> start
		{
			get
			{
				return _start;
			}
			set
			{
				if (value != _start)
				{
					_start = value;
					if (value != null)
						reinitializeMap();
				}
			}
		}

		/// <summary>
		/// Goal point. Reinitializes the map if it is set to a new non-null value.
		/// </summary>
		public Tuple<int, int, int> goal
		{
			get
			{
				return _goal;
			}
			set
			{
				if (value != _goal)
				{
					_goal = value;
					if (value != null)
						reinitializeMap(goal);
				}
			}
		}

		/// <summary>
		/// Changes whether the provided block is a wall or not.
		/// </summary>
		/// <param name="whatX">The x coordinate of the block. Null based.</param>
		/// <param name="whatY">The y coordinate of the block. Null based.</param>
		/// <param name="whatZ">The z coordinate of the block. Null based.</param>
		public void toggleWall(int whatX, int whatY, int whatZ)
		{
			MapNode cache = actualMap[whatX][whatY][whatZ];
			cache.currentStatus ^= A_star_Algorithm.MapNode.Status.isWall;

			//If there is any path information, the start is closed.
			if (start != null && actualMap[start.Item1][start.Item2][start.Item3].currentStatus.HasFlag(MapNode.Status.isClosed))
				reinitializeMap();
			else cache.initializeNode(goal, false);
		}

		public LinkedList<BlockInfo> path { get; private set; }
		public bool canBarelyPassBlocks { get; set; }
		public int xSizeCache { get; private set; }
		public int ySizeCache { get; private set; }
		public int zSizeCache { get; private set; }

		private List<List<List<MapNode>>> actualMap = new List<List<List<MapNode>>>();
		private Tuple<int, int, int> _goal;
		private Tuple<int, int, int> _start;
	}
}
