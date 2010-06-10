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
using System.Linq;
using System.Text;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Configuration;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class IssueAssignCommand : JiraCommandBase
	{
		public IssueAssignCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override void Initialize()
		{
			if (Storage.Contains(AssigneesKey))
			{
				_userAssigneesMap = FromStorage(AssigneesKey, new Dictionary<string, Assignees>());
			}

			if (Storage.Contains(IterationKey))
			{
				_lastUsedIteration = FromStorage(IterationKey, NoIteration);
			}
		}

		private T FromStorage<T>(string key, T defaultValue)
		{
			return Storage.Contains(key)
			       	? (T) Storage[key]
			       	: defaultValue;
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
			                                                     peer => ParamValidator.Peer,
			                                                     iteration => ParamValidator.Iteration.AsOptional());

			Assignees assignees = ResolveAssignees(context, arguments["toUser"], arguments["peer"]);
			if (assignees == null) return string.Format("Failed to assign issue {0}; assignee not informed and no previous assignment found for user '{1}'", CommaSeparated(arguments["issueId"].Values), context.UserName);

			var sb = new StringBuilder();
			foreach (var ticket in arguments["issueId"].Values)
			{
				sb.AppendLine(AssignIssue(ticket, assignees.Assignee, assignees.Peer, IterationFrom(arguments["iteration"])));
			}

			StoreAssignees();

			return sb.Remove(sb.Length - 2, 2).ToString();
		}

		private void StoreAssignees()
		{
			Storage[AssigneesKey] = _userAssigneesMap;
			Storage[IterationKey] = _lastUsedIteration;
		}

		private static string CommaSeparated(IEnumerable<string> list)
		{
			return list.Aggregate("", (acc, current) => acc + ((acc.Length > 0) ? ", " : "") + current);
		}

		private Assignees ResolveAssignees(IContext context, Argument assignee, Argument peer)
		{
			if (assignee.IsPresent)
			{
				var resolvedAssignee = ConfigServices.ResolveUser(assignee.Value, context.UserName);
				var peerField = IssueField.CustomField(CustomFieldId.Peers) <= Peer(context, peer);

				return AddToAssigneesMap(context.UserName, resolvedAssignee, peerField);
			}
			
			return LookupAssignees(context.UserName);
		}

		private Assignees LookupAssignees(string userName)
		{
			Assignees assignees;
			_userAssigneesMap.TryGetValue(userName, out assignees);

			return assignees;
		}

		private Assignees AddToAssigneesMap(string userName, string assignee, IssueField peer)
		{
			Assignees assignees = new Assignees {Assignee = assignee, Peer = peer};
			_userAssigneesMap[userName] = assignees;

			return assignees;
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
				_lastUsedIteration = Int32.Parse(iteration.Value);
			}

			return _lastUsedIteration == NoIteration ? null : IssueField.CustomField(CustomFieldId.Iteration) <= _lastUsedIteration.ToString();
		}

		private static string Peer(IContext context, Argument peer)
		{
			return peer.IsPresent ? ConfigServices.ResolveUser(peer, context.UserName) : NoOne ;
		}
		
		private class Assignees
		{
			public string Assignee;
			public IssueField Peer;
		}
		
		private const string NoOne = null;
		private const int NoIteration = -1;

		private int _lastUsedIteration = NoIteration;
		private const string AssigneesKey = "Assignees";
		private const string IterationKey = "Iteration";

		private IDictionary<string, Assignees> _userAssigneesMap = new Dictionary<string, Assignees>();
	}
}
