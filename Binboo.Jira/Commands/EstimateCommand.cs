/**
 * Copyright (c) 2010 Adriano Carlos Verona
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
	class EstimateCommand : JiraCommandBase
	{
		public EstimateCommand(IJiraProxy jira, string help) : base(jira, help)
		{
		}

		public override string Id
		{
			get { return "Estimate"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			IDictionary<string, Argument> arguments = CollectAndValidateArguments(
															context.Arguments,
															issueId => JiraParamValidator.MultipleIssueId,
                                                            estimation => JiraParamValidator.Estimation);

			var ticket = arguments["issueId"];
			var sb = new StringBuilder();

			foreach (var issue in ticket.Values)
			{
				sb.Append(Estimate(issue, arguments["estimation"]));
			}

			return CommandResult.Success(sb.ToString(), ticket.Values);
		}

		private string Estimate(string issue, string estimation)
		{
			return Run(
				() => _jira.UpdateIssue(issue, string.Empty, IssueField.CustomField(CustomFieldId.OriginalIDsEstimate) <= estimation),
				string.Format("[{0}] Estimation set to {1}\r\n", issue, estimation));
		}
	}
}
