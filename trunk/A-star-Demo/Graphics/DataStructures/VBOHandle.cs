using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace A_star_Demo.Graphics.DataStructures
{
	/// <summary>
	/// Possible types for 3D VBO objects.
	/// </summary>
	enum ObjectName
	{
		Cube,
		Pyramid,
		Grid
	}

	/// <summary>
	/// Wrapper for the VBO.
	/// </summary>
	class VBOHandle : IDisposable
	{
		/// <summary>
		/// openGL name for the VBO
		/// </summary>
		public readonly int VBOName;

		/// <summary>
		/// openGL name for the IBO
		/// </summary>
		public readonly int IBOName;

		/// <summary>
		/// Amount of elements in the IBO
		/// </summary>
		public readonly int NumberOfElements;

		/// <summary>
		/// VAO for this handle. Saves the VBO state and usually one IBO state.
		/// </summary>
		public readonly int VAOName;

		/// <summary>
		/// Type of 3D object stored in the VBO
		/// </summary>
		public readonly ObjectName ObjectName;

		/// <summary>
		/// Creates a VBO and an IBO for a 3D object of the given type.
		/// </summary>
		/// <param name="ObjectToCreate">The 3D object that this VBO will contain</param>
		public VBOHandle(ObjectName ObjectToCreate, int x = 1, int y = 1, int z = 1, float xScale = 1, float yScale = 1, float zScale = 1)
		{
			GL.GenVertexArrays(1, out VAOName);
			GL.GenBuffers(1, out VBOName);
			GL.GenBuffers(1, out IBOName);

			if (GL.GetError() != ErrorCode.NoError)
				throw new Exception("Could not generate Buffers");

			switch (ObjectToCreate)
			{
				case ObjectName.Cube:
					NumberOfElements = createCube();
					break;
				case ObjectName.Pyramid:
					NumberOfElements = createPyramid();
					break;
				case ObjectName.Grid:
					NumberOfElements = createGrid(x,y,z,xScale,yScale,zScale);
					break;
			}
			ObjectName = ObjectToCreate;
		}

		/// <summary>
		/// Width and height of a floor grid.
		/// </summary>
		private const int GridSizeX = 1024, GridSizeY = 32;

		/// <summary>
		/// Delete buffers if they havent already been disposed.
		/// </summary>
		~VBOHandle()
		{
#warning this seems to not work
			DeleteBuffers(true);
		}

		/// <summary>
		/// Creates the VBO and IBO for a cube.
		/// </summary>
		/// <returns>Returns the length of the IBO array.</returns>
		private int createCube()
		{
			//List of vertices for a cube.
			VertexN3fV3f[] VertexListCube = new VertexN3fV3f[]
			{
			//Bottom face vertices
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-0.5f, -0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-0.5f, -0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.5f, -0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.5f, -0.5f, 0.5f)),

			//Top face vertices
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(-0.5f, 0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(-0.5f, 0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.5f, 0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.5f, 0.5f, 0.5f)),

			//Left face vertices
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-0.5f, -0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-0.5f, -0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-0.5f, 0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), new Vector3(-0.5f, 0.5f, 0.5f)),

			//Right face vertices
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.5f, -0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.5f, -0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.5f, 0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.5f, 0.5f, 0.5f)),

			//Front face vertices
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-0.5f, -0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-0.5f, 0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.5f, 0.5f, 0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.5f, -0.5f, 0.5f)),

			//Back face vertices
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(-0.5f, -0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(-0.5f, 0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.5f, 0.5f, -0.5f)),
			new VertexN3fV3f(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.5f, -0.5f, -0.5f)),
			};

			//Element list for a cube using triangles and the VertexListCube.
			ushort[] ElementsListCube = new ushort[]
			{
			0, 1, 2, 2, 3, 0, //bottom face
			4, 5, 6, 6, 7, 4, //top face
			8, 9, 10, 10, 11, 8, //left face
			12, 13, 14, 14, 15, 12, //right face
			16, 17, 18, 18, 19, 16, //front face
			20, 21, 22, 22, 23, 20 //back face
			};

			LoadIntoGraphicsMemory(VertexListCube, ElementsListCube);
			return ElementsListCube.Length;
		}

		private int createPyramid()
		{
			Vector3 bottomFront = new Vector3(0.0f, (float)-Math.Sqrt(2) / 3 * SCALE, 1.0f * SCALE);
			Vector3 bottomLeftBack = new Vector3((float)-Math.Cos(Math.PI / 6) * SCALE, (float)-Math.Sqrt(2) / 3 * SCALE, (float)-Math.Sin(Math.PI / 6) * SCALE);
			Vector3 bottomRightBack = new Vector3((float)Math.Cos(Math.PI / 6) * SCALE, (float)-Math.Sqrt(2) / 3 * SCALE, (float)-Math.Sin(Math.PI / 6) * SCALE);
			Vector3 top = new Vector3(0.0f, (float)Math.Sqrt(2) * 2 / 3 * SCALE, 0.0f);

			Vector3 leftFaceNormal = Vector3.Cross(bottomLeftBack - top, bottomFront - top);
			leftFaceNormal.Normalize();

			Vector3 rightFaceNormal = Vector3.Cross(bottomFront - top, bottomRightBack - top);
			rightFaceNormal.Normalize();

			Vector3 backFaceNormal = Vector3.Cross(bottomRightBack - top, bottomLeftBack - top);
			backFaceNormal.Normalize();

			//List of vertices for a cube.
			VertexN3fV3f[] VertexListPyramid = new VertexN3fV3f[]
			{
			//Bottom face vertices
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), bottomFront),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), bottomLeftBack),
			new VertexN3fV3f(new Vector3(0.0f, -1.0f, 0.0f), bottomRightBack),

			//Left face vertices
			new VertexN3fV3f(leftFaceNormal, bottomFront),
			new VertexN3fV3f(leftFaceNormal, bottomLeftBack),
			new VertexN3fV3f(leftFaceNormal, top),

			//Right face vertices
			new VertexN3fV3f(rightFaceNormal, bottomFront),
			new VertexN3fV3f(rightFaceNormal, top),
			new VertexN3fV3f(rightFaceNormal, bottomRightBack),

			//Back face vertices
			new VertexN3fV3f(backFaceNormal, bottomLeftBack),
			new VertexN3fV3f(backFaceNormal, bottomRightBack),
			new VertexN3fV3f(backFaceNormal, top),
			};

			//Element list for a cube using triangles and the VertexListCube.
			ushort[] ElementsListPyramid = new ushort[]
			{
			0, 1, 2, //bottom face
			3, 4, 5, //left face
			6, 7, 8, //right face
			9, 10, 11 //back face
			};

			LoadIntoGraphicsMemory(VertexListPyramid, ElementsListPyramid);
			return ElementsListPyramid.Length;
		}

		/// <summary>
		/// Creates a grid with the specified amount of quaders.
		/// </summary>
		/// <param name="whatX">Amount of quaders in X direction. Zero for being flat in this dimension</param>
		/// <param name="whatY">Amount of quaders in Y direction. Zero for being flat in this dimension</param>
		/// <param name="whatZ">Amount of quaders in Z direction. Zero for being flat in this dimension</param>
		/// <param name="whatXScale">Scaling for each quader in the X dimension</param>
		/// <param name="whatYScale">Scaling for each quader in the Y dimension</param>
		/// <param name="whatZScale">Scaling for each quader in the Z dimension</param>
		/// <returns></returns>
		private int createGrid(int whatX, int whatY, int whatZ, float whatXScale = 1, float whatYScale = 1, float whatZScale = 1)
		{
			//Need one more layer of vertices for the cubes
			VertexN3f[] VertexListGrid = new VertexN3f[(whatX + 1) * (whatY + 1) * (whatZ + 1)];
			for (int a = 0; a <= whatX; a++)
			{
				for (int b = 0; b <= whatY; b++)
				{
					for (int c = 0; c <= whatZ; c++)
					{
						VertexListGrid[(a * (whatY + 1) * (whatZ + 1)) + (b * (whatZ + 1)) + c] = new VertexN3f(new Vector3(
							((float)a - (float)whatX / 2) * whatXScale,
							((float)b - (float)whatY / 2) * whatYScale,
							((float)c - (float)whatZ / 2) * whatZScale));
					}
				}
			}

			ushort[] ElementsListGrid = new ushort[2 * (
				(whatY + 1) * (whatZ + 1) * whatX +			//Lines in X direction
				(whatX + 1) * (whatZ + 1) * whatY +			//Lines in Y direction
				(whatX + 1) * (whatY + 1) * whatZ)];		//Lines in Z direction

			//Helper variable to keep track of the vertices we have finished so far
			int finishedVertexCount = 0;

			//Create lines in X direction
			for (int a = 0; a < whatX; a++)
			{
				for (int b = 0; b <= whatY; b++)
				{
					for (int c = 0; c <= whatZ; c++)
					{
						ElementsListGrid[finishedVertexCount * 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						ElementsListGrid[finishedVertexCount * 2 + 1] = (ushort)((a + 1) * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						finishedVertexCount++;
					}
				}
			}

			//Create lines in Y direction
			for (int b = 0; b < whatY; b++)
			{
				for (int a = 0; a <= whatX; a++)
				{
					for (int c = 0; c <= whatZ; c++)
					{
						ElementsListGrid[finishedVertexCount * 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						ElementsListGrid[finishedVertexCount * 2 + 1] = (ushort)(a * (whatY + 1) * (whatZ + 1) + (b + 1) * (whatZ + 1) + c);
						finishedVertexCount++;
					}
				}
			}

			//Create lines in Z direction
			for (int c = 0; c < whatZ; c++)
			{
				for (int a = 0; a <= whatX; a++)
				{
					for (int b = 0; b <= whatY; b++)
					{
						ElementsListGrid[finishedVertexCount * 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						ElementsListGrid[finishedVertexCount * 2 + 1] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c + 1);
						finishedVertexCount++;
					}
				}
			}


			/* Stupid old version that originates in me having done a similar thing years ago and just using the same counting method again.
			 * This works but is highly unintuitive and too complex. Dont want to drag the mistake from back then over :)
			 * 
			//Each line needs two vertices.
			//Three lines for every vertex that is not on the positive maximum x, y or z plane
			//and additional finish lines at the positive maximum planes
			//( beware of overlap! its not as first thought i.e. "whatX * (whatY + 1 + whatZ + 1)" because two planes overlap in one line )
			ushort[] ElementsListGrid = new ushort[2 * (
				3 * whatX * whatY * whatZ +
				whatX * (whatY + whatZ + 1) +
				whatY * (whatX + whatZ + 1) + 
				whatZ * (whatX + whatY + 1))];

			//Helper variable to keep track of the vertex we are on ( especially how many we already finished )
			int finishedVertexCounter3D = 0;

			//These are three lines ( one in X, one in Y and one in Z direction, positive )
			//for every vertex that is not on the maximum positive plane of any direction.
			for (int a = 0; a < whatX; a++)
			{
				for (int b = 0; b < whatY; b++)
				{
					for (int c = 0; c < whatZ; c++)
					{
						//Line in X direction
						ElementsListGrid[finishedVertexCounter3D * 2 * 3] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						ElementsListGrid[finishedVertexCounter3D * 2 * 3 + 1] = (ushort)((a + 1) * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);

						//Line in Y direction
						ElementsListGrid[finishedVertexCounter3D * 2 * 3 + 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						ElementsListGrid[finishedVertexCounter3D * 2 * 3 + 3] = (ushort)(a * (whatY + 1) * (whatZ + 1) + (b + 1) * (whatZ + 1) + c);

						//Line in Z direction
						ElementsListGrid[finishedVertexCounter3D * 2 * 3 + 4] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
						ElementsListGrid[finishedVertexCounter3D * 2 * 3 + 5] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c + 1);

						finishedVertexCounter3D++;
					}
				}
			}

			//Now for the remaining lines on the maximum positive planes

			//Helper variable to keep track of the vertex we are on ( especially how many we already finished )
			int finishedVertexCounter2D = 0;

			//Max X plane ( but aswell doesnt reach max Y or max Z )
			for (int b = 0; b < whatY; b++)
			{
				for (int c = 0; c < whatZ; c++)
				{
					//Line in Y direction
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 1] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + (b + 1) * (whatZ + 1) + c);

					//Line in Z direction
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 2] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c);
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 3] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + c + 1);

					finishedVertexCounter2D++;
				}
			}

			//Max Y plane ( but aswell doesnt reach max X or max Z )
			for (int a = 0; a < whatX; a++)
			{
				for (int c = 0; c < whatZ; c++)
				{
					//Line in X direction
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + c);
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 1] = (ushort)((a + 1) * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + c);

					//Line in Z direction
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + c);
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 3] = (ushort)(a * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + c + 1);

					finishedVertexCounter2D++;
				}
			}

			//Max Z plane ( but aswell doesnt reach max X or max Y )
			for (int a = 0; a < whatX; a++)
			{
				for (int b = 0; b < whatY; b++)
				{
					//Line in X direction
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + whatZ);
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 1] = (ushort)((a + 1) * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + whatZ);

					//Line in Y direction
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + whatZ);
					ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + 3] = (ushort)(a * (whatY + 1) * (whatZ + 1) + (b + 1) * (whatZ + 1) + whatZ);

					finishedVertexCounter2D++;
				}
			}

			//Now for the final lines, the ones that are on two 2D planes at once

			//Helper variable to keep track of the vertex we are on ( especially how many we already finished )
			int finishedVertexCounter1D = 0;

			//Max Y Max Z, the X-Line.
			for (int a = 0; a < whatX; a++)
			{
				ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + finishedVertexCounter1D * 2] = (ushort)(a * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + whatZ);
				ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + finishedVertexCounter1D * 2 + 1] = (ushort)((a + 1) * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + whatZ);

				finishedVertexCounter1D++;
			}

			//Max X Max Z, the Y-Line.
			for (int b = 0; b < whatY; b++)
			{
				ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + finishedVertexCounter1D * 2] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + b * (whatZ + 1) + whatZ);
				ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + finishedVertexCounter1D * 2 + 1] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + (b + 1) * (whatZ + 1) + whatZ);

				finishedVertexCounter1D++;
			}

			//Max X Max Y, the Z-Line.
			for (int c = 0; c < whatZ; c++)
			{
				ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + finishedVertexCounter1D * 2] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + c);
				ElementsListGrid[finishedVertexCounter3D * 2 * 3 + finishedVertexCounter2D * 2 * 2 + finishedVertexCounter1D * 2 + 1] = (ushort)(whatX * (whatY + 1) * (whatZ + 1) + whatY * (whatZ + 1) + c + 1);

				finishedVertexCounter1D++;
			}*/

			LoadIntoGraphicsMemory(VertexListGrid, ElementsListGrid);

			return ElementsListGrid.Length;
		}

		/// <summary>
		/// Moves the given data into graphics memory.
		/// </summary>
		/// <param name="VertexList">List of vertices for the VBO</param>
		/// <param name="ElementsList">List of elements for the IBO</param>
		private void LoadIntoGraphicsMemory(VertexN3fV3f[] VertexList, ushort[] ElementsList)
		{
			GL.BindVertexArray(VAOName);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.EnableClientState(ArrayCap.NormalArray);

			writeBuffer(VertexList, ElementsList);

			GL.NormalPointer(NormalPointerType.Float, BlittableValueType.StrideOf(VertexList), IntPtr.Zero);
			GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(VertexList), new IntPtr(3 * sizeof(float)));

			GL.BindVertexArray(0);
		}

		/// <summary>
		/// Moves the given data into graphics memory.
		/// </summary>
		/// <param name="VertexList">List of vertices for the VBO</param>
		/// <param name="ElementsList">List of elements for the IBO</param>
		private void LoadIntoGraphicsMemory(VertexN3f[] VertexList, ushort[] ElementsList)
		{
			GL.BindVertexArray(VAOName);
			GL.EnableClientState(ArrayCap.VertexArray);

			writeBuffer(VertexList, ElementsList);

			GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(VertexList), IntPtr.Zero);

			GL.BindVertexArray(0);
		}

		/// <summary>
		/// This may ONLY be called with a bound VAO and accordingly enabled captions. Writes the data out to the graphics card memory.
		/// </summary>
		/// <typeparam name="T">The kind of vertex-structure to use</typeparam>
		/// <param name="VertexList">List of vertices for the VBO</param>
		/// <param name="ElementsList">List of elements for the IBO</param>
		private void writeBuffer<T>(T[] VertexList, ushort[] ElementsList)
			where T : struct
		{
			int size = 0;

			GL.BindBuffer(BufferTarget.ArrayBuffer, VBOName);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexList.Length * BlittableValueType.StrideOf(VertexList)), VertexList, BufferUsageHint.StaticDraw);
			GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
			if (size != VertexList.Length * BlittableValueType.StrideOf(VertexList))
				throw new Exception("Vertex data not uploaded correctly");

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBOName);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(ElementsList.Length * sizeof(ushort)), ElementsList, BufferUsageHint.StaticDraw);
			GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
			if (size != ElementsList.Length * sizeof(ushort))
				throw new Exception("Element data not uploaded correctly");
		}

		/// <summary>
		/// Delete buffers.
		/// </summary>
		public void Dispose()
		{
			DeleteBuffers(false);
		}

		/// <summary>
		/// Delete the VBO and the IBO.
		/// </summary>
		public void DeleteBuffers(bool Finalizer)
		{
			int WasteRefParam = VBOName;
			GL.DeleteBuffers(1, ref WasteRefParam);
			WasteRefParam = IBOName;
			GL.DeleteBuffers(1, ref WasteRefParam);
			if (!Finalizer)
				GC.SuppressFinalize(this);
		}

		private const float SCALE = 0.2f;
	}
}
