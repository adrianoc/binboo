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

		public override string Process(Context context)
		{
			string[] args = context.Arguments;

			string result = CheckParameters(args, ParamValidator.IssueStatus.AsOptional());
			return result ?? CalculateIDs(GetStatusOrDefault(args));
		}

		private static string GetStatusOrDefault(string[] args)
		{
			return args.Length == 1 ? args[0] : "open";
		}

		private string CalculateIDs(string status)
		{
			return Run(
				() =>
					{
						IEnumerable<RemoteIssue> results = CurrentIterationIssuesForStatus(status);

						var allDevIssues=  from devName in GetDevs(results)
							               from RemoteIssue issue in results
										   group issue by devName
											   into issuesByDev
											   select new
											   {
												   Name = issuesByDev.Key,
												   Issues = from issueTemp in results
															where
															   issueTemp.assignee == issuesByDev.Key ||
															   DevIsAPeer(issueTemp, issuesByDev.Key)
															select new
															{
																Item = issueTemp,
																IsPeer = DevIsAPeer(issueTemp, issuesByDev.Key)
															}
											   };

						var sb = new StringBuilder();
						foreach (var devIssues in allDevIssues)
						{
							sb.AppendLine(devIssues.Name);
							float idsForDev = 0.0F;
							foreach (var issue in devIssues.Issues)
							{
								float estimate = GetEstimatedIds(issue.Item);
								idsForDev += estimate;
								sb.AppendFormat("{0,-14}{1,3} {2,4}{3}", issue.Item.key, issue.IsPeer ? "" : "(*)", estimate,
								                Environment.NewLine);
							}
							sb.AppendFormat("{1}{0,22}{1}", idsForDev, Environment.NewLine);
						}

						sb.AppendFormat("{1}Total: {0}", LoadFor(results), Environment.NewLine);

						return sb.ToString();
					});
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
			int index = findIndex(issue.customFieldValues, fieldId);
			return index == -1 ? EmptyRemoteCustomFieldValue : issue.customFieldValues[index];
		}

		private static RemoteCustomFieldValue NewEmptyRemoteCustomFieldValue()
		{
			var rcfv = new RemoteCustomFieldValue();
			rcfv.values = new string[0];

			return rcfv;
		}

		private static int findIndex(RemoteCustomFieldValue[] customFields, string tbf)
		{
			return Array.FindIndex(customFields, candidate => candidate.customfieldId == tbf);
		}

		private static readonly RemoteCustomFieldValue EmptyRemoteCustomFieldValue = NewEmptyRemoteCustomFieldValue();
	}
}