using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A_star_Demo.A_star_Algorithm
{
	partial class WorldMap
	{
		/// <summary>
		/// Information about blocks in a map that can publically be obtained. This is can i.e. be used for printing the map.
		/// </summary>
		public struct BlockInfo
		{
			public BlockInfo(int whatX, int whatY, int whatZ, MapNode.Status whatStatus)
			{
				x = whatX;
				y = whatY;
				z = whatZ;
				status = whatStatus;
			}

			public readonly int x, y, z;
			public readonly MapNode.Status status;
		}
	}
}
