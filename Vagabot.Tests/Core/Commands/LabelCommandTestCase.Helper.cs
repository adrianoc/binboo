/**
 * Copyright (c) 2011 Adriano Carlos Verona
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
using System.Text;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Support;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	partial class LabelCommandTestCase
	{
		private void AssertIssueLabels(string issueKeys, string expectedLabels)
		{
			AssertIssueLabels(issueKeys, expectedLabels, pipe => { });
		}

		private void AssertQueryPipe(string issueKeys, string expectedLabels, string expectedPipedValue)
		{
			AssertIssueLabels(issueKeys, expectedLabels, pipedValue => Assert.AreEqual(expectedPipedValue, pipedValue));
		}

		private void AssertIssueLabels(string issueKeys, string expectedLabels, Action<string> pipeAsserter)
		{
			using (var mockedCommand = NewCommand<LabelCommand>(RetrieveIssueProxyCalls(issueKeys).ToArray()))
			{
				var context = ContextMockFor("label-user", issueKeys);
				var result = mockedCommand.Process(context.Object);

				Assert.AreEqual(CommandStatus.Success, result.Status);
				StringAssert.StartsWith(ExpectedMessageForQueries(issueKeys, expectedLabels), result.HumanReadable);

				pipeAsserter(result.PipeValue);
			}
		}

		private void AssertUpdateLabels(string issueKeys, string labelCmdLine, string allIssuesExpectedLabels)
		{
			AssertUpdateLabels(issueKeys, labelCmdLine, allIssuesExpectedLabels, delegate { });
		}

		private void AssertUpdatePipe(string issueKeys, string labelCmdLine, string allIssuesExpectedLabels, string expectedPipeValue)
		{
			AssertUpdateLabels(issueKeys, labelCmdLine, allIssuesExpectedLabels, actualPipe => Assert.AreEqual(expectedPipeValue, actualPipe));
		}

		private void AssertUpdateLabels(string issueKeys, string labelCmdLine, string allIssuesExpectedLabels, Action<string> pipeAsserter)
		{
			using (var mockedCommand = NewCommand<LabelCommand>(UpdateLabelsForMultipleIssuesProxyCalls(issueKeys, allIssuesExpectedLabels)))
			{
				var labels = LabelFormatConveter.FromUI(labelCmdLine).To(la => la);
				var fullCmdLine = new List<string>(labels);
				fullCmdLine.Insert(0, issueKeys);
				var context = ContextMockFor("label-user", fullCmdLine.ToArray());
				var result = mockedCommand.Process(context.Object);

				Assert.AreEqual(CommandStatus.Success, result.Status);
				StringAssert.StartsWith(ExpectedMessageForUpdates(issueKeys, allIssuesExpectedLabels), result.HumanReadable);

				pipeAsserter(result.PipeValue);
			}
		}

		private static Action<Mock<IJiraProxy>>[] UpdateLabelsForMultipleIssuesProxyCalls(string issueKeys, string allIssuesExpectedLabels)
		{
			IEnumerator<string> expectedLabels = EnumerateExpectedLabels(allIssuesExpectedLabels).GetEnumerator();
			var proxyCalls = new List<Action<Mock<IJiraProxy>>>();
			foreach (var issueKey in EnumerateIssueKeys(issueKeys))
			{
				Assert.IsTrue(expectedLabels.MoveNext());

				var setupProxyCalls = UpdateLabelsProxyCalls(issueKey, ExpectedLabelsToJiraLabelFormat(expectedLabels.Current));
				proxyCalls.AddRange(setupProxyCalls);
			}
			return proxyCalls.ToArray();
		}

		private static string ExpectedLabelsToJiraLabelFormat(string expectedLabels)
		{
			return LabelFormatConveter.FromUI(expectedLabels).ToJira();
		}

		private static IEnumerable<string> EnumerateIssueKeys(string issueKeys)
		{
			return issueKeys.Split(',').Select(issue => issue.Trim());
		}

		private static IEnumerable<string> EnumerateExpectedLabels(string allIssuesExpectedLabels)
		{
			return allIssuesExpectedLabels.Split('/');
		}

		private static string ExpectedMessageForQueries(string issueKeys, string expectedLabels)
		{
			return ExpectedMessageFor(expectedLabels, issueKeys, LabelCommand.FormatOutputMessage);
		}

		private static string ExpectedMessageForUpdates(string issueKeys, string expectedLabels)
		{
			return ExpectedMessageFor(expectedLabels, issueKeys, LabelCommand.FormatOutputMessage);
		}

		private static string ExpectedMessageFor(string expectedLabels, string issueKeys, Func<string, string, string> formater)
		{
			var labelsEnumerator = EnumerateExpectedLabels(expectedLabels).GetEnumerator();
			var sb = new StringBuilder();
			foreach (var issueKey in EnumerateIssueKeys(issueKeys))
			{
				Assert.IsTrue(labelsEnumerator.MoveNext());
				sb.AppendFormat("{0}\r\n", formater(issueKey, ExpectedLabelsToJiraLabelFormat(labelsEnumerator.Current)));
			}
			
			return sb.Remove(sb.Length-2, 2).ToString();
		}

		private static IEnumerable<Action<Mock<IJiraProxy>>> UpdateLabelsProxyCalls(string key, string expectedLabels)
		{
			Expression<Action<IJiraProxy>> updateIssueCall = jira => jira.UpdateIssue(
																		key,
																		It.IsAny<string>(),
																		It.Is<IssueField[]>(customFields =>
																					customFields.Any(cf => cf.Id == CustomFieldId.Labels.Id && cf.Values.Length == 1 && cf.Values[0] == expectedLabels)));
			var proxyCalls = RetrieveIssueProxyCalls(key);
			proxyCalls.Add(ms2 => ms2.Setup(updateIssueCall));
			return proxyCalls.ToArray();
		}

		private static IList<Action<Mock<IJiraProxy>>> RetrieveIssueProxyCalls(string issueKeys)
		{
			IList<Action<Mock<IJiraProxy>>> calls = new List<Action<Mock<IJiraProxy>>>();
			foreach (var issueKey in issueKeys.Split(',').Select(key => key.Trim()))
			{
				var currentKey = issueKey;
				calls.Add(mock => mock.Setup(jira => jira.GetIssue(currentKey)).Returns(FindIssue(currentKey)));	
			}
			return calls;
		}

		private static RemoteIssue FindIssue(string key)
		{
			return Issues.Where(ri => ri.key == key).Single();
		}
	}
}
