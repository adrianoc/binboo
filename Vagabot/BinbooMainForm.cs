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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Forms;
using Binboo.Core;
using Binboo.Core.Events;
using Binboo.Core.Persistence;
using Binboo.Jira.Configuration;
using Microsoft.Win32;

namespace Binboo
{
    //TODO: Detect when no plugins can be fould.
    //TODO: How to handle errors?
    //          - Config file not found
    //          - Failure to resolve imports
    //          - Exceptions in general
    //TODO: How to avoid configuration duplication ?
    public partial class BinbooMainForm : Form
	{
		public BinbooMainForm()
		{
			InitializeComponent();

			InitializeUI();

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
				//_jira = null;
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
            //TODO: Notify plugins that we ate going to unload.
            //if (null != _jira)
            //{
            //    _jira.LogOut();
            //}

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
					//TODO: How to handle this in a better way?
					//CoreConfig.User = new User(f.User, f.Password);	
				}
			}
		}

	    private static ComposablePartCatalog Catalog()
        {
			try
			{
				return new AggregateCatalog(new DirectoryCatalog("Plugins"), new TypeCatalog(typeof(IStorageManager)));
			}
			catch(Exception ex)
			{
				MessageBox.Show("Binboo cannot continue due to an exception during initialization: " + ex);
				Environment.Exit(-1);
				// Make the compiler happy
				return null;
			}

        }
        
        private readonly Core.Application _controler = Core.Application.WithPluginsFrom(Catalog());
	}
}
