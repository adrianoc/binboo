namespace TCL.Net.UI.Configuration
{
	partial class SearchableConfiguration
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchableConfiguration));
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tpConfigSearch = new System.Windows.Forms.TabPage();
			this.tvControls = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.txtToBeFound = new System.Windows.Forms.TextBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tpConfigSearch.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.tabControl1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(544, 388);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(544, 413);
			this.toolStripContainer1.TabIndex = 2;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tpConfigSearch);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(544, 388);
			this.tabControl1.TabIndex = 1;
			// 
			// tpConfigSearch
			// 
			this.tpConfigSearch.Controls.Add(this.tvControls);
			this.tpConfigSearch.Controls.Add(this.label1);
			this.tpConfigSearch.Controls.Add(this.txtToBeFound);
			this.tpConfigSearch.Location = new System.Drawing.Point(4, 22);
			this.tpConfigSearch.Name = "tpConfigSearch";
			this.tpConfigSearch.Padding = new System.Windows.Forms.Padding(3);
			this.tpConfigSearch.Size = new System.Drawing.Size(536, 362);
			this.tpConfigSearch.TabIndex = 0;
			this.tpConfigSearch.Text = "Configuration search";
			this.tpConfigSearch.UseVisualStyleBackColor = true;
			// 
			// tvControls
			// 
			this.tvControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvControls.Location = new System.Drawing.Point(8, 79);
			this.tvControls.Name = "tvControls";
			this.tvControls.Size = new System.Drawing.Size(520, 277);
			this.tvControls.TabIndex = 2;
			this.tvControls.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvControls_NodeMouseClick);
			this.tvControls.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvControls_KeyUp);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Search";
			// 
			// txtToBeFound
			// 
			this.txtToBeFound.Location = new System.Drawing.Point(55, 43);
			this.txtToBeFound.Name = "txtToBeFound";
			this.txtToBeFound.Size = new System.Drawing.Size(278, 20);
			this.txtToBeFound.TabIndex = 0;
			this.txtToBeFound.TextChanged += new System.EventHandler(this.txtToBeFound_TextChanged);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(35, 25);
			this.toolStrip1.TabIndex = 0;
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "toolStripButton1";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// SearchableConfiguration
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolStripContainer1);
			this.Name = "SearchableConfiguration";
			this.Size = new System.Drawing.Size(544, 413);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tpConfigSearch.ResumeLayout(false);
			this.tpConfigSearch.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tpConfigSearch;
		private System.Windows.Forms.TreeView tvControls;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtToBeFound;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.Timer timer1;
	}
}
