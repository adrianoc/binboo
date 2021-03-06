﻿/**
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

using System.Collections.Generic;
using System.Text;
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Jira.Integration;

namespace Binboo.Jira.Commands
{
	internal class IssueCommand : JiraCommandBase
	{
		public IssueCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get
			{
				return "Issue";
			}
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
            IDictionary<string, Argument> arguments = CollectAndValidateArguments(context.Arguments, issueId => JiraParamValidator.MultipleIssueId, comments => ParamValidator.Custom("comments", true));

			var sb = new StringBuilder();
			var tikets = arguments["issueId"].Values;

			foreach (var issue in tikets)
			{
				var currentIssue = issue;
				sb.AppendLine(Run(  () => _jira.GetIssue(currentIssue),
									ri => FormatIssue(ri, arguments["comments"].IsPresent))
							 );
			}

			return CommandResult.Success(sb.ToString(), CommaSeparated(tikets));
		}

		private string FormatIssue(RemoteIssue issue, bool showComments)
		{
			string issueDetails = Run(() => issue.Format());
			if (showComments)
			{
				issueDetails = issueDetails + "\r\n" + Run(() => _jira.GetComments(issue.key));
			}

			return issueDetails + "\r\n" + UrlFor(issue);
		}
	}
}
