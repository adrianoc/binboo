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

using Binboo.Tests.Core.Commands;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	[TestFixture]
	public partial class SetOrderCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSimple()
		{
			AssertSetOrder("ITBS-001", 2);
		}

		[Test]
		public void TestBulkSetIteration()
		{
			AssertSetOrder("ITBS-001, ITBS-002, ITBS-003", 2);
		}

		[Test]
		public void TestTooManyArguments()
		{
			AssertInvalidArguments(
				"SetOrder: Unmached arguments: \r\n\r\nITBS-005 4 ** NOT EXPECTED **\r\n          ^^^^^^^^^^^^^^^^^^^",
				"ITBS-005", "4", "** NOT EXPECTED **");
		}

		[Test]
		public void TestMissingOrder()
		{
			AssertInvalidArguments(
				"SetOrder: Invalid arguments count. Expected at least 2 and no more than 2, got 1\r\n\r\n0: ITBS-005\r\n\r\n\r\nHelp",
				"ITBS-005");
		}
		
		[Test]
		public void TestInvalidOrder()
		{
			AssertInvalidArguments(
				"SetOrder: Invalid arguments count. Expected at least 2 and no more than 2, got 1\r\n\r\n0: ITBS-005\r\n\r\n\r\nHelp",
				"ITBS-005", "fortytwo");
		}
		
		[Test]
		public void TestInvalidIssueInBulkOperation()
		{
			AssertInvalidArguments(
				"SetOrder: Invalid arguments count. Expected at least 2 and no more than 2, got 1\r\n\r\n0: ITBS-005\r\n\r\n\r\nHelp",
				"ITBS-005, WHAT?", "42");
		}

		[Test]
		public void TestInvalidIssue()
		{
			AssertInvalidArguments(
				"SetOrder: Invalid arguments count. Expected at least 2 and no more than 2, got 0\r\n\r\n\r\n\r\nHelp",
				"WHAT?", "42");
		}
	}
}
