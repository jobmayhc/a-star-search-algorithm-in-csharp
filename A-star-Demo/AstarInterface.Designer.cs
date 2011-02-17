namespace A_star_Demo
{
	partial class AstarInterface
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.glControl1 = new OpenTK.GLControl();
			this.textOutput = new System.Windows.Forms.TextBox();
			this.ClearMap = new System.Windows.Forms.Button();
			this.CalculatePath = new System.Windows.Forms.Button();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.Wall = new System.Windows.Forms.RadioButton();
			this.Start = new System.Windows.Forms.RadioButton();
			this.Goal = new System.Windows.Forms.RadioButton();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.SizeOfMap = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SizeOfMap.SuspendLayout();
			this.SuspendLayout();
			// 
			// glControl1
			// 
			this.glControl1.BackColor = System.Drawing.Color.Black;
			this.glControl1.Location = new System.Drawing.Point(12, 12);
			this.glControl1.Name = "glControl1";
			this.glControl1.Size = new System.Drawing.Size(640, 480);
			this.glControl1.TabIndex = 0;
			this.glControl1.VSync = false;
			this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
			// 
			// textOutput
			// 
			this.textOutput.Location = new System.Drawing.Point(12, 498);
			this.textOutput.Multiline = true;
			this.textOutput.Name = "textOutput";
			this.textOutput.ReadOnly = true;
			this.textOutput.Size = new System.Drawing.Size(640, 56);
			this.textOutput.TabIndex = 1;
			this.textOutput.Text = "Controls are WASD ( TG ) for movement and space for confirmation.";
			// 
			// ClearMap
			// 
			this.ClearMap.Location = new System.Drawing.Point(658, 161);
			this.ClearMap.Name = "ClearMap";
			this.ClearMap.Size = new System.Drawing.Size(122, 46);
			this.ClearMap.TabIndex = 5;
			this.ClearMap.Text = "Clear Map";
			this.ClearMap.UseVisualStyleBackColor = false;
			this.ClearMap.Click += new System.EventHandler(this.ClearMap_Click);
			// 
			// CalculatePath
			// 
			this.CalculatePath.Location = new System.Drawing.Point(658, 328);
			this.CalculatePath.Name = "CalculatePath";
			this.CalculatePath.Size = new System.Drawing.Size(122, 46);
			this.CalculatePath.TabIndex = 6;
			this.CalculatePath.Text = "Calculate Path";
			this.CalculatePath.UseVisualStyleBackColor = false;
			this.CalculatePath.Click += new System.EventHandler(this.CalculatePath_Click);
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Items.AddRange(new object[] {
            "Barely pass walls"});
			this.checkedListBox1.Location = new System.Drawing.Point(658, 213);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(122, 109);
			this.checkedListBox1.TabIndex = 7;
			this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
			// 
			// Wall
			// 
			this.Wall.AutoSize = true;
			this.Wall.ForeColor = System.Drawing.Color.MediumBlue;
			this.Wall.Location = new System.Drawing.Point(658, 12);
			this.Wall.Name = "Wall";
			this.Wall.Size = new System.Drawing.Size(46, 17);
			this.Wall.TabIndex = 8;
			this.Wall.TabStop = true;
			this.Wall.Text = "Wall";
			this.Wall.UseVisualStyleBackColor = true;
			// 
			// Start
			// 
			this.Start.AutoSize = true;
			this.Start.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.Start.Location = new System.Drawing.Point(658, 36);
			this.Start.Name = "Start";
			this.Start.Size = new System.Drawing.Size(47, 17);
			this.Start.TabIndex = 9;
			this.Start.TabStop = true;
			this.Start.Text = "Start";
			this.Start.UseVisualStyleBackColor = true;
			// 
			// Goal
			// 
			this.Goal.AutoSize = true;
			this.Goal.ForeColor = System.Drawing.Color.Green;
			this.Goal.Location = new System.Drawing.Point(658, 60);
			this.Goal.Name = "Goal";
			this.Goal.Size = new System.Drawing.Size(47, 17);
			this.Goal.TabIndex = 10;
			this.Goal.TabStop = true;
			this.Goal.Text = "Goal";
			this.Goal.UseVisualStyleBackColor = true;
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(6, 19);
			this.trackBar1.Minimum = 1;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(110, 42);
			this.trackBar1.TabIndex = 11;
			this.trackBar1.Value = 1;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// SizeOfMap
			// 
			this.SizeOfMap.Controls.Add(this.trackBar1);
			this.SizeOfMap.Location = new System.Drawing.Point(658, 83);
			this.SizeOfMap.Name = "SizeOfMap";
			this.SizeOfMap.Size = new System.Drawing.Size(122, 72);
			this.SizeOfMap.TabIndex = 13;
			this.SizeOfMap.TabStop = false;
			this.SizeOfMap.Text = "Size of Map";
			// 
			// AstarInterface
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.SizeOfMap);
			this.Controls.Add(this.Goal);
			this.Controls.Add(this.Start);
			this.Controls.Add(this.Wall);
			this.Controls.Add(this.checkedListBox1);
			this.Controls.Add(this.CalculatePath);
			this.Controls.Add(this.ClearMap);
			this.Controls.Add(this.textOutput);
			this.Controls.Add(this.glControl1);
			this.KeyPreview = true;
			this.Name = "AstarInterface";
			this.Text = "A* Demo";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AstarInterface_FormClosing);
			this.Load += new System.EventHandler(this.AstarInterface_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AstarInterface_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.SizeOfMap.ResumeLayout(false);
			this.SizeOfMap.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenTK.GLControl glControl1;
		private System.Windows.Forms.TextBox textOutput;
		private System.Windows.Forms.Button ClearMap;
		private System.Windows.Forms.Button CalculatePath;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.RadioButton Wall;
		private System.Windows.Forms.RadioButton Start;
		private System.Windows.Forms.RadioButton Goal;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.GroupBox SizeOfMap;
	}
}

