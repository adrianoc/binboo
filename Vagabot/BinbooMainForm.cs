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

using System;
using System.Windows.Forms;

using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Configuration;
using Binboo.Core.Events;
using Binboo.JiraIntegration;
using Binboo.JiraIntegration.JiraHttp;
using Microsoft.Win32;
using TCL.Net.Net;

namespace Binboo
{
	public partial class BinbooMainForm : Form
	{
		public BinbooMainForm()
		{
			InitializeComponent();

			InitializeUI();

			InitializeCommmands();

			RegisterForSleepModeNotifications();

			_controler.AttachToSkype();
		}

		protected override void OnShown(EventArgs e)
		{
			Visible = false;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			_controler.Stop();
		}

		//FIXME: I beleieve this has no effect in the application. The proxy are already set in all commands.
		//		 Possible fix is to pass a "ProxyExtractor" delegate to commands. This extractor can be 
		//		 JiraProxy() method.
		private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			if (e.Mode == PowerModes.Resume)
			{
				_jira = null;
			}
		}

		private void InitializeUI()
		{
			ShowInTaskbar = false;

			popupExit.Click += delegate { Close(); };
			popupJiraAccount.Click += SetJiraAccount;

			_controler.Quit += Quit;
			_controler.Error += Error;
			_controler.Attached += Attached;
		}

		private void RegisterForSleepModeNotifications()
		{
			SystemEvents.PowerModeChanged += OnPowerModeChanged;
		}

		private void Quit(object sender, EventArgs e)
		{
			if (null != _jira)
			{
				_jira.LogOut();
			}

			Close();
		}

		private void Attached(object sender, EventArgs e)
		{
			binbooNotify.Icon = Binboo.Connected;
		}

		private void Error(object sender, ErrorEventArgs e)
		{
			binbooNotify.Icon = Binboo.NotConnected;
			binbooNotify.ShowBalloonTip(2000, "Warning", e.Message, ToolTipIcon.Warning);
			binbooNotify.BalloonTipClicked += delegate
			                                  	{
			                                  		MessageBox.Show(e.Details, e.Message);
			                                  	};
		}

		private void SetJiraAccount(object sender, EventArgs e)
		{
			using (var f = new ConfigForm())
			{
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					ConfigServices.User = new JiraUser(f.User, f.Password);	
				}
			}
		}

		private void InitializeCommmands()
		{
			_controler.
				AddCommand(new FileIssueCommand(JiraProxy(), Binboo.File)).
				AddCommand(new EstimateCommand(JiraProxy(), Binboo.Estimate)).
				AddCommand(new ResolveIssueCommand(JiraProxy(), Binboo.Resolve)).
				AddCommand(new SearchCommand(JiraProxy(), Binboo.Search)).
				AddCommand(new CountIDSCommand(JiraProxy(), Binboo.CountIDS)).
				AddCommand(new HelpCommand(_controler, Binboo.Help)).
				AddCommand(new IssueCommand(JiraProxy(), Binboo.Issue)).
				AddCommand(new IssueAssignCommand(JiraProxy(), Binboo.Assign)).
				AddCommand(new TaskDropperCommand(JiraProxy(), Binboo.Drop)).
				AddCommand(new ListProjectsCommand(JiraProxy(), Binboo.ListProjects)).
				AddCommand(new PairsCommand(JiraProxy(), Binboo.Pairs)).
				AddCommand(new SetOrderCommand(JiraProxy(), Binboo.SetOrder));
		}

		private JiraProxy JiraProxy()
		{
			if (_jira == null)
			{
				try
				{
					_jira = new JiraProxy(
									ConfigServices.EndPoint, 
									ConfigServices.User,
									new JiraHttpProxy(new SystemNetHttpFactory(), ConfigServices.HttpInterfaceConfiguration));
				}
				catch(Exception e)
				{
					if (MessageBox.Show("Unable to log user on. Have you copied config file from another machine?", "Error", MessageBoxButtons.OK | MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
					{
						SetJiraAccount(null, EventArgs.Empty);
					}
					
					Environment.Exit(-1);
				}
			}

			return _jira;
		}

		private void popupShowConsole_Click(object sender, EventArgs e)
		{
		}

		private JiraProxy _jira;
		private readonly Core.Application _controler = new Core.Application("Jira");
	}
}
