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
using Binboo.Core.Commands.Arguments;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class ResolveIssueCommand : JiraCommandBase
	{
		public ResolveIssueCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "Resolve"; }
		}

		protected override string ProcessCommand(IContext context)
		{
			IDictionary<string, Argument> arguments = CollectAndValidateArguments(context);
			return CloseIssue(
						arguments["issueId"],
						OptionalArgumentOrDefault(arguments, "comment", string.Empty),
						arguments["resolution"]);
		}

		private IDictionary<string, Argument> CollectAndValidateArguments(IContext context)
		{
			return CollectAndValidateArguments(context.Arguments, 
			                                   issueId => ParamValidator.IssueId, 
			                                   resolution => ParamValidator.From(IssueResolution.FriendlyNames()), 
											   //fixedInVersion => ParamValidator.Version,
			                                   comment => ParamValidator.Anything.AsOptional());
		}

		private string CloseIssue(string ticket, string comment, string resolution)
		{
			try
			{
				_jira.ResolveIssue(ticket, comment, IssueResolution.Parse(resolution), new RemoteVersion[0]);
				return "OK";
			}
			catch(JiraProxyException jipe)
			{
				return jipe.Message + Environment.NewLine + jipe.InnerException.Message;
			}
		}
	}
}
