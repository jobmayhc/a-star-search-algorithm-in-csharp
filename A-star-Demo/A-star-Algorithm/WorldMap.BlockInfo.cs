using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A_star_Demo.A_star_Algorithm
{
	partial class WorldMap
	{
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
