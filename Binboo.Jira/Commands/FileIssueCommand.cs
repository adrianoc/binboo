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
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Jira.Configuration;
using Binboo.Jira.Integration;
using Binboo.Jira.Plugin;

namespace Binboo.Jira.Commands
{
	internal class FileIssueCommand : JiraCommandBase
	{
		internal const int DefaultIssueOrder = 7;

		public FileIssueCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "File"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			var arguments = CollectAndValidateArguments(context.Arguments,
                                                        project => JiraParamValidator.Project,
														summary => ParamValidator.QuotedString,
														description => ParamValidator.QuotedString.AsOptional(),
                                                        order => JiraParamValidator.Order.AsOptional(),
                                                        type => JiraParamValidator.Type);

			
			Func<RemoteIssue, ICommandResult> mapping = issue => CommandResult.Success(string.Format("Jira tiket created successfuly ({2}).{0}{0}{1}", Environment.NewLine, issue.Format(), UrlFor(issue)), issue.key);

			return Run(
						() => _jira.FileIssue(
									JiraConfig.Instance.IMUserToIssueTrackerUser(context.User.Name),
									arguments["project"].Value,
									arguments["summary"].Value,
									arguments["description"].ValueOrDefault(String.Empty),
									IssueType.Parse(arguments["type"].ValueOrDefault("bug")).Id,
									arguments["order"].ValueOrDefault(DefaultIssueOrder)),

						mapping);
		}
	}
}