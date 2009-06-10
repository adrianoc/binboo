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
using System.Linq.Expressions;
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;

namespace Binboo.Tests.Commands
{
	public class JiraCommandTestCaseBase
	{
		private static readonly RemoteStatus[] _issueStatus = 
												{
													new RemoteStatus {id="1", name = "open"}, 
		                                            new RemoteStatus {id="2", name = "closed"}, 
													new RemoteStatus {id="3", name = "in progress"}, 
													new RemoteStatus {id="4", name = "resolved"}, 
													new RemoteStatus {id="5", name = "reopened"}, 
												};

		private static readonly RemoteIssueType[] _issueTypes = 
													{
														new RemoteIssueType {id = "1", name="bug"}, 
														new RemoteIssueType {id = "2", name="task"}, 
														new RemoteIssueType {id = "3", name="improvement"}, 
														new RemoteIssueType {id = "4", name="new feature"}, 
													};

		internal Mock<IJiraProxy> _jiraProxyMock;

		static JiraCommandTestCaseBase()
		{
			IssueType.Initialize(_issueTypes);
			IssueStatus.Initialize(_issueStatus);
			//CustomFieldId.Initialize(new RemoteField[] { });
			//IssueResolution.Initialize(new RemoteResolution[] {});
			//IssuePriority.Initialize(new RemotePriority[] {});
		}

		protected Mock<IContext> ContextMockFor(string arguments)
		{
			Mock<IContext> contextMock = new Mock<IContext>();
			contextMock.Setup(context => context.Arguments).Returns(arguments);
			return contextMock;
		}

		internal T NewCommand<T, R>(Expression<Func<IJiraProxy, R>> expectedMethodCall, R valueToReturn)
		{
			_jiraProxyMock = new Mock<IJiraProxy>();
			_jiraProxyMock.Setup(expectedMethodCall).Returns(valueToReturn);

			return (T) Activator.CreateInstance(typeof(T), new object[] {_jiraProxyMock.Object, typeof(T).Name});
		}

		internal T NewCommand<T>(params Action<Mock<IJiraProxy>>[] mockSetups)
		{
			_jiraProxyMock = new Mock<IJiraProxy>();
			foreach (var setup in mockSetups)
			{
				setup(_jiraProxyMock);
			}

			return (T)Activator.CreateInstance(typeof(T), new object[] { _jiraProxyMock.Object, typeof(T).Name });
		}
	}
}