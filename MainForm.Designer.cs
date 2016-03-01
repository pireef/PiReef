/*
 * Created by SharpDevelop.
 * User: brett
 * Date: 2/16/2016
 * Time: 5:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PiReefComplete
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStrip;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblAirTemp;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label lblWaterTemp;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label lblPH;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.Timer DataUpdateTimer;
		private ZedGraph.ZedGraphControl graph;
		private System.Windows.Forms.Timer chartUpdateTimer;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStrip = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblAirTemp = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lblWaterTemp = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lblPH = new System.Windows.Forms.Label();
			this.DataUpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.graph = new ZedGraph.ZedGraphControl();
			this.chartUpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStrip});
			this.statusStrip1.Location = new System.Drawing.Point(0, 558);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(925, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStrip
			// 
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(118, 17);
			this.toolStrip.Text = "toolStripStatusLabel1";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.controlToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(925, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.settingsToolStripMenuItem,
			this.aboutToolStripMenuItem,
			this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.settingsToolStripMenuItem.Text = "Settings";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			// 
			// controlToolStripMenuItem
			// 
			this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
			this.controlToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
			this.controlToolStripMenuItem.Text = "Control";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblAirTemp);
			this.groupBox1.Location = new System.Drawing.Point(12, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(155, 150);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Air Temperature";
			// 
			// lblAirTemp
			// 
			this.lblAirTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblAirTemp.Location = new System.Drawing.Point(8, 16);
			this.lblAirTemp.Name = "lblAirTemp";
			this.lblAirTemp.Size = new System.Drawing.Size(142, 81);
			this.lblAirTemp.TabIndex = 0;
			this.lblAirTemp.Text = "label1";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lblWaterTemp);
			this.groupBox2.Location = new System.Drawing.Point(12, 181);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(155, 150);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Water Temperature";
			// 
			// lblWaterTemp
			// 
			this.lblWaterTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblWaterTemp.Location = new System.Drawing.Point(8, 20);
			this.lblWaterTemp.Name = "lblWaterTemp";
			this.lblWaterTemp.Size = new System.Drawing.Size(142, 77);
			this.lblWaterTemp.TabIndex = 0;
			this.lblWaterTemp.Text = "label1";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.lblPH);
			this.groupBox3.Location = new System.Drawing.Point(12, 341);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(155, 150);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "PH";
			// 
			// lblPH
			// 
			this.lblPH.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPH.Location = new System.Drawing.Point(7, 20);
			this.lblPH.Name = "lblPH";
			this.lblPH.Size = new System.Drawing.Size(140, 77);
			this.lblPH.TabIndex = 0;
			this.lblPH.Text = "label1";
			// 
			// DataUpdateTimer
			// 
			this.DataUpdateTimer.Enabled = true;
			this.DataUpdateTimer.Tick += new System.EventHandler(this.DataUpdateTimerTick);
			// 
			// graph
			// 
			this.graph.AutoSize = true;
			this.graph.Location = new System.Drawing.Point(182, 27);
			this.graph.Name = "graph";
			this.graph.ScrollGrace = 0D;
			this.graph.ScrollMaxX = 0D;
			this.graph.ScrollMaxY = 0D;
			this.graph.ScrollMaxY2 = 0D;
			this.graph.ScrollMinX = 0D;
			this.graph.ScrollMinY = 0D;
			this.graph.ScrollMinY2 = 0D;
			this.graph.Size = new System.Drawing.Size(731, 464);
			this.graph.TabIndex = 5;
			this.graph.UseExtendedPrintDialog = true;
			// 
			// chartUpdateTimer
			// 
			this.chartUpdateTimer.Enabled = true;
			this.chartUpdateTimer.Interval = 300000;
			this.chartUpdateTimer.Tick += new System.EventHandler(this.ChartUpdateTimerTick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(925, 580);
			this.Controls.Add(this.graph);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "PiReefComplete";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
