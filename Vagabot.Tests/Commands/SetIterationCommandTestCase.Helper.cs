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
using System.Text;
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Commands
{
	public partial class SetIterationCommandTestCase
	{
		private void AssertInvalidArguments(string expectedMessage, params string[] arguments)
		{
			var jiraProxyMock = new Mock<IJiraProxy>();
			var commandMock = new CommandMock<SetIterationCommand>(new SetIterationCommand(jiraProxyMock.Object, "Help"), jiraProxyMock);
			var contextMock = ContextMockFor(arguments);
			Assert.AreEqual(expectedMessage, commandMock.Process(contextMock.Object));
		}

		private void AssertSetIteration(string issues, int iteration)
		{
			using (var commandMock = NewCommand<SetIterationCommand>(SetupProxyCalls(issues, iteration)))
			{
				var contextMock = ContextMockFor(issues, iteration.ToString());

				Assert.AreEqual(ExpectedMessage(issues), commandMock.Process(contextMock.Object));
			}
		}

		private static string ExpectedMessage(string commaSeparatedIssues)
		{
			var sb = new StringBuilder();
			foreach (var issue in commaSeparatedIssues.Split(','))
			{
				sb.AppendLine(string.Format("Iteration set for issue '{0}'.", issue.Trim()));
			}
			return sb.ToString();
		}

		private static Action<Mock<IJiraProxy>>[] SetupProxyCalls(string commaSeparatedIssues, int iteration)
		{
			var issues = commaSeparatedIssues.Split(',');
			var setups = new Action<Mock<IJiraProxy>>[issues.Length];

			for (int i = 0; i < issues.Length; i++)
			{
				var currentIssue = issues[i].Trim();
				setups[i] = mock => mock.Setup(jira => jira.UpdateIssue(
																It.Is<string>(ci => ci == currentIssue),
																It.IsAny<string>(),
																It.Is<IssueField[]>(fields => fields.Length == 1 && fields[0].Values[0] == iteration.ToString())));

			}

			return setups;
		}
	}
}
