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

namespace Binboo.Tests
{
	[TestFixture]
	public class FileIssueTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSuccessful()
		{
			AssertFileIssue("BTST", "File issue command test.", "Su", "task", 1);
		}

		[Test]
		public void TestDefaultDescription()
		{
			AssertFileIssue("BTST", "doesn't matter", Arguments.Missing(string.Empty), "improvement", 1);
		}

		[Test]
		public void TestJustRequiredArguments()
		{
			AssertFileIssue("BTST", "required argument 1", Arguments.Missing(string.Empty), Arguments.Missing("bug"), Arguments.Missing(-1));
		}

		[Test]
		[ExpectedException(typeof(NullReferenceException))]
		public void TestInvalidType()
		{
			ExecuteFileIssueCommand("BTST", "doesn't matter", "doesn't matter", "INVALID_TYPE", 1);
		}

		private void AssertFileIssue(Argument<string> project, Argument<string> summary, Argument<string> description, Argument<string> type, Argument<int> order)
		{
			string result = ExecuteFileIssueCommand(project, summary, description, type, order);

			string expectedResult = String.Format(@"Jira tiket created successfuly (http://tracker.db4o.com/browse/{0}-001).

{0}-001   Open        31/12/1600 21:00:00 {1}", project.Value, summary.Value);

			Assert.AreEqual(expectedResult, result);

		}

		private static string ExecuteFileIssueCommand(Argument<string> project, Argument<string> summary, Argument<string> description, Argument<string> type, Argument<int> order)
		{
			var proxyMock = new Mock<IJiraProxy>();
			proxyMock.Setup(p => p.FileIssue(string.Empty, project.Value, summary.Value, description.Value, type.Value, order.Value)).Returns(new RemoteIssue { key = project.Value + "-001", status = "1", created = DateTime.FromFileTime(42), summary = summary.Value });

			FileIssueCommand fileIssueCommand = new FileIssueCommand(proxyMock.Object, "Test");

			var contextMock = new Mock<IContext>();
			contextMock.Setup(ctx => ctx.Arguments).Returns(String.Format("{0} \"{1}\" {2} {3} type={4}", project.Value, summary.Value, QuotedStringOrEmpty(description), OrderOrEmpty(order), type.Value));
			contextMock.Setup(ctx => ctx.UserName).Returns("unit.test.user");

			string result = fileIssueCommand.Process(contextMock.Object);
			proxyMock.VerifyAll();
			return result;
		}

		private static string OrderOrEmpty(Argument<int> order)
		{
			return order.IsPresent ? order.Value.ToString() : string.Empty;
		}

		private static string QuotedStringOrEmpty(Argument<string> str)
		{
			return str.IsPresent ? "\"" + str.Value + "\"" : string.Empty;
		}
	}
}
