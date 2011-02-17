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

		public void moveCamera(Vector3d whatPosition)
		{
			Vector3d normalizedConnection = Vector3d.NormalizeFast(position - target);
			double cosine = Vector3d.Dot(normalizedConnection, Vector3d.UnitY);
			if (!(cosine < 1.02 && cosine > 0.98) && !(cosine > -1.02 && cosine < -0.98))
			{
				position = whatPosition;
				cameraMatrix = Matrix4d.LookAt(position, target, Vector3d.UnitY);
			}
			
		}

		public void changeTarget(Vector3d whatTarget)
		{
			Vector3d normalizedConnection = Vector3d.NormalizeFast(position - target);
			double cosine = Vector3d.Dot(normalizedConnection, Vector3d.UnitY);
			if (!(cosine < 1.02 && cosine > 0.98) && !(cosine > -1.02 && cosine < -0.98))
			{
				target = whatTarget;
				cameraMatrix = Matrix4d.LookAt(position, target, Vector3d.UnitY);
			}
		}

		public Matrix4d cameraMatrix { get; private set; }
		private Vector3d position;
		private Vector3d target;
	}
}
