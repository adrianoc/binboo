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
using Binboo.Core.Commands.Support;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class CountIDSCommand : JiraCommandBase
	{
		public CountIDSCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "CountIDS"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			return CalculateIDs(OptionalArgumentOrDefault(CollectArguments(context), "status", "all"));
		}

		private IDictionary<string, Argument> CollectArguments(IContext context)
		{
			return CollectAndValidateArguments(context.Arguments, status => ParamValidator.IssueStatus.AsOptional());
		}

		private ICommandResult CalculateIDs(string status)
		{
			var result = Run(
				() =>
					{
						var issues = CurrentIterationIssuesForStatus(status);

						var allDevIssues=  from devName in GetDevs(issues)
							               from RemoteIssue issue in issues
											   
										   select new
										   {
											   Name = devName,
											   Issues = from issueTemp in issues
											            let devIsAPeer = DevIsAPeer(issueTemp, devName)
											            where issueTemp.assignee == devName || devIsAPeer
														select new
														{
															Item = issueTemp,
															IsPeer = devIsAPeer
														}
											   };

						var sb = new StringBuilder();
						foreach (var devIssues in allDevIssues)
						{
							sb.AppendLine(devIssues.Name);
							var idsForDev = 0.0F;
							foreach (var issue in devIssues.Issues)
							{
								var estimate = GetEstimatedIds(issue.Item);
								idsForDev += estimate;
								sb.AppendFormat("{0,-13}{1,-12} {2,4}{3}", issue.Item.key, IssueStatus.FriendlyNameFor(issue.Item.status), estimate, Environment.NewLine);
							}
							sb.AppendFormat("{1}{0,30}{1}", idsForDev, Environment.NewLine);
						}

						AppendTotals(sb, issues);

						return sb.ToString();
					});

			return CommandResult.Success(result);
		}

		private static void AppendTotals(StringBuilder buffer, IEnumerable<RemoteIssue> issues)
		{
			buffer.AppendFormat("{1}Total: {0}{1}", LoadFor(issues), Environment.NewLine);

			var totalByStatus = from issue in issues
			                    group issue by IssueStatus.IsFinished(issue.status)
			                    into issuesByStatus
			                    	select new
			                    	       	{
			                    	       		Status = issuesByStatus.Key ? "Closed" : "Open",
			                    	       		Total = issuesByStatus.Sum(item => GetEstimatedIds(item))
			                    	       	};

			foreach (var idsCountGroup in totalByStatus)
			{
				buffer.AppendFormat("\t{0,-7} : {1}{2}", idsCountGroup.Status, idsCountGroup.Total, Environment.NewLine);
			}

		}

		private static IEnumerable<string> GetDevs(IEnumerable<RemoteIssue> results)
		{
			var assignees = from issue in results
			                select issue.assignee;

			var peers = from issue in results
			            from peer in GetCustomField(issue, CustomFieldId.Peers).values
			            select peer;

			return assignees.Concat(peers).Distinct();
		}

		private static float LoadFor(IEnumerable<RemoteIssue> issues)
		{
			float totalIDS = 0.0F;
			foreach (RemoteIssue issue in issues)
			{
				totalIDS += GetEstimatedIds(issue);
			}
			return totalIDS;
		}

		private IEnumerable<RemoteIssue> CurrentIterationIssuesForStatus(string status)
		{
			RemoteIssue[] issues = _jira.IssuesForFilter("10221");
			return from RemoteIssue issue in issues
			       where IncludeIssue(status, issue)
			       select issue;
		}

		private static bool IncludeIssue(string status, RemoteIssue issue)
		{
			if (status == "all") return true;
			return status == "open" ? !IssueStatus.IsFinished(issue.status) : IssueStatus.IsFinished(issue.status);
		}

		private static bool DevIsAPeer(RemoteIssue issue, string dev)
		{
			RemoteCustomFieldValue peersField = GetCustomField(issue, CustomFieldId.Peers);
			if (peersField == null) return false;

			return Array.Exists(peersField.values, candidate => candidate == dev);
		}

		private static float GetEstimatedIds(RemoteIssue issue)
		{
			RemoteCustomFieldValue idsField = GetCustomField(issue, CustomFieldId.OriginalIDsEstimate);
			if (idsField == EmptyRemoteCustomFieldValue) return 0.0F;

			float issueIds;
			float.TryParse(idsField.values[0], out issueIds);

			return issueIds;
		}

		private static RemoteCustomFieldValue GetCustomField(RemoteIssue issue, string fieldId)
		{
			int index = FindIndex(issue.customFieldValues, fieldId);
			return index == -1 ? EmptyRemoteCustomFieldValue : issue.customFieldValues[index];
		}

		private static RemoteCustomFieldValue NewEmptyRemoteCustomFieldValue()
		{
			return new RemoteCustomFieldValue { values = new string[0] };
		}

		private static int FindIndex(RemoteCustomFieldValue[] customFields, string tbf)
		{
			return Array.FindIndex(customFields, candidate => candidate.customfieldId == tbf);
		}

		private static readonly RemoteCustomFieldValue EmptyRemoteCustomFieldValue = NewEmptyRemoteCustomFieldValue();
	}
}