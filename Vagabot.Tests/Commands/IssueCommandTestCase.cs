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
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Commands
{
	[TestFixture]
	public class IssueCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestRequiredArguments()
		{
			RemoteIssue issue = new RemoteIssue {key = "BTST-123", status="1", created = new DateTime(2009, 01, 05), summary = "summary"};
			var issueCommandMock = NewCommand<IssueCommand, RemoteIssue>(proxy => proxy.GetIssue("BTST-123"), issue);

			Mock<IContext> contextMock = ContextMockFor(issue.key);

			string result = issueCommandMock.Process(contextMock.Object);
			
			Assert.AreEqual(ExpectedResultFor(issue), result);

			issueCommandMock.Verify();
		}

		[Test]
		public void TestComments()
		{
			RemoteIssue issue = new RemoteIssue { key = "BTST-123", status = "2", created = new DateTime(2009, 01, 05), summary = "summary" };
			const string comments = "some comments...";
			var commandMock = NewCommand<IssueCommand>(
												mock => mock.Setup(proxy => proxy.GetIssue("BTST-123")).Returns(issue),
												mock => mock.Setup(proxy => proxy.GetComments("BTST-123")).Returns(comments));

			Mock<IContext> contextMock = ContextMockFor(issue.key, "comments");

			Assert.AreEqual(
				ExpectedResultFor(issue, comments), 
				commandMock.Process(contextMock.Object));

			contextMock.VerifyAll();
			commandMock.Verify();
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
			
			return string.Format("{0}{1}\r\nhttp://tracker.db4o.com/browse/{2}", issue.Format(), comments, issue.key);
		}
	}
}
