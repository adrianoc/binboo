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
using Binboo.Core.Commands;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	public partial class IssueAssignCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestJustRequiredArguments()
		{
			AssertIssueAssignment("PRJ-123", "binboo_test_user", string.Empty, string.Empty);
		}

		[Test]
		public void TestBulkAssignment()
		{
			AssertIssueAssignment("PRJ-123, PRJ-124, PRJ-125", "binboo_test_user", string.Empty, "42");
		}
		
		[Test]
		public void TestBulkAssignmentWithInvalidIssue()
		{
			AssertIssueAssignment("PRJ-123, " + NonExistingIssue + ", PRJ-125", "binboo_test_user", string.Empty, "42");
		}

		[Test]
		public void TestFullArgumentLine()
		{
			AssertIssueAssignment("FAL-123", "binboo_fal", "fal_peer", "12");
		}

		[Test]
		public void TestIterationIsSetAfterFirstAssignment()
		{
			AssertIssueAssignment("PRJ-123", "binboo_test_user", string.Empty, "42");
			AssertIssueAssignment("PRJ-123", "binboo_test_user", p => p.Values[0] == "42");
		}

		[Test]
		public void TestAssigneePeerAreCached()
		{
			AssertIssueAssignment("user-1", "PRJ-123", "woody", "rex", "42");
			AssertIssueAssignment("user-2", "PRJ-171", "andy", "sid", "42");
			AssertIssueAssignment("user-3", "STOP-174", "boo", "mikewazowski", "42");

			AssertCachedAssigneeAndPeer("user-1", "PRJ-202", "woody", "rex");
			AssertCachedAssigneeAndPeer("user-2", "PRJ-303", "andy", "sid");
			AssertCachedAssigneeAndPeer("user-3", "PRJ-174", "boo", "mikewazowski");
			AssertCachedAssigneeAndPeer("user-2", "PRJ-303", "andy", "sid");
			AssertCachedAssigneeAndPeer("user-1", "PRJ-202", "woody", "rex");
		}

		[Test]
		public void TestNoAssigneePeerCached()
		{
			Reset<IssueAssignCommand>();
			AssertNoAssigneeAndPeerCached("user-1", "PRJ-202");
			AssertNoAssigneeAndPeerCached("user-1", "PRJ-202, PRJ-303");
		}

		private const string NonExistingIssue = "NON-666";
	}
}
