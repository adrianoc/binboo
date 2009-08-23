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
using System.Linq;
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Commands
{
	[TestFixture]
	public class SearchCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSearchParameters()
		{
			AssertParameters("what-we-are-looking-for");
			AssertParameters("what-we-are-looking-for", "all");
			AssertParameters("what-we-are-looking-for", "open");
			AssertParameters("what-we-are-looking-for", "closed");
		}

		private void AssertParameters(params string[] args)
		{
			var commandMock = NewCommand<SearchCommand>(mock => mock.Setup(proxy => proxy.SearchIssues(args[0])).Returns(_issues));

			Mock<IContext> contextMock = ContextMockFor("some-user", args);
			
			string status = (args.Length == 2) ? args[1] : "open";

			int max = 0;
			string expected = _issues.Where(FilterByStatus(status)).Aggregate("", (buffer, issue) => 
																					{
																						string formatedIssue = issue.Format();
																						max = Math.Max(max, formatedIssue.Length);
																						return buffer + "\r\n" + formatedIssue; 
																					});

			expected = string.Format("Issue      Status      Created             Sumary{0}{1}{2}{0}", Environment.NewLine, new String('-', max), expected);
			Assert.AreEqual(expected, commandMock.Process(contextMock.Object));

			commandMock.Verify();
		}

		private Func<RemoteIssue, bool> FilterByStatus(string status)
		{
			return candidate => status == "all" || candidate.status == IssueStatus.Parse(status);
		}
	}
}
