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
using System.Text;
using Binboo.Core.Commands.Arguments;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class SetIterationCommand : JiraCommandBase
	{
		public SetIterationCommand(IJiraProxy jira, string help) : base(jira, help)
		{
		}

		public override string Id
		{
			get { return "SetIteration"; }
		}

		protected override string ProcessCommand(IContext context)
		{
			var arguments = CollectAndValidateArguments(context.Arguments, issueId => ParamValidator.MultipleIssueId, iteration => ParamValidator.Iteration);
			var sb = new StringBuilder();
			
			foreach (var issue in arguments["issueId"].Values)
			{
				string currentIssue = issue;
				sb.AppendLine(Run(
									() => _jira.UpdateIssue(currentIssue, String.Empty, NewIteration(arguments["iteration"])),
									string.Format("Iteration set for issue '{0}'.", currentIssue)
								));
			}

			return sb.ToString();
		}

		private static IssueField NewIteration(Argument iteration)
		{
			return IssueField.CustomField(CustomFieldId.Iteration) <= iteration.Value;
		}
	}
}
