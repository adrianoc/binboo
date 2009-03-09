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
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class CloseIssueCommand : JiraCommandBase
	{
		public CloseIssueCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "Close"; }
		}

		public override string Process(Context context)
		{
			string[] args = context.Arguments;

			string result = CheckParameters(args, ParamValidator.TicketNumber, ParamValidator.Anything, ParamValidator.Anything.AsOptional());
			return result ?? 
					CloseIssue(
						args[0], 
						args[1],
						OptionalParameterOrDefault(args, 2, ParamValidator.Anything, string.Empty));
		}

		private string CloseIssue(string ticket, string resolution, string description)
		{
			try
			{
				_jira.CloseIssue(ticket, description, IssueResolution.Parse(resolution));
				return "OK";
			}
			catch(JiraProxyException jipe)
			{
				return jipe.Message + Environment.NewLine + jipe.InnerException.Message;
			}
		}
	}
}
