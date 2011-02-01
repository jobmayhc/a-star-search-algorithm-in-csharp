using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace A_star_Demo.Graphics
{
	class Camera
	{
		public Camera(Vector3d whatPosition, Vector3d whatTarget)
		{
			position = whatPosition;
			target = whatTarget;
			cameraMatrix = Matrix4d.LookAt(position, target, Vector3d.UnitY);
		}

		public Matrix4d cameraMatrix { get; private set; }
		private Vector3d position;
		private Vector3d target;
	}
}
