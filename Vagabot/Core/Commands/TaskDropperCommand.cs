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

using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class TaskDropperCommand : JiraCommandBase
	{
		public TaskDropperCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "Drop"; }
		}

		public override string Process(Context context)
		{
			string result = CheckParameters(context.Arguments, ParamValidator.TicketNumber, ParamValidator.Anything.AsOptional());
			return result ?? DropTask(context.Arguments[0], context.Arguments.Length > 1 ? context.Arguments[1] : string.Empty);
		}

		private string DropTask(string ticket, string comment)
		{
			return Run(() => _jira.UpdateIssue(ticket.ToUpper(), comment, DropTaskFields()),
						string.Format("Issue {0} dropped.", ticket));
		}

		private static IssueField[] DropTaskFields()
		{
			return new[]
			       	{
			       		IssueField.Assignee <= string.Empty,
						IssueField.CustomField(CustomFieldId.Peers) <= string.Empty,
						IssueField.CustomField(CustomFieldId.Iteration) <= string.Empty,
			       	};
		}
	}
}
