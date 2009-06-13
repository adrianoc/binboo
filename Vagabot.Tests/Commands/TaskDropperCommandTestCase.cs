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
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Commands
{
	[TestFixture]
	public class TaskDropperCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestJustRequiredArguments()
		{
			var issue = "TDC-001";
			using (var commandMock = NewCommand<TaskDropperCommand>(
						mock => mock.Setup(
							proxy => proxy.UpdateIssue(
											issue, 
                                            It.IsAny<string>(),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0]))))))
			{
				var contextMock = ContextMockFor(issue);
				Assert.AreEqual(string.Format("Issue {0} dropped.\r\n", issue), commandMock.Process(contextMock.Object));
			}
		}

		public void TestIssueNotFound()
		{
			var issue = "TDC-002";
			using (var commandMock = NewCommand<TaskDropperCommand>(
						mock => mock.Setup(
							proxy => proxy.UpdateIssue(
											issue,
											It.IsAny<string>(),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])),
											It.Is<IssueField>(p => string.IsNullOrEmpty(p.Values[0])))).Throws(new Exception())))
			{
				var contextMock = ContextMockFor(issue);
				Assert.AreEqual(string.Format("Issue {0} dropped.\r\n", issue), commandMock.Process(contextMock.Object));
			}
		}

	}
}
