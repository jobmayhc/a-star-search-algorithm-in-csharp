using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A_star_Demo.A_star_Algorithm
{
	partial class WorldMap
	{
		/// <summary>
		/// This class requires access to the real data of the map. It can try to find a shortest path on a map from start to goal.
		/// </summary>
		public static class Searcher
		{
			public static bool searchPathOnMap(WorldMap whatMap)
			{
				if (whatMap.goal == null || whatMap.start == null)
					throw new ArgumentException("The provided map does either not have a start or a goal.", "whatMap");

				bool foundPath = false;

				//Initialize open list
				SortedDictionary<int, List<MapNode>> openList = new SortedDictionary<int, List<MapNode>>();
				MapNode cache = whatMap.actualMap[whatMap.start.Item1][whatMap.start.Item2][whatMap.start.Item3];

				//Very simple test whether it makes sense to search the map at all
				if (cache.currentStatus == MapNode.Status.isClosed)
				{
					for (int i = 0; i < CIRCLE_TIMEOUT && cache != whatMap.start; i++)
					{
						if (cache.currentPredesessorNode == null)
							throw new Exception("Tried to search on a map that has been corrupted.");
						cache = cache.currentPredesessorNode;
					}
					if (cache != whatMap.start)
						throw new Exception("Map has been altered to contain a circle as a path");
					else foundPath = true;
				}
				else
				{
					cache.currentShortestPathToNode = 0;
					openList.Add(cache.possibleDistanceToGoal, new List<MapNode>() { cache });

					while (openList.Any())
					{
						cache = openList.First().Value.First();
						openList[cache.possibleDistanceToGoal].Remove(cache);
						if (!openList[cache.possibleDistanceToGoal].Any())
							openList.Remove(cache.possibleDistanceToGoal);
						if (cache == whatMap.goal)
						{
							foundPath = true;
							break;
						}
						cache.currentStatus |= MapNode.Status.isClosed;

						expandNode(cache, openList, whatMap);
					}

					LinkedList<BlockInfo> path = new LinkedList<BlockInfo>();
					if (foundPath)
					{
						cache = whatMap.actualMap[whatMap.goal.Item1][whatMap.goal.Item2][whatMap.goal.Item3];
						while (cache.currentPredesessorNode != null)
						{
							path.AddFirst(new BlockInfo(cache.x, cache.y, cache.z, cache.currentStatus));
							cache = cache.currentPredesessorNode;
						}
						//Add the start aswell even though its predecessor is null ( obviously )
						path.AddFirst(new BlockInfo(cache.x, cache.y, cache.z, cache.currentStatus));
					}
					whatMap.path = path;
				}
				return foundPath;
			}

			private static void expandNode(MapNode whatNode, SortedDictionary<int, List<MapNode>> openList, WorldMap whatMap)
			{
				foreach (Tuple<MapNode, int> reachableOpenNode in whatMap.getNeighbours(whatNode))
				{
					if (reachableOpenNode.Item1.currentShortestPathToNode == null || reachableOpenNode.Item1.currentShortestPathToNode > whatNode.currentShortestPathToNode + reachableOpenNode.Item2)
					{
						if (reachableOpenNode.Item1.currentStatus.HasFlag(MapNode.Status.isClosed))
							throw new Exception("Node is closed but currently known path is not the shortest. If the algorithm is working properly, this is impossible. This could be due to rounding errors but still hasn't been confirmed, so no further check is implemented.");
						if (reachableOpenNode.Item1.currentShortestPathToNode != null)
						{
							openList[reachableOpenNode.Item1.possibleDistanceToGoal].Remove(reachableOpenNode.Item1);
							if (!openList[reachableOpenNode.Item1.possibleDistanceToGoal].Any())
								openList.Remove(reachableOpenNode.Item1.possibleDistanceToGoal);
						}

						reachableOpenNode.Item1.currentPredesessorNode = whatNode;
						reachableOpenNode.Item1.currentShortestPathToNode = whatNode.currentShortestPathToNode + reachableOpenNode.Item2;

						if (!openList.ContainsKey(reachableOpenNode.Item1.possibleDistanceToGoal))
							openList.Add(reachableOpenNode.Item1.possibleDistanceToGoal, new List<MapNode>());

						openList[reachableOpenNode.Item1.possibleDistanceToGoal].Add(reachableOpenNode.Item1);
					}
				}
			}

			private const int CIRCLE_TIMEOUT = 2048;
		}
	}
}
