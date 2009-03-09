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
using System.Linq;
using System.Text;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class SearchCommand : JiraCommandBase
	{
		public SearchCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "Search"; }
		}

		public override string Process(Context context)
		{
			string[] args = context.Arguments;

			string ret = CheckParameters(args, ParamValidator.Anything, ParamValidator.IssueStatus.AsOptional());
			if (ret != null) return ret;

			return Run(() =>
			           	{
			           		string status = OptionalParameterOrDefault(args, 1, ParamValidator.IssueStatus, "open");
			           		var sb = new StringBuilder();
			           		
							int max = 0;
			           		foreach (RemoteIssue issue in _jira.SearchIssues(args[0]).Where(candidate => status == "all" || candidate.status == IssueStatus.Parse(status).Id))
			           		{
			           			string issueMessage = IssueToResultString(issue);
			           			max = Math.Max(max, issueMessage.Length);

			           			sb.AppendFormat("{0}{1}", issueMessage, Environment.NewLine);
			           		}

							sb.Insert(0, String.Format("Ticket     Status      Created             Sumary{0}{1}{0}", Environment.NewLine, new String('-', max)));
			           		return sb.ToString();
			           	});
		}
	}
}