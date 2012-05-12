/**
 * Copyright (c) 2012 Adriano Carlos Verona
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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TCL.Net.UI.Configuration
{
	public partial class SearchableConfiguration : UserControl
	{
		public SearchableConfiguration()
		{
			InitializeComponent();

			tvControls.Font = new Font(tvControls.Font, FontStyle.Bold);
			regularNodeFont = new Font(tvControls.Font, FontStyle.Regular);

			ActiveControl = txtToBeFound;

		}

		public void AddPages(params TabPage[] pages)
		{
			tabControl1.TabPages.AddRange(pages);
		}

		private void txtToBeFound_TextChanged(object sender, EventArgs e)
		{
			if (lastFired + 500 > DateTime.Now.Ticks) return;

			lastFired = DateTime.Now.Ticks;
			tvControls.Nodes.Clear();
			if (string.IsNullOrWhiteSpace(txtToBeFound.Text)) return;

			timer1.Start();
			var root = tvControls.Nodes.Add("Configuration Items");

			foreach (Control ctrl in Controls)
			{
				TryAddControl(root, ctrl, txtToBeFound.Text);
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (last != null)
			{
				ActiveControl = last;
				DumpInfo("[Activate] Control={0}, Next = {1}", last, last.GetNextControl(last, true));
				last = null;
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			tabControl1.SelectTab(0);
		}

		private void tvControls_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			EnsureFocused(e.Node);
		}

		private void tvControls_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				EnsureFocused(tvControls.SelectedNode);
			}
		}

		private void EnsureFocused(TreeNode node)
		{
			var selected = node.Tag as Control;
			if (selected != null)
			{
				EnsureFocused(selected);
			}
		}

		private bool TryAddControl(TreeNode root, Control ctrl, string tbf)
		{
			if (ctrl is Label) return false;
			if (ctrl == tpConfigSearch) return false;

			bool isLabelTargetMach = (ctrl is TextBox && ctrl.Tag != null && IsMatch(((Control)ctrl.Tag).Text, tbf));

			if (!isLabelTargetMach && !IsMatch(ctrl.Text, tbf) && ctrl.Controls.Count == 0)
			{
				DumpInfo("[{0}] discarded. TBF={1}", ctrl.Text, tbf);
				return false;
			}

			TreeNode current;

			if (ctrl is GroupBox || ctrl is TabPage || !ctrl.HasChildren)
			{
				current = root.Nodes.Add(LabelFor(ctrl));
				current.Parent.Expand();
				current.Tag = ctrl;
			}
			else
			{
				current = root;
			}

			var hasChildren = AddChildControls(current, ctrl.Controls, tbf);
			if (!hasChildren && IsGroupingControl(ctrl))
			{
				if (current != root)
				{
					DumpInfo("[{0}] removed.", current.Text);
					current.Remove();
				}
				return false;
			}

			if (hasChildren)
			{
				current.NodeFont = regularNodeFont;
			}

			return true;
		}

		private bool IsMatch(string text, string tbf)
		{
			Regex regex = new Regex(".*" + tbf + ".*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			return regex.IsMatch(text);
		}

		private bool IsGroupingControl(Control ctrl)
		{
			return ctrl is GroupBox || ctrl is Form || ctrl is TabPage || ctrl is SplitterPanel || ctrl is SplitContainer;
		}

		private string LabelFor(Control ctrl)
		{
			if (ctrl.Tag is Label)
			{
				return ((Label)ctrl.Tag).Text;
			}

			var label = string.IsNullOrWhiteSpace(ctrl.Text) ? ctrl.Name : ctrl.Text;
			return (string.IsNullOrWhiteSpace(label) ? ctrl.GetType().Name : label) + string.Empty;
		}

		private bool AddChildControls(TreeNode root, Control.ControlCollection controls, string tbf)
		{
			bool ret = false;
			foreach (Control control in controls)
			{
				ret |= TryAddControl(root, control, tbf);
			}

			return ret;
		}

		private void EnsureFocused(Control selected)
		{
			if (selected is Form) return;

			if (selected.Parent != null)
			{
				EnsureFocused(selected.Parent);
			}

			DumpInfo("Focusing : {0}", selected.Name);

			TabPage tb = selected as TabPage;
			if (tb != null)
			{
				tabControl1.SelectedTab = tb;
				return;
			}

			last = selected;
		}

		[Conditional("DEBUG")]
		private void DumpInfo(string focusing, params object[] args)
		{
			Console.WriteLine(focusing, args);
		}

		private long lastFired = -1;
		private Control last;
		private readonly Font regularNodeFont;
	}

}
