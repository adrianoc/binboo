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
using NUnit.Framework;

namespace Binboo.Tests.Commands
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
		public void TestQuotedDescription()
		{
			AssertFileIssue("BTST", "doesn't matter", "This is a 'quoted' description", "improvement", 1);
		}

		[Test]
		public void TestJustRequiredArguments()
		{
			AssertFileIssue("BTST", "required argument 1", Arguments.Missing(string.Empty), Arguments.Missing("bug"), Arguments.Missing(FileIssueCommand.DefaultIssueOrder));
		}

		[Ignore("Behavior when type is invalid is to use Bug as default. we should throw an exception.")]
		[ExpectedException(typeof(NullReferenceException))]
		public void TestInvalidType()
		{
			ExecuteFileIssueCommand("BTST", "doesn't matter", "This is a description", "INVALID_TYPE", 1);
		}

		private void AssertFileIssue(Argument<string> project, Argument<string> summary, Argument<string> description, Argument<string> type, Argument<int> order)
		{
			string result = ExecuteFileIssueCommand(project, summary, description, type, order);
			string expectedResult = String.Format(@"Jira tiket created successfuly (http://sei.la.com/browse/{0}-001).

{0}-001   Open        {1} {2}", project.Value, CreationDate, summary.Value);

			Assert.AreEqual(expectedResult, result);

		}

		private string ExecuteFileIssueCommand(Argument<string> project, Argument<string> summary, Argument<string> description, Argument<string> type, Argument<int> order)
		{
			IssueType issueType = IssueType.Parse(type.Value);
			using (var commandMock = NewCommand<FileIssueCommand>(proxyMock => proxyMock.Setup(p => p.FileIssue(string.Empty, project.Value, summary.Value, description.Value, issueType.Id, order.Value)).Returns(new RemoteIssue { key = project.Value + "-001", status = "1", created = CreationDate, summary = summary.Value })))
			{
				var contextMock = ContextMockFor("creator", String.Format("{0} \"{1}\" {2} {3} type={4}", project.Value, summary.Value, QuotedStringIfRequiredOrEmpty(description), OrderOrEmpty(order), type.Value));
				contextMock.Setup(ctx => ctx.UserName).Returns("unit.test.user");

				return commandMock.Process(contextMock.Object);
			}
		}

		private static string OrderOrEmpty(Argument<int> order)
		{
			return order.IsPresent ? order.Value.ToString() : string.Empty;
		}

		private static string QuotedStringIfRequiredOrEmpty(Argument<string> str)
		{
			if (!str.IsPresent) return string.Empty;
			return str.Value.IndexOf(' ') >= 0 ? "\"" + str.Value + "\"" : str.Value;
		}
		
		private readonly DateTime CreationDate = DateTime.FromFileTime(42);
	}
}
