﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using A_star_Demo.A_star_Algorithm;

namespace A_star_Demo.Graphics
{
	class Renderer : IDisposable
	{
		public enum RMessage
		{
			Abort,
			Reblit
		}

		/// <summary>
		/// Generates a renderthread and graphics related data structures
		/// </summary>
		/// <param name="whatControl">The GLControl to use for rendering</param>
		/// <param name="whatMap">The map to render in the control</param>
		public Renderer(GLControl whatControl, WorldMap whatMap, AstarInterface whatInterface)
		{
			if (whatInterface == null)
				throw new ArgumentNullException("whatInterface", "Cannot get selector position from non-existing interface.");
			AstarInterface = whatInterface;
			if (whatMap == null)
				throw new ArgumentNullException("whatMap", "Cannot render a non-existing map");
			map = whatMap;
			if (whatControl == null)
				throw new ArgumentNullException("whatControl", "Cannot use a non-existing GLControl for rendering");
			GLControl = whatControl;
			GLControl.Context.MakeCurrent(null);

			//Currently hardcoded camera position
			lock (whatMap)
			{
				camera = new Camera(
					new Vector3d((double)whatMap.xSizeCache / 2, (double)whatMap.ySizeCache / 2, (double)whatMap.zSizeCache / 2 + whatMap.xSizeCache + CAMERA_HEIGHT_PADDING),
					new Vector3d((double)whatMap.xSizeCache / 2, (double)whatMap.ySizeCache / 2, (double)whatMap.zSizeCache / 2));
			}
			renderTask = Task.Factory.StartNew(rendererMain, TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Need to stop and release the render thread
		/// </summary>
		public void Dispose()
		{
			sendMessage(RMessage.Abort);
			if (!renderTask.Wait(THREAD_ABORT_TIMEOUT))
				throw new Exception("renderTask did not finish within the given timeout after a cancellation request.");
			renderTask.Dispose();
		}

		/// <summary>
		/// Sends a message to the rendering thread.
		/// </summary>
		/// <param name="whatMessage">The message to send</param>
		public void sendMessage(RMessage whatMessage)
		{
			lock (messageQueue)
			{
				messageQueue.Enqueue(whatMessage);
				System.Threading.Monitor.PulseAll(messageQueue);
			}
		}

		/// <summary>
		/// The render thread. This is the owner of the GLControl and does the actual loop for reblitting.
		/// </summary>
		private void rendererMain()
		{
			//Make the GLControl current on this thread
			GLControl.MakeCurrent();

			//Enable all necessary GL captions
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);
			GL.Enable(EnableCap.ColorMaterial);
			GL.Light(LightName.Light0, LightParameter.Diffuse, OpenTK.Graphics.Color4.White);
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(8.0f, 8.0f, -1.0f, 1.0f));
			GL.Enable(EnableCap.Light0);

			setupViewport();

			//Create the VAOs for several graphic objects
			cubeHandle = new DataStructures.VBOHandle(DataStructures.ObjectName.Cube);
			pyramidHandle = new DataStructures.VBOHandle(DataStructures.ObjectName.Pyramid);
			gridHandle = new DataStructures.VBOHandle(DataStructures.ObjectName.Grid, map.xSizeCache, map.ySizeCache, map.zSizeCache);
			sizeOfGrid = new Tuple<int, int, int>(map.xSizeCache, map.ySizeCache, map.zSizeCache);
			selectorGrid = new DataStructures.VBOHandle(DataStructures.ObjectName.Grid, 1, 1, 1, 1.3f, 1.3f, 1.3f);

			RMessage current;

			//The main loop for rendering. Check and wait for messages and process them
			do
			{
				lock (messageQueue)
				{
					while (!messageQueue.Any())
						System.Threading.Monitor.Wait(messageQueue);
					current = messageQueue.Dequeue();
				}

				switch (current)
				{
					case RMessage.Reblit:
						reblit();
						break;
				}
			}while (!(current == RMessage.Abort));

			//Release VAOs
			cubeHandle.Dispose();
			pyramidHandle.Dispose();
			gridHandle.Dispose();
			selectorGrid.Dispose();
		}

		/// <summary>
		/// Renders the entire scene and updates it on the screen.
		/// </summary>
		private void reblit()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Color3(0.4, 0.4, 0.4);

			//Resize the grid and adjust the camera if necessary
			if (map.xSizeCache != sizeOfGrid.Item1 || map.ySizeCache != sizeOfGrid.Item2 || map.zSizeCache != sizeOfGrid.Item3)
			{
				gridHandle.Dispose();
				gridHandle = new DataStructures.VBOHandle(DataStructures.ObjectName.Grid, map.xSizeCache, map.ySizeCache, map.zSizeCache);
				sizeOfGrid = new Tuple<int, int, int>(map.xSizeCache, map.ySizeCache, map.zSizeCache);

				lock (map)
				{
					camera.moveCamera(new Vector3d((double)map.xSizeCache / 2, (double)map.ySizeCache / 2, (double)map.zSizeCache / 2 + map.xSizeCache + CAMERA_HEIGHT_PADDING));
					camera.changeTarget(new Vector3d((double)map.xSizeCache / 2, (double)map.ySizeCache / 2, (double)map.zSizeCache / 2));
				}
				Matrix4d cameraCache = camera.cameraMatrix;
				GL.LoadMatrix(ref cameraCache);
			}
			
			GL.BindVertexArray(gridHandle.VAOName);

			GL.Translate((float)map.xSizeCache / 2, (float)map.ySizeCache / 2, (float)map.zSizeCache / 2);
			GL.DrawElements(BeginMode.Lines, gridHandle.NumberOfElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
			GL.Translate((float)-map.xSizeCache / 2, (float)-map.ySizeCache / 2, (float)-map.zSizeCache / 2);
			
			GL.Enable(EnableCap.Lighting);
			GL.BindVertexArray(cubeHandle.VAOName);

			lock (map)
			{
				GL.Color3(0.0, 0.0, 1.0);
				foreach (WorldMap.BlockInfo info in map)
				{
					if (info.status.HasFlag(MapNode.Status.isWall))
					{
						GL.Translate(info.x + 0.5, info.y + 0.5, info.z + 0.5);
						GL.DrawElements(BeginMode.Triangles, cubeHandle.NumberOfElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
						GL.Translate(-info.x - 0.5, -info.y - 0.5, -info.z - 0.5);
					}
				}
				if (map.start != null)
				{
					GL.BindVertexArray(pyramidHandle.VAOName);
					GL.Translate(map.start.Item1 + 0.5, map.start.Item2 + 0.5, map.start.Item3 + 0.5);
					GL.Color3(1.0, 0.0, 0.0);
					GL.DrawElements(BeginMode.Triangles, pyramidHandle.NumberOfElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
					GL.Translate(-map.start.Item1 - 0.5, -map.start.Item2 - 0.5, -map.start.Item3 - 0.5);
				}

				if (map.goal != null)
				{
					//This should be much cheaper than just binding the VAO again.
					if (map.start == null)
						GL.BindVertexArray(pyramidHandle.VAOName);

					GL.Translate(map.goal.Item1 + 0.5, map.goal.Item2 + 0.5, map.goal.Item3 + 0.5);
					GL.Color3(0.0, 1.0, 0.0);
					GL.DrawElements(BeginMode.Triangles, pyramidHandle.NumberOfElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
					GL.Translate(-map.goal.Item1 - 0.5, -map.goal.Item2 - 0.5, -map.goal.Item3 - 0.5);
				}

				GL.Color3(1.0, 0.0, 0.0);
				GL.Disable(EnableCap.Lighting);
				if (map.path.Any())
				{
					LinkedListNode<WorldMap.BlockInfo> cache = map.path.First;
					while (cache.Next != null)
					{
						GL.Begin(BeginMode.Lines);
						GL.Vertex3(cache.Value.x + 0.5, cache.Value.y + 0.5, cache.Value.z + 0.5);
						GL.Vertex3(cache.Next.Value.x + 0.5, cache.Next.Value.y + 0.5, cache.Next.Value.z + 0.5);
						GL.End();
						cache = cache.Next;
					}
				}
			}
			//Cache the selector so it cannot change between the pair of GL.Translates
			Vector3d selectorCache = AstarInterface.selector;
			GL.Translate(selectorCache.X + 0.5, selectorCache.Y + 0.5, selectorCache.Z + 0.5);
			GL.Color3(1.0, 0.4, 0.4);
			GL.BindVertexArray(selectorGrid.VAOName);
			GL.DrawElements(BeginMode.Lines, selectorGrid.NumberOfElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
			GL.Translate(-selectorCache.X - 0.5, -selectorCache.Y - 0.5, -selectorCache.Z - 0.5);

			GLControl.SwapBuffers();
			GL.BindVertexArray(0);
		}

		/// <summary>
		/// Initialize a viewport and set up a perspective scene
		/// </summary>
		private void setupViewport()
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Matrix4d cache = Matrix4d.CreatePerspectiveFieldOfView(
				Math.PI / 4,
				GLControl.Width / (double)(GLControl.Height == 0 ? 1 : GLControl.Height),
				1.0,
				100.0);

			GL.LoadMatrix(ref cache);
			GL.Viewport(new Rectangle(0, 0, GLControl.Width, GLControl.Height));

			GL.MatrixMode(MatrixMode.Modelview);
			Matrix4d cameraCache = camera.cameraMatrix;
			GL.LoadMatrix(ref cameraCache);
		}

		private Camera camera;
		private WorldMap map;
		private Task renderTask;
		private GLControl GLControl;
		private Queue<RMessage> messageQueue = new Queue<RMessage>();
		private DataStructures.VBOHandle cubeHandle;
		private DataStructures.VBOHandle pyramidHandle;
		private DataStructures.VBOHandle gridHandle;
		private Tuple<int, int, int> sizeOfGrid;
		private DataStructures.VBOHandle selectorGrid;
		private AstarInterface AstarInterface;
		private const int THREAD_ABORT_TIMEOUT = 1000;
		private const double CAMERA_HEIGHT_PADDING = 1.0;
	}
}
