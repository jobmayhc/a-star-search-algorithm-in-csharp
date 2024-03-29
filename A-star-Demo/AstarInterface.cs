﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using A_star_Demo.Graphics;
using OpenTK;

namespace A_star_Demo
{
	public partial class AstarInterface : Form
	{
		public AstarInterface()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Prints a string in the form's output textbox. This will invoke the form's thread asynchronously.
		/// </summary>
		/// <param name="whatMessage">The text to show</param>
		public void postMessage(string whatMessage)
		{
			//Check if the running thread is the correct one
			if (textOutput.InvokeRequired)
			{
				//Forward to function that does actually parse the text and scroll to the bottom.
				//Invoke to switch thread.
				textOutput.BeginInvoke(new Action<string>(ProcessText), new object[] { whatMessage });
			}
			else
			{
				//Forward to function that does actually parse the text and scroll to the bottom
				ProcessText(whatMessage);
			}
		}

		/// <summary>
		/// Invoked by WriteText(message) - this may only be called from the thread with the controle's underlying window handle.
		/// Prints the message on the form's textbox.
		/// </summary>
		/// <param name="message">network message to print. Handed through by WriteText(message)</param>
		private void ProcessText(string message)
		{
			//Security measure - this may only be called from the thread with the controle's underlying window handle.
			if (textOutput.InvokeRequired)
				throw new System.Exception("private Client.ProcessText was called from the wrong thread");

			//Add timestamp and paste new message into the box. No invoke required.
			textOutput.AppendText(String.Format("{0}[{1}] {2}", Environment.NewLine, System.DateTime.Now.ToLongTimeString(), message));
			//Check if the string is too long and if necessary shorten it.
			DeleteExcessiveLines();
			//Scroll to the bottom
			textOutput.Select(textOutput.Text.Length, 0);
			textOutput.ScrollToCaret();
			textOutput.Invalidate();
		}

		/// <summary>
		/// Checks the length of the text in TextOutputBox and deletes the first lines until it is short enough if needed.
		/// </summary>
		private void DeleteExcessiveLines()
		{
			//Cache for the next carriage return
			int indexCache;

			//Check if the string is too long and if necessary shorten it.
			//search for next carriage return and delete line ( always in form of \r\n )
			while (textOutput.Text.Length >= MAX_CHARACTERS)
			{
				indexCache = textOutput.Text.IndexOf(Environment.NewLine);
				if (indexCache == 0 && textOutput.Text.Length == 0)
					break;
				//the +1 is so it will not delete single lines that are longer than the limit - check if there is a character behind the line.
				if (textOutput.Text.Length >= indexCache + Environment.NewLine.Length + 1)
					//delete the first line ( up to the carriage return )
					textOutput.Text = textOutput.Text.Substring(indexCache + Environment.NewLine.Length, textOutput.Text.Length - indexCache - Environment.NewLine.Length);
				else break;
			}
		}

		private void AstarInterface_Load(object sender, EventArgs e)
		{
			map = new A_star_Algorithm.WorldMap(4, 3, 1, new Tuple<int, int, int>(0, 0, 0), new Tuple<int, int, int>(2, 2, 0));
			Renderer = new Graphics.Renderer(glControl1, map, this);
			rendererOnline = true;
		}

		private Renderer Renderer;
		private bool rendererOnline = false;

		private void glControl1_Paint(object sender, PaintEventArgs e)
		{
			if (rendererOnline)
				Renderer.sendMessage(Renderer.RMessage.Reblit);
		}

		private void AstarInterface_FormClosing(object sender, FormClosingEventArgs e)
		{
			Renderer.Dispose();
		}

		private void CalculatePath_Click(object sender, EventArgs e)
		{
			A_star_Algorithm.WorldMap.Searcher.SearchResult searchResult = A_star_Algorithm.WorldMap.Searcher.SearchResult.None;
			bool startOrGoalMissing = false;
			lock (map)
			{
				if (map.start == null || map.goal == null)
					startOrGoalMissing = true;
				else
				{
					searchResult = A_star_Algorithm.WorldMap.Searcher.searchPathOnMap(map);
					Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
				}
			}
			if (!startOrGoalMissing)
			{
				switch (searchResult)
				{
					case A_star_Algorithm.WorldMap.Searcher.SearchResult.FoundPath:
						postMessage("A shortest path has been calculated.");
						break;
					case A_star_Algorithm.WorldMap.Searcher.SearchResult.None:
						postMessage("No path could be found.");
						break;
					case A_star_Algorithm.WorldMap.Searcher.SearchResult.FoundPath | A_star_Algorithm.WorldMap.Searcher.SearchResult.MapHasAlreadyBeenSearched:
						postMessage("An existing path was found, likely because this map has been searched before.");
						break;
					case A_star_Algorithm.WorldMap.Searcher.SearchResult.MapHasAlreadyBeenSearched:
						postMessage("No path could be found and the map was not in a ready state. Have you searched it before?");
						break;
				}
			}
			else postMessage("Please set both start and goal before trying to calculate a path.");
		}

		private void ClearMap_Click(object sender, EventArgs e)
		{
			lock (map)
			{
				map.reinitializeMap(null, true);
				map.goal = null;
				map.start = null;
				Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
			}
		}

		private void AstarInterface_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
			Vector3d cache;
			if (e.KeyCode == Keys.W)
			{
				if (selector.Y != map.ySizeCache - 1)
				{
					cache = selector;
					cache.Y += 1.0;
					selector = cache;
				}
			}
			else if (e.KeyCode == Keys.A)
			{
				if (selector.X != 0)
				{
					cache = selector;
					cache.X -= 1.0;
					selector = cache;
				}
			}
			else if (e.KeyCode == Keys.S)
			{
				if (selector.Y != 0)
				{
					cache = selector;
					cache.Y -= 1.0;
					selector = cache;
				}
			}
			else if (e.KeyCode == Keys.D)
			{
				if (selector.X != map.xSizeCache - 1)
				{
					cache = selector;
					cache.X += 1.0;
					selector = cache;
				}
			}
			else if (e.KeyCode == Keys.T)
			{
				if (selector.Z != map.zSizeCache - 1)
				{
					cache = selector;
					cache.Z += 1.0;
					selector = cache;
				}
			}
			else if (e.KeyCode == Keys.G)
			{
				if (selector.Z != 0)
				{
					cache = selector;
					cache.Z -= 1.0;
					selector = cache;
				}
			}
			else if (e.KeyCode == Keys.Space)
			{
				lock (map)
				{
					if (Wall.Checked)
					{
						map.toggleWall((int)selector.X, (int)selector.Y, (int)selector.Z);
					}
					if (Start.Checked)
					{
						map.start = new Tuple<int, int, int>((int)selector.X, (int)selector.Y, (int)selector.Z);
					}
					if (Goal.Checked)
					{
						map.goal = new Tuple<int, int, int>((int)selector.X, (int)selector.Y, (int)selector.Z);
					}
				}
			}
			else
			{
				e.Handled = false;
				e.SuppressKeyPress = false;
			}
			if (e.Handled)
				Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
		}

		private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (checkedListBox1.Items[e.Index].ToString() == CAN_BARELY_PASS_WALLS)
			{
				lock (map)
				{
					map.canBarelyPassBlocks = e.NewValue.HasFlag(CheckState.Checked);
					map.reinitializeMap();
					Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
				}
			}
		}

		private const int MAX_CHARACTERS = 2048;

		/// <summary>
		/// This is because checkedListBox only accepts strings, this is a bit stupid.
		/// </summary>
		private const string CAN_BARELY_PASS_WALLS = "Barely pass walls";

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			lock (map)
			{
				map.resizeMap(4 * trackBar1.Value, 3 * trackBar1.Value, 1);
				if (selector.X > map.xSizeCache)
				{
					selector = new Vector3d(map.xSizeCache - 1, selector.Y, selector.Z);
				}
				if (selector.Y > map.ySizeCache)
				{
					selector = new Vector3d(selector.X, map.ySizeCache - 1, selector.Z);
				}
				if (selector.Z > map.zSizeCache)
				{
					selector = new Vector3d(selector.X, selector.Y, map.zSizeCache - 1);
				}
			}
			Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
		}

		private A_star_Algorithm.WorldMap map;
		public Vector3d selector
		{
			get
			{
				lock (Wall)
				{
					return _selector;
				}
			}
			private set
			{
				lock (Wall)
				{
					_selector = value;
				}
			}
		}
		private Vector3d _selector = new Vector3d(0.0, 0.0, 0.0);
	}
}
