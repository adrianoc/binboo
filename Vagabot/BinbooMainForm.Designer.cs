/**
 * Copyright (c) 2009 Adriano Carlos Verona
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 **/

namespace Binboo
{
	partial class BinbooMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param Name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ContextMenuStrip binbooNotifyPopUp;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BinbooMainForm));
			this.popupJiraAccount = new System.Windows.Forms.ToolStripMenuItem();
			this.popupShowConsole = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.popupExit = new System.Windows.Forms.ToolStripMenuItem();
			this.binbooNotify = new System.Windows.Forms.NotifyIcon(this.components);
			this.txtConsole = new System.Windows.Forms.TextBox();
			binbooNotifyPopUp = new System.Windows.Forms.ContextMenuStrip(this.components);
			binbooNotifyPopUp.SuspendLayout();
			this.SuspendLayout();
			// 
			// binbooNotifyPopUp
			// 
			binbooNotifyPopUp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.popupJiraAccount,
            this.popupShowConsole,
            this.toolStripMenuItem1,
            this.popupExit});
			binbooNotifyPopUp.Name = "vagabotNotifyPopUp";
			binbooNotifyPopUp.Size = new System.Drawing.Size(223, 98);
			// 
			// popupJiraAccount
			// 
			this.popupJiraAccount.Name = "popupJiraAccount";
			this.popupJiraAccount.Size = new System.Drawing.Size(222, 22);
			this.popupJiraAccount.Text = "Set Jira account information";
			// 
			// popupShowConsole
			// 
			this.popupShowConsole.Name = "popupShowConsole";
			this.popupShowConsole.Size = new System.Drawing.Size(222, 22);
			this.popupShowConsole.Text = "Show Console";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(219, 6);
			// 
			// popupExit
			// 
			this.popupExit.Name = "popupExit";
			this.popupExit.Size = new System.Drawing.Size(222, 22);
			this.popupExit.Text = "Exiit";
			// 
			// binbooNotify
			// 
			this.binbooNotify.ContextMenuStrip = binbooNotifyPopUp;
			this.binbooNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("binbooNotify.Icon")));
			this.binbooNotify.Text = "Binboo";
			this.binbooNotify.Visible = true;
			// 
			// txtConsole
			// 
			this.txtConsole.BackColor = System.Drawing.Color.Black;
			this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtConsole.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtConsole.ForeColor = System.Drawing.Color.LawnGreen;
			this.txtConsole.Location = new System.Drawing.Point(0, 0);
			this.txtConsole.Multiline = true;
			this.txtConsole.Name = "txtConsole";
			this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtConsole.Size = new System.Drawing.Size(527, 456);
			this.txtConsole.TabIndex = 1;
			// 
			// BinbooMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 456);
			this.Controls.Add(this.txtConsole);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "BinbooMainForm";
			this.Text = "Binboo Console";
			binbooNotifyPopUp.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NotifyIcon binbooNotify;
		private System.Windows.Forms.ToolStripMenuItem popupJiraAccount;
		private System.Windows.Forms.ToolStripMenuItem popupExit;
		private System.Windows.Forms.ToolStripMenuItem popupShowConsole;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.TextBox txtConsole;
	}
}

