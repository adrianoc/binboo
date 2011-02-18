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
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{

	[TestFixture]
	public partial class ResolveCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSimplestPossible()
		{
			AssertResolve("TBC-001", IssueResolution.WontFix);
		}

		[Test]
		public void TestFullCommandLine()
		{
			AssertResolve("TBC-002", IssueResolution.Fixed, "\"Some comment with quotes\"");
		}

		[Test]
		public void TestIssueResolutionInComment()
		{
			AssertResolve("TBC-002", IssueResolution.Fixed, "\"This comment contains the fixed resolution.\"");
		}
		
		[Test]
		[Ignore("not implemented")]
		public void TestBulkResolve()
		{
		}

		[Test]
		public void TestIssueStatuses()
		{
			foreach (var name in IssueResolution.FriendlyNames())
			{
				AssertResolve("TBC-004", IssueResolution.Parse(name));
			}
		}

		[Test]
		public void TestInvalidIssueResolution()
		{
			using (var commandMock = NewCommand<ResolveIssueCommand>())
			{
				var contextMock = ContextMockFor(UserName, "TBC-009", "non-existing-resolution");
				contextMock.Setup(ctx => ctx.UserName).Returns("unit.test.user");

				StringAssert.Contains("Resolve: Argument index 1 (vale = 'non-existing-resolution') is invalid", commandMock.Process(contextMock.Object));
			}
		}

		[Test]
		public void TestNonExistingIssue()
		{
			using (var commandMock = NewCommand<ResolveIssueCommand>(mock => mock.Setup(proxy => proxy.ResolveIssue("TBC-010", It.IsAny<String>(), It.IsAny<IssueResolution>(), It.IsAny<IEnumerable<string>>())).Throws(new JiraProxyException("Failed to resolve issue: TBC-010", new Exception()))))
			{
				var contextMock = ContextMockFor(UserName, "TBC-010", "fixed");
				contextMock.Setup(ctx => ctx.UserName).Returns("unit.test.user");

				Assert.AreEqual("Failed to resolve issue: TBC-010\r\nException of type 'System.Exception' was thrown.", commandMock.Process(contextMock.Object));
			}
		}

		[Test]
		public void TestSingleFixedVersion()
		{
			AssertResolve("TBC-002", IssueResolution.Fixed, "\"version 1.1\"", "versions = 1.1");
		}
		
		[Test]
		public void TestMultipleFixedVersion()
		{
			AssertResolve("TBC-002", IssueResolution.Fixed, "", "versions =1.1, 1.2,1.3");
		}

		[Test]
		public void TestVersionsWithMoreThan2Components()
		{
			AssertResolve("TBC-002", IssueResolution.Fixed, "", "versions = 1.1.0");
		}
		
		private const string UserName = "resolving-user";
	}
}
