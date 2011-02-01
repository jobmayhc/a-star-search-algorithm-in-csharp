using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A_star_Demo.A_star_Algorithm
{
	class MapNode
	{
		[Flags]
		public enum Status : byte
		{
			None = 0,
			isWall = (byte)1 << 1,
			isClosed = (byte) 1 << 2
		}

		public MapNode(int whatX, int whatY, int whatZ, Tuple<int,int,int> goal)
		{
			x = whatX;
			y = whatY;
			z = whatZ;
			currentStatus = Status.None;
			initializeNode(goal);
		}

		/// <summary>
		/// The f value
		/// </summary>
		public int possibleDistanceToGoal
		{
			get
			{
				return (currentShortestPathToNode ?? 0) + minimalDistanceToGoal;
			}
		}

		public void initializeNode(Tuple<int,int,int> goal = null, bool clearStatus = true)
		{
			currentPredesessorNode = null;
			currentShortestPathToNode = null;
			currentStatus &= ~Status.isClosed;
			if (clearStatus)
				currentStatus = Status.None;
			if (goal != null)
				calculateMinimalDistanceToGoal(goal);
		}

		public static bool operator ==(MapNode first, Tuple<int, int, int> second)
		{
			if (ReferenceEquals(first, null))
				if (ReferenceEquals(second, null))
					return true;
				else return false;

			return first.Equals(second);
		}

		public static bool operator !=(MapNode first, Tuple<int, int, int> second)
		{
			return !(first == second);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() == typeof(Tuple<int, int, int>))
				return Equals((Tuple<int, int, int>)obj);
			else if (obj.GetType() == typeof(MapNode))
				return Equals((MapNode)obj);
			else return false;
		}

		public bool Equals(Tuple<int, int, int> other)
		{
			if (other != null)
				return x == other.Item1 && y == other.Item2 && z == other.Item3;
			else return false;
		}

		public bool Equals(MapNode other)
		{
			if (other != null)
				return x == other.x && y == other.y && z == other.z;
			else return false;
		}

		public override int GetHashCode()
		{
			return x ^ y ^ z;
		}

		private void calculateMinimalDistanceToGoal(Tuple<int,int,int> goal)
		{
			//Calculate the minimalDistance
			//Positive differences
			int[] diff = new int[3]{
				goal.Item1 > x ? goal.Item1 - x : x - goal.Item1,
				goal.Item2 > y ? goal.Item2 - y : y - goal.Item2,
				goal.Item3 > z ? goal.Item3 - z : z - goal.Item3};
			sortDiffAsc(diff);
			//go 3D diagonal as often as possible, then 2D diagonal as often as possible and then the rest straight
			minimalDistanceToGoal = APPROX_3D_DIAGONAL * diff[0] + APPROX_2D_DIAGONAL * (diff[1] - diff[0]) + BLOCK_DISTANCE * (diff[2] - diff[1]);
		}

		/// <summary>
		/// Sorts an integer array with three elements in ascending order.
		/// Has no checks, only used for code readability.
		/// </summary>
		/// <param name="diff">the array to sort</param>
		private void sortDiffAsc(int[] diff)
		{
			int store;
			if (diff[0] > diff[1])
			{
				if (diff[0] > diff[2])
				{
					store = diff[0];
					diff[0] = diff[2];
					diff[2] = store;
					if (diff[0] > diff[1])
					{
						store = diff[0];
						diff[0] = diff[1];
						diff[1] = store;
					}
				}
				else
				{
					store = diff[0];
					diff[0] = diff[1];
					diff[1] = store;
				}
			}
			else if (diff[1] > diff[2])
			{
				store = diff[1];
				diff[1] = diff[2];
				diff[2] = store;
				if (diff[0] > diff[1])
				{
					store = diff[0];
					diff[0] = diff[1];
					diff[1] = store;
				}
			}
		}

		/// <summary>
		/// The h value
		/// </summary>
		public int minimalDistanceToGoal { get; set; }

		/// <summary>
		/// The g value
		/// </summary>
		public int? currentShortestPathToNode { get; set; }
		public MapNode currentPredesessorNode { get; set; }
		public Status currentStatus { get; set; }
		public readonly int x, y, z;

		public const int APPROX_3D_DIAGONAL = 17;
		public const int APPROX_2D_DIAGONAL = 14;
		public const int BLOCK_DISTANCE = 10;
	}
}
