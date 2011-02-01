using System;
using System.Runtime.InteropServices;
using OpenTK;
using System.Drawing;

namespace A_star_Demo.Graphics.DataStructures
{
	/// <summary>
	/// Vertext data structure that mimics N3fV3f
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct VertexN3fV3f
	{
		Vector3 normal;
		Vector3 position;

		public VertexN3fV3f(Vector3 whatNormal, Vector3 whatPosition)
		{
			position = whatPosition;
			normal = whatNormal;
		}
	}
}
