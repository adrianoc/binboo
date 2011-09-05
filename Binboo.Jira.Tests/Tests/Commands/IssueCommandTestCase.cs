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
using Binboo.Core;
using Binboo.Jira.Commands;
using Binboo.Jira.Integration;
using Moq;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	[TestFixture]
	public class IssueCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestRequiredArguments()
		{
			var issue = new RemoteIssue {key = "BTST-123", status="1", created = new DateTime(2009, 01, 05), summary = "summary"};
			using (var issueCommandMock = NewCommand<IssueCommand, RemoteIssue>(proxy => proxy.GetIssue("BTST-123"), issue))
			{
				Mock<IContext> contextMock = ContextMockFor("issue-user", issue.key);

				var result = issueCommandMock.Process(contextMock.Object);
				Assert.AreEqual(ExpectedResultFor(issue), result.HumanReadable);
				Assert.AreEqual(issue.key, result.PipeValue);
			}
		}

		[Test]
		public void TestComments()
		{
			var issue = new RemoteIssue { key = "BTST-123", status = "2", created = new DateTime(2009, 01, 05), summary = "summary" };
			const string comments = "some comments...";
			var commandMock = NewCommand<IssueCommand>(
												mock => mock.Setup(proxy => proxy.GetIssue("BTST-123")).Returns(issue),
												mock => mock.Setup(proxy => proxy.GetComments("BTST-123")).Returns(comments));

			Mock<IContext> contextMock = ContextMockFor("issue-user", issue.key, "comments");

			var result = commandMock.Process(contextMock.Object);

			Assert.AreEqual(
				ExpectedResultFor(issue, comments), 
				result.HumanReadable);
			
			Assert.AreEqual(
				issue.key, 
				result.PipeValue);

			commandMock.Verify();
		}

		[Test]
		public void TestNonExistingIssue()
		{
			RemoteIssue issue = new RemoteIssue { key = "BTST-123", status = "2", created = new DateTime(2009, 01, 05), summary = "summary" };
			using (var commandMock = NewCommand<IssueCommand>(mock => mock.Setup(proxy => proxy.GetIssue("BTST-123")).Throws(new JiraProxyException("Failed to get issue: " + issue.key, new Exception("")))))
			{
				Mock<IContext> contextMock = ContextMockFor("issue-user", issue.key);

				var result = commandMock.Process(contextMock.Object);

				Assert.AreEqual("Failed to get issue: " + issue.key + "\r\n\r\n", result.HumanReadable);
				Assert.AreEqual(issue.key, result.PipeValue);
			}
		}

		private static string ExpectedResultFor(RemoteIssue issue)
		{
			return ExpectedResultFor(issue, string.Empty);
		}

		private static string ExpectedResultFor(RemoteIssue issue, string comments)
		{
			if (!string.IsNullOrEmpty(comments))
			{
				comments = "\r\n" + comments;
			}
			
			return string.Format("{0}{1}\r\nhttp://sei.la.com/browse/{2}\r\n", issue.Format(), comments, issue.key);
		}
	}
}
