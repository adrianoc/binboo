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
using System.Collections.Generic;
using System.Text;
using Binboo.Core.Commands.Support;
using Binboo.Jira.Integration;

namespace Binboo.Jira.Commands
{
	internal class ListProjectsCommand : JiraCommandBase
	{
		public ListProjectsCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "ListProjects"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			CollectAndValidateArguments(context.Arguments);

			IList<string> projectKeys = new List<string>();

			var ret = Run( delegate
			            {
							var projects = new StringBuilder();
							foreach(RemoteProject project in _jira.GetProjectList())
							{
								projectKeys.Add(project.key);
								projects.AppendFormat("{0,-5}{1,-15}{2,-30}{3}", project.key, project.lead, project.description, Environment.NewLine);
							}

			            	return "OK\r\n" + projects;
						});

			return CommandResult.Success(ret, projectKeys);
		}
	}
}
