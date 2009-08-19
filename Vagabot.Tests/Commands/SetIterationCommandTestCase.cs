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

using NUnit.Framework;

namespace Binboo.Tests.Commands
{
	[TestFixture]
	public partial class SetIterationCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSimple()
		{
			AssertSetIteration("ITBS-001", 42);
		}

		[Test]
		public void TestBulkSetIteration()
		{
			AssertSetIteration("ITBS-001, ITBS-002, ITBS-003", 42);
		}

		[Test]
		public void TestTooManyArguments()
		{
			AssertInvalidArguments(
				"SetIteration: Unmached arguments: \r\n\r\nITBS-005 42 ** NOT EXPECTED **\r\n           ^^^^^^^^^^^^^^^^^^^",
				"ITBS-005", "42", "** NOT EXPECTED **");
		}
		
		[Test]
		public void TestInvalidIteration()
		{
			AssertInvalidArguments(
				"SetIteration: Unmached arguments: \r\n\r\nITBS-005 fortytwo\r\n         ^^^^^^^^",
				"ITBS-005", "fortytwo");
		}
		
		[Test]
		public void TestInvalidIssueInBulkOperation()
		{
			AssertInvalidArguments(
				@"SetIteration: Argument index 0 (vale = 'ITBS-005, ') is invalid (Validator: ((?<issues>(?:\b(?<param>[A-Za-z]{1,4}-[0-9]{1,4})\s*,?\s*)+\b),required)).",
				"ITBS-005, WHAT?", "42");
		}

		[Test]
		public void TestInvalidIssue()
		{
			AssertInvalidArguments(
				@"SetIteration: Argument index 0 (vale = '42') is invalid (Validator: ((?<issues>(?:\b(?<param>[A-Za-z]{1,4}-[0-9]{1,4})\s*,?\s*)+\b),required)).",
				"WHAT?", "42");
		}
	}
}
