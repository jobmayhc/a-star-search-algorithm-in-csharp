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
			/// <summary>
			/// Searches a path on a given map. The map data is altered in the process and a path is set on it.
			/// </summary>
			/// <param name="whatMap">The map to search a path on</param>
			/// <returns>Whether a path could be found</returns>
			public static bool searchPathOnMap(WorldMap whatMap)
			{
				if (whatMap.goal == null || whatMap.start == null)
					throw new ArgumentException("The provided map does either not have a start or a goal.", "whatMap");

				bool foundPath = false;

				//Initialize open list
				SortedDictionary<int, List<MapNode>> openList = new SortedDictionary<int, List<MapNode>>();
				MapNode cache = whatMap.actualMap[whatMap.start.Item1][whatMap.start.Item2][whatMap.start.Item3];
				cache.currentShortestPathToNode = 0;
				openList.Add(cache.possibleDistanceToGoal, new List<MapNode>() { cache });

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
					//The actual searching. Take the node with the possibly least remaining distance ( estimated ) and expand it
					while (openList.Any())
					{
						cache = openList.First().Value.First();

						//remove it from the open list and possibly the entire entry list if empty
						openList[cache.possibleDistanceToGoal].Remove(cache);
						if (!openList[cache.possibleDistanceToGoal].Any())
							openList.Remove(cache.possibleDistanceToGoal);

						//Have we found the goal?
						if (cache == whatMap.goal)
						{
							foundPath = true;
							break;
						}
						cache.currentStatus |= MapNode.Status.isClosed;

						expandNode(cache, openList, whatMap);
					}

					//Create the path information on the map data ( This is an extra list for simplicity, the main data has been altered already above )
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

			/// <summary>
			/// Checks all neighbour nodes whether this node provides a shorter ( or new, thats trivially shorter ) path to it
			/// and update the data on the map accordingly
			/// </summary>
			/// <param name="whatNode">The node to expand</param>
			/// <param name="openList">A link to the current open list. As this function is just for code readability, this is plainly forwarded</param>
			/// <param name="whatMap">The map that is currently searched on</param>
			private static void expandNode(MapNode whatNode, SortedDictionary<int, List<MapNode>> openList, WorldMap whatMap)
			{
				foreach (Tuple<MapNode, int> reachableOpenNode in whatMap.getNeighbours(whatNode))
				{
					if (reachableOpenNode.Item1.currentShortestPathToNode == null || reachableOpenNode.Item1.currentShortestPathToNode > whatNode.currentShortestPathToNode + reachableOpenNode.Item2)
					{
						if (reachableOpenNode.Item1.currentStatus.HasFlag(MapNode.Status.isClosed))
							throw new Exception("Node is closed but currently known path is not the shortest. If the algorithm is working properly, this is impossible. This could be due to rounding errors but still hasn't been confirmed, so no further check is implemented.");
						
						//Remove the node from the open list because it needs to be put in another sublist. Remove current sublist if empty
						if (reachableOpenNode.Item1.currentShortestPathToNode != null)
						{
							openList[reachableOpenNode.Item1.possibleDistanceToGoal].Remove(reachableOpenNode.Item1);
							if (!openList[reachableOpenNode.Item1.possibleDistanceToGoal].Any())
								openList.Remove(reachableOpenNode.Item1.possibleDistanceToGoal);
						}

						//Set the data on the updated node
						reachableOpenNode.Item1.currentPredesessorNode = whatNode;
						reachableOpenNode.Item1.currentShortestPathToNode = whatNode.currentShortestPathToNode + reachableOpenNode.Item2;

						//Put the node in the correct sublist. Create a new sublist if it doesnt already exist
						if (!openList.ContainsKey(reachableOpenNode.Item1.possibleDistanceToGoal))
							openList.Add(reachableOpenNode.Item1.possibleDistanceToGoal, new List<MapNode>());

						openList[reachableOpenNode.Item1.possibleDistanceToGoal].Add(reachableOpenNode.Item1);
					}
				}
			}

			/// <summary>
			/// This is a counter to avoid corrupted maps with circles as paths to permanently block the searcher
			/// </summary>
			private const int CIRCLE_TIMEOUT = 2048;
		}
	}
}
