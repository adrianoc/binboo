﻿/**
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
using System.ServiceModel;
using System.Text;
using Binboo.Core;

namespace Binboo.JiraIntegration
{
	class JiraProxy : IJiraProxy
	{
		public JiraProxy(string endPoint, JiraUser user)
		{
			_client = new ExpirationStrategy<JiraSoapServiceClient>(
								new JiraSoapServiceClient("jirasoapservice-v2", new EndpointAddress(endPoint)),
								client => _loginToken = client.login(user.Name, user.Password));

			Login();
		}

		private void Login()
		{
			_client.Refresh();
			InitializeConstants();
		}

		private void InitializeConstants()
		{
			CustomFieldId.Initialize(_client.Item.getCustomFields(_loginToken));
			IssueResolution.Initialize(_client.Item.getResolutions(_loginToken));
			IssueStatus.Initialize(_client.Item.getStatuses(_loginToken));
			IssueType.Initialize(_client.Item.getIssueTypes(_loginToken));
			IssuePriority.Initialize(_client.Item.getPriorities(_loginToken));
		}

		public void LogOut()
		{
			ValidateConnection();
			_client.Item.logout(_loginToken);
		}

		public RemoteIssue[] SearchIssues(string content)
		{
			ValidateConnection();
			return Run( () => _client.Item.getIssuesFromTextSearch(_loginToken, content), "Failed searching for content: " + content);
		}

		public RemoteProject[] GetProjectList()
		{
			ValidateConnection();
			return Run(() => _client.Item.getProjects(_loginToken), "Failed to get project list.");
		}

		public RemoteIssue[] IssuesForFilter(string filterId)
		{
			ValidateConnection();
			return Run( () => _client.Item.getIssuesFromFilter(_loginToken, filterId), string.Format("Failed to get issues for field '{0}'", filterId));
		}

		public RemoteIssue GetIssue(string ticket)
		{
			ValidateConnection();
			ticket = Normalize(ticket);

			return Run(() => _client.Item.getIssue(_loginToken, ticket), "Failed to get issue: " + ticket);
		}

		public RemoteIssue FileIssue(string reporter, string project, string summary, string description, string type, int order)
		{
			ValidateConnection();
			RemoteIssue issue = CreateIssue(reporter, project.ToUpper(), summary, description, type, order, IssuePriority.Major);

			return Run( () => _client.Item.createIssue(_loginToken, issue),
						string.Format("Failed to file issue '{0}' for project {1}.", summary, issue.project));
		}

		public void AssignIssue(string ticket, IssueField assignee, IssueField peer, IssueField iteration)
		{
			ValidateConnection();
			ticket = Normalize(ticket);

			Run(delegate
			{
				RemoteIssue issue = GetIssue(ticket);
				if (issue.status == IssueStatus.Closed)
				{
					_client.Item.progressWorkflowAction(
												_loginToken,
												ticket,
												ActionIdFor(ticket, "Reopen"),
												CollectChangedFields(IssueField.Status <= IssueStatus.ReOpened, assignee, iteration ?? IssueField.CustomFieldFromIssue(issue, CustomFieldId.Iteration)));
				}
				else
				{
					UpdateIssue(ticket, String.Empty, assignee, peer, iteration);
				}
			},

			"Failed to asssign issue " + ticket);			
		}

		public void CloseIssue(string ticket, string comment, IssueResolution resolution)
		{
			ValidateConnection();
			ticket = Normalize(ticket);

			Run(delegate
			    {
			    	RemoteIssue issue = GetIssue(ticket);
					string actionId = ActionIdFor(ticket, "Close");

					RemoteFieldValue[] fields = CollectChangedFields(
													IssueField.Status <= IssueStatus.Closed,
													IssueField.Assignee <= issue.assignee,
													IssueField.CustomField(CustomFieldId.Iteration) <= issue.CustomFieldValue(CustomFieldId.Iteration),
													IssueField.CustomField(CustomFieldId.Peers) <= issue.CustomFieldValue(CustomFieldId.Peers),
													IssueField.CustomField(CustomFieldId.OriginalIDsEstimate) <= issue.CustomFieldValue(CustomFieldId.OriginalIDsEstimate),
													IssueField.Resolution <= resolution);

			    	return _client.Item.progressWorkflowAction(_loginToken, ticket, actionId, fields);
				},
				
				"Failed to close issue " + ticket);

			AddComment(
				ticket, 
				comment,
				string.Format("Issue {0} was closed but an error prevented the comment to be appended.", ticket));
		}

		public void UpdateIssue(string ticketNumber, params IssueField[] fields)
		{
			UpdateIssue(ticketNumber, string.Empty, fields);
		}

		public void UpdateIssue(string ticketNumber, string comment, params IssueField[] fields)
		{
			ValidateConnection();
			ticketNumber = Normalize(ticketNumber);

			Run( () => _client.Item.updateIssue(
								_loginToken, 
								ticketNumber, 
								CollectChangedFields(fields)),
				"Failed to update issue: " + ticketNumber);

			AddComment(
					ticketNumber, 
					comment,
					string.Format("Issue {0} was updated but an error prevented the comment to be appended.", ticketNumber));
		}

		public void AddComment(string ticket, string comment)
		{
			ValidateConnection();

			AddComment(
				ticket,
				comment,
				string.Format("Failed to comment issue {0} : '{1}'", ticket, comment));
		}

		private void AddComment(string ticket, string comment, string errorMessage)
		{
			if (!string.IsNullOrEmpty(comment))
			{
				Run(
					() => _client.Item.addComment(_loginToken, ticket, new RemoteComment {body = comment}),
					errorMessage);
			}
		}

		public string GetComments(string ticket)
		{
			ticket = Normalize(ticket);
			RemoteComment[] comments = Run(() => _client.Item.getComments(_loginToken, ticket), "Unable to retrieve comments.");

			var sb = new StringBuilder("Comments\r\n\r\n");
			foreach (RemoteComment comment in comments)
			{
				sb.AppendFormat("[{0} on {1}]{3}{2}{3}{3}", comment.author, comment.created, comment.body, Environment.NewLine);
			}

			return sb.ToString();
		}

		private static string Normalize(string ticketNumber)
		{
			return ticketNumber.ToUpper();
		}

		private string ActionIdFor(string ticket, string actionName)
		{
			actionName = actionName.ToLower();

			RemoteNamedObject[] actions = _client.Item.getAvailableActions(_loginToken, ticket);
			RemoteNamedObject found = Array.Find(actions, action => action.name.ToLower().Contains(actionName));
			if (found == null)
			{
				throw new ArgumentException(string.Format("Action '{0}' not found", actionName), actionName);
			}
			return found.id;
		}

		private static RemoteFieldValue[] CollectChangedFields(params IssueField[] fields)
		{
			IList<RemoteFieldValue> changedFields = new List<RemoteFieldValue>();
			foreach(IssueField field in fields)
			{
				if (null != field)
				{
					changedFields.Add(RemoteFieldFor(field));
				}
			}

			return changedFields.ToArray();
		}

		private static RemoteFieldValue RemoteFieldFor(IssueField field)
		{
			return new RemoteFieldValue {id = field.Id, values = field.Values};
		}

		private static RemoteIssue CreateIssue(string reporter, string project, string summary, string desc, string type, int order, string priority)
		{
			var issue = new RemoteIssue {reporter=reporter, summary = summary, project = project, type = type, description = desc, priority = priority};
			if (order != -1)
			{
				issue.customFieldValues = new[]
				                          	{
				                          		RemoteCustomFieldValueFor(IssueField.CustomField(CustomFieldId.Order) <=
				                          		                          new[] {order.ToString()}),
				                          	};
			}
			return issue;
		}

		private static RemoteCustomFieldValue RemoteCustomFieldValueFor(IssueField field)
		{
			return new RemoteCustomFieldValue {customfieldId = field.Id, values = field.Values};
		}

		private void ValidateConnection()
		{
			if (_loginToken == null || _client == null)
			{
				throw new InvalidOperationException("Client needs to be connected and loged-in.");
			}
		}

		private static void Run(Action action, string message)
		{
			try
			{
				action();
			}
			catch (Exception fe)
			{
				throw new JiraProxyException(message, fe);
			}
		}

		private static T Run<T>(Func<T> func, string message)
		{
			try
			{
				return func();
			}
			catch (Exception fe)
			{
				throw new JiraProxyException(message, fe);
			}
		}

		private readonly ExpirationStrategy<JiraSoapServiceClient> _client;
		private string _loginToken;
	}

	internal class JiraProxyException : Exception
	{
		public JiraProxyException(string message, Exception inner) : base(message, inner)
		{
		}

		public JiraProxyException(string message) : base(message)
		{
		}
	}
}
