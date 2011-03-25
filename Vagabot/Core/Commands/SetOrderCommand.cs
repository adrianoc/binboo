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
using Binboo.Core.Commands.Support;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class SetOrderCommand : JiraCommandBase
	{
		public SetOrderCommand(IJiraProxy jira, string help) : base(jira, help)
		{
		}

		public override string Id
		{
			get { return "SetOrder"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			var arguments = CollectAndValidateArguments(context.Arguments, issueId => ParamValidator.MultipleIssueId, order => ParamValidator.Order);
			var sb = new StringBuilder();

			var orderField = NewOrder(arguments["order"]);
			var issues = arguments["issueId"].Values;

			foreach (var issue in issues)
			{
				string currentIssue = issue;
				sb.AppendLine(Run(
				              	() => _jira.UpdateIssue(currentIssue, String.Empty, orderField),
								string.Format("Order set to {0} for issue '{1}'.", arguments["order"].Value, currentIssue)));
			}

			return CommandResult.Success(sb.ToString(), issues);
		}

		private static IssueField NewOrder(Argument order)
		{
			return IssueField.CustomField(CustomFieldId.Order) <= order.Value;
		}
	}
}
