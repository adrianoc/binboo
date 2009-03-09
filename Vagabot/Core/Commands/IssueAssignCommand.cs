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
using System.Text.RegularExpressions;
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
		 * Assign <ticket #> <main developer> [<peer>] [<iteration>]
		 */
		public override string Process(Context context)
		{
			ParamValidator[] validators = {
			                              	ParamValidator.TicketNumber,
			                              	ParamValidator.UserName,
			                              	ParamValidator.Peer.AsOptional(),
			                              	ParamValidator.Iteration.AsOptional(),
			                              };

			string result  = CheckParameters(context.Arguments, validators);

			if (result != null) return result;

			string assignee = ConfigServices.ResolveUser(context.Arguments[1], context);
			return Run(() => _jira.AssignIssue(
										context.Arguments[0],
										IssueField.Assignee <= assignee,
										IssueField.CustomField(CustomFieldId.Peers) <= Peer(context.Arguments, NonOptionalCount(validators), context),
										IterationFrom(context.Arguments, NonOptionalCount(validators))),
										
							String.Format("Successfuly assigned issue {0} to {1}", context.Arguments[0], assignee));
		}

		private IssueField IterationFrom(IEnumerable<string> args, int startIndex)
		{
			Match match = OptionalParameterOrNull(args, startIndex, ParamValidator.Iteration);
			if (match != null)
			{
				_lastestIterationUsed = Int32.Parse(match.Value);
			}

			return _lastestIterationUsed == -1 ? null : IssueField.CustomField(CustomFieldId.Iteration) <= _lastestIterationUsed.ToString();
		}

		private static string Peer(IEnumerable<String> args, int startIndex, Context context)
		{
			Match match = OptionalParameterOrNull(args, startIndex, ParamValidator.Peer);
			return match == null ? NoOne : ConfigServices.ResolveUser(match.Value, context);
		}
	}
}
