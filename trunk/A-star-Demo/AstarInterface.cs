using System;
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

		private void AstarInterface_Load(object sender, EventArgs e)
		{
			map = new A_star_Algorithm.WorldMap(20, 15, 1, new Tuple<int, int, int>(0, 0, 0), new Tuple<int, int, int>(19, 14, 0));
			Renderer = new Graphics.Renderer(glControl1, map);
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

		private A_star_Algorithm.WorldMap map;
		private Vector3d selector = new Vector3d(0.0, 0.0, 0.0);

		private void CalculatePath_Click(object sender, EventArgs e)
		{
			lock (map)
			{
				A_star_Algorithm.WorldMap.Searcher.searchPathOnMap(map);
				Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
			}
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
			if (e.KeyCode == Keys.W)
			{
				if (selector.Y != map.ySizeCache - 1)
				{
					selector.Y += 1.0;
					Renderer.selector = selector;
				}
			}
			else if (e.KeyCode == Keys.A)
			{
				if (selector.X != 0)
				{
					selector.X -= 1.0;
					Renderer.selector = selector;
				}
			}
			else if (e.KeyCode == Keys.S)
			{
				if (selector.Y != 0)
				{
					selector.Y -= 1.0;
					Renderer.selector = selector;
				}
			}
			else if (e.KeyCode == Keys.D)
			{
				if (selector.X != map.xSizeCache - 1)
				{
					selector.X += 1.0;
					Renderer.selector = selector;
				}
			}
			else if (e.KeyCode == Keys.T)
			{
				if (selector.Z != map.zSizeCache - 1)
				{
					selector.Z += 1.0;
					Renderer.selector = selector;
				}
			}
			else if (e.KeyCode == Keys.G)
			{
				if (selector.Z != 0)
				{
					selector.Z -= 1.0;
					Renderer.selector = selector;
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
			else e.Handled = false;
			if (e.Handled)
				Renderer.sendMessage(Graphics.Renderer.RMessage.Reblit);
		}
	}
}
