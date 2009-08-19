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
using System.Linq.Expressions;
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;

namespace Binboo.Tests.Commands
{
	public class JiraCommandTestCaseBase
	{
		private static readonly RemoteStatus[] IssueStatus = 
												{
													new RemoteStatus {id="1", name = "open"}, 
		                                            new RemoteStatus {id="2", name = "closed"}, 
													new RemoteStatus {id="3", name = "in progress"}, 
													new RemoteStatus {id="4", name = "resolved"}, 
													new RemoteStatus {id="5", name = "reopened"}, 
												};

		private static readonly RemoteIssueType[] IssueTypes = 
													{
														new RemoteIssueType {id = "1", name="bug"}, 
														new RemoteIssueType {id = "2", name="task"}, 
														new RemoteIssueType {id = "3", name="improvement"}, 
														new RemoteIssueType {id = "4", name="new feature"}, 
													};

		private static readonly RemoteField[] Statuses = new[]
		                                                  	{
		                                                  		new RemoteField { id = "1", name = "peers" },
		                                                  		new RemoteField { id = "2", name = "iteration" },
		                                                  		new RemoteField { id = "3", name = "original ids estimate" },
		                                                  		new RemoteField { id = "4", name = "order" },
		                                                  	};


		protected RemoteIssue[] _issues = new []
		                                  	{
		                                  		new RemoteIssue {key = "BTS-001", status = JiraIntegration.IssueStatus.Open, summary = "", assignee = "tetyana"}, 
		                                  		new RemoteIssue {key = "BTS-002", status = JiraIntegration.IssueStatus.Open, summary = "", assignee = "shrek"}, 
		                                  		new RemoteIssue {key = "BTS-003", status = JiraIntegration.IssueStatus.Closed, summary = "", assignee = "rodrigo"}, 
		                                  		new RemoteIssue {key = "BTS-004", status = JiraIntegration.IssueStatus.Closed, summary = "", assignee = "adriano"}, 
		                                  		new RemoteIssue {key = "BTS-005", status = JiraIntegration.IssueStatus.Resolved, summary = "", assignee = "carl"}, 
		                                  		new RemoteIssue {key = "BTS-006", status = JiraIntegration.IssueStatus.ReOpened, summary = "", assignee = "patrick"}, 
		                                  		new RemoteIssue {key = "BTS-007", status = JiraIntegration.IssueStatus.InProgress, summary = "", assignee = "anat"}, 
		                                  	};

		private static readonly RemoteResolution [] Resolutions = new []
		                                           	{
		                                           		new RemoteResolution {id="1", description = "fixed", name="Fixed"},
		                                           		new RemoteResolution {id="2", description = "won't fix", name="Won't Fix"},
		                                           		new RemoteResolution {id="3", description = "duplicate", name="Duplicate"},
		                                           		new RemoteResolution {id="4", description = "incomplete", name="Incomplete"},
		                                           		new RemoteResolution {id="5", description = "cannot reproduce", name="Cannot Reproduce"},
													};

		private Mock<IJiraProxy> _jiraProxyMock;
		private static readonly IDictionary<Type, JiraCommandBase> Commands = new Dictionary<Type, JiraCommandBase>();

		static JiraCommandTestCaseBase()
		{
			IssueType.Initialize(IssueTypes);
			JiraIntegration.IssueStatus.Initialize(IssueStatus);
			CustomFieldId.Initialize(Statuses);
			IssueResolution.Initialize(Resolutions);
			//IssuePriority.Initialize(new RemotePriority[] {});
		}

		protected static Mock<IContext> ContextMockFor(params string[] arguments)
		{
			var contextMock = new Mock<IContext>();
			contextMock.Setup(context => context.Arguments).Returns(ZipArguments(arguments));
			return contextMock;
		}

		private static string ZipArguments(IEnumerable<string> arguments)
		{
			return arguments.Aggregate("", (acc, current) => acc + " " + current).Substring(1);
		}

		internal CommandMock<T> NewCommand<T, R>(Expression<Func<IJiraProxy, R>> expectedMethodCall, R valueToReturn) where T : JiraCommandBase
		{
			_jiraProxyMock = new Mock<IJiraProxy>();
			_jiraProxyMock.Setup(expectedMethodCall).Returns(valueToReturn);

			return new CommandMock<T>(FromCacheOrNew<T>(), _jiraProxyMock);
		}

		internal CommandMock<T> NewCommand<T>(params Action<Mock<IJiraProxy>>[] mockSetups) where T: JiraCommandBase
		{
			_jiraProxyMock = new Mock<IJiraProxy>(MockBehavior.Strict);
			foreach (var setup in mockSetups)
			{
				setup(_jiraProxyMock);
			}

			return new CommandMock<T>(FromCacheOrNew<T>(), _jiraProxyMock);
		}

		private T FromCacheOrNew<T>() where T : JiraCommandBase
		{
			if (!Commands.ContainsKey(typeof (T)))
			{
				Commands[typeof (T)] = (T) Activator.CreateInstance(typeof (T), new object[] {_jiraProxyMock.Object, typeof (T).Name});
			}

			JiraCommandBase command = Commands[typeof (T)];
			command.Proxy = _jiraProxyMock.Object;
			return (T) command;
		}
	}
}