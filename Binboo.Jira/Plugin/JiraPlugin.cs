/**
 * Copyright (c) 2011 Adriano Carlos Verona
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
using System.ComponentModel.Composition;

using Binboo.Core.Commands;
using Binboo.Core.Persistence;
using Binboo.Core.Plugins;

using Binboo.Jira.Commands;
using Binboo.Jira.Configuration;
using Binboo.Jira.Integration;
using Binboo.Jira.Integration.JiraHttp;
using TCL.Net.Net;

namespace Binboo.Jira.Plugin
{
    [Export(typeof(IPlugin))]
    class JiraPlugin : AbstractBasePlugin, IPlugin
    {
        public string Name
        {
            get { return "jira"; }
        }

        [ImportingConstructor]
        JiraPlugin(IStorageManager storageManager)
        {
            _log.InfoFormat("Initializing plugin: {0}.", typeof(JiraPlugin).Name);

            AddCommand(storageManager, new FileIssueCommand(JiraProxy(), Binboo_Jira.File));
            AddCommand(storageManager, new EstimateCommand(JiraProxy(), Binboo_Jira.Estimate));
            AddCommand(storageManager, new ResolveIssueCommand(JiraProxy(), Binboo_Jira.Resolve));
            AddCommand(storageManager, new SearchCommand(JiraProxy(), Binboo_Jira.Search));
            AddCommand(storageManager, new CountIDSCommand(JiraProxy(), Binboo_Jira.CountIDS));
            AddCommand(storageManager, new HelpCommand(this));
            AddCommand(storageManager, new IssueCommand(JiraProxy(), Binboo_Jira.Issue));
            AddCommand(storageManager, new IssueAssignCommand(JiraProxy(), Binboo_Jira.Assign));
            AddCommand(storageManager, new TaskDropperCommand(JiraProxy(), Binboo_Jira.Drop));
            AddCommand(storageManager, new ListProjectsCommand(JiraProxy(), Binboo_Jira.ListProjects));
            AddCommand(storageManager, new PairsCommand(JiraProxy(), Binboo_Jira.Pairs));
            AddCommand(storageManager, new SetOrderCommand(JiraProxy(), Binboo_Jira.SetOrder));
            AddCommand(storageManager, new LinkIssueCommand(JiraProxy(), Binboo_Jira.Link));
            AddCommand(storageManager, new LabelCommand(JiraProxy(), Binboo_Jira.Label));
        }

        private JiraProxy JiraProxy()
        {
            if (_jira == null)
            {
                try
                {
                    _jira = new JiraProxy(
                                    JiraConfig.Instance.EndPoint,
                                    JiraConfig.Instance.User,
                                    new JiraHttpProxy(new SystemNetHttpFactory(), JiraConfig.Instance.HttpInterfaceConfiguration));
                }
                catch (Exception e)
                {
                    //TODO: How to handle such exceptions ???
                    //if (MessageBox.Show("Unable to log user on. Have you copied config file from another machine?", "Error", MessageBoxButtons.OK | MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    //{
                    //    SetJiraAccount(null, EventArgs.Empty);
                    //}

                    //Environment.Exit(-1);
                }
            }

            return _jira;
        }

        private JiraProxy _jira;
    }
}
