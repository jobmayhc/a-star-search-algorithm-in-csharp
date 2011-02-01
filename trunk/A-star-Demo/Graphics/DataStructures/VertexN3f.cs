using System;
using System.Runtime.InteropServices;
using OpenTK;
using System.Drawing;

namespace A_star_Demo.Graphics.DataStructures
{
	/// <summary>
	/// Vertext data structure that mimics N3f
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct VertexN3f
	{
		Vector3 position;

		public VertexN3f(Vector3 whatPosition)
		{
			position = whatPosition;
		}
	}
}
