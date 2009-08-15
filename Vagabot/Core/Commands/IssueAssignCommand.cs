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
using Binboo.Core.Commands.Arguments;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class IssueAssignCommand : JiraCommandBase
	{
		private const string NoOne = null;
		private const int NoIteration = -1;

		private int _lastestIterationUsed = NoIteration;

		public IssueAssignCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "Assign"; }
		}

		/*
		 * Assign <ticket #(s)> <main developer> [<peer>] [<iteration>]
		 */
		protected override string ProcessCommand(IContext context)
		{
			IDictionary<string, Argument> arguments = CollectAndValidateArguments(context.Arguments,
			                                                     issueId => ParamValidator.MultipleIssueId,
			                                                     toUser => ParamValidator.UserName,
			                                                     peer => ParamValidator.Peer.AsOptional(),
			                                                     iteration => ParamValidator.Iteration.AsOptional());
			
			var assignee = ConfigServices.ResolveUser(arguments["toUser"].Value, context);
			var peerField = IssueField.CustomField(CustomFieldId.Peers) <= Peer(context, arguments["peer"]);

			StringBuilder sb = new StringBuilder();
			foreach (var ticket in arguments["issueId"].Values)
			{
				sb.AppendLine(AssignIssue(ticket, assignee, peerField, IterationFrom(arguments["iteration"])));
			}

			return sb.Remove(sb.Length - 2, 2).ToString();
		}

		private string AssignIssue(string ticket, string assignee, IssueField peer, IssueField iteration)
		{
			return Run(() => _jira.AssignIssue(
			                 	ticket,
			                 	IssueField.Assignee <= assignee,
			                 	peer,
			                 	iteration),

			           String.Format("Successfuly assigned issue {0} to {1}", ticket, assignee));
		}

		private IssueField IterationFrom(Argument iteration)
		{
			if (iteration.IsPresent)
			{
				_lastestIterationUsed = Int32.Parse(iteration.Value);
			}

			return _lastestIterationUsed == NoIteration ? null : IssueField.CustomField(CustomFieldId.Iteration) <= _lastestIterationUsed.ToString();
		}

		private static string Peer(IContext context, Argument peer)
		{
			return peer.IsPresent ? ConfigServices.ResolveUser(peer, context) : NoOne ;
		}
	}
}
