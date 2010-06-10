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
using System.Text;
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	public class TaskDropperCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestJustRequiredArguments()
		{
			AssertDrop("TDC-001");
		}

		[Test]
		public void TestBulkDrop()
		{
			AssertDrop("TBD-001, TBD-002");
		}

		private void AssertDrop(string issues)
		{
			using (var commandMock = NewCommand<TaskDropperCommand>(MocksFor(issues)))
			{
				var contextMock = ContextMockFor("drop-user", issues);
				Assert.AreEqual(ExpectedResponse(issues), commandMock.Process(contextMock.Object));
			}
		}

		private static string ExpectedResponse(string issues)
		{
			var sb = new StringBuilder();
			foreach (var issue in issues.Split(','))
			{
				sb.Append(string.Format("Issue {0} dropped.\r\n", issue.Trim()));
			}
			
			return sb.ToString();
		}

		private static Action<Mock<IJiraProxy>>[] MocksFor(string issues)
		{
			var issuesArray = issues.Split(',');
			var setups = new Action<Mock<IJiraProxy>>[issuesArray.Length];

			for(int i = 0; i < issuesArray.Length; i++)
			{
				string issue = issuesArray[i].Trim();
				setups[i] = mock => mock.Setup(
										proxy => proxy.UpdateIssue(
														issue,
														It.IsAny<string>(),
														It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
														It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
														It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0]))));
			}

			return setups;
		}

		[Test]
		public void TestIssueNotFound()
		{
			const string issue = "TDC-002";
			using (var commandMock = NewCommand<TaskDropperCommand>(
						mock => mock.Setup(
							proxy => proxy.UpdateIssue(
											issue,
											It.IsAny<string>(),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])))).Throws(new JiraProxyException(issue, new Exception("Not found")))))
			{
				var contextMock = ContextMockFor("drop-user", issue);
				Assert.AreEqual(string.Format("{0}\r\nNot found\r\n", issue), commandMock.Process(contextMock.Object));
			}
		}
	}
}
