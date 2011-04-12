/**
 * Copyright (c) 2011 Adriano Carlos Verona
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
using System.Linq;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Support;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	internal partial class LabelCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestAdd()
		{
			AssertUpdateLabels("BTS-001", "+42", "foo,42");
		}

		[Test]
		public void TestRemove()
		{
			AssertUpdateLabels("BTS-001", "-foo", "");
		}
		
		[Test]
		public void TestRemoveNonExistingLabel()
		{
			AssertUpdateLabels("BTS-002", "-foo", "bar");
		}

		[Test]
		public void TestAddRemoveMixed()
		{
			AssertUpdateLabels("BTS-001", "+l1 -bar +l2", "foo,l1,l2");
			AssertUpdateLabels("BTS-002", "+l1 -foo +l2", "bar,l1,l2");
			AssertUpdateLabels("BTS-003", "+l1 -foo -bar -foobar", "l1");
		}
		
		[Test]
		public void TestListSingleIssue()
		{
			AssertIssueLabels("BTS-001", "foo");
			AssertIssueLabels("BTS-002", "foo,bar");
		}

		[Test]
		public void TestListMultipleIssues()
		{
			AssertIssueLabels("BTS-001,BTS-002", "foo/foo,bar");
		}
		
		[Test]
		public void TestAddMultipleIssues()
		{
			AssertUpdateLabels("BTS-001, BTS-002", "+42", "foo,42/foo,bar,42");
		}

		[Test]
		public void TestIssueKeyArePipedUponUpdates()
		{
			AssertUpdatePipe("BTS-001", "+rtf", "foo,rtf", "BTS-001");
			AssertUpdatePipe("BTS-001, BTS-002", "+rtf", "foo,rtf/foo,bar,rtf", "BTS-001, BTS-002");
		}

		[Test]
		public void TestLabelsArePipedUponQueries()
		{
			AssertQueryPipe("BTS-001", "foo", "foo");
		}	
		
		[Test]
		public void TestNoPipedLabelDuplicationHappensUponQueriesWithMultipleIssues()
		{
			AssertQueryPipe("BTS-001, BTS-002", "foo/foo,bar", "foo, bar");
			AssertQueryPipe("BTS-001, BTS-004", "foo/foo,bar,foobar,baz", "foo, bar, foobar, baz");
		}

		[Test]
		public void TestLabelsAreNotDupplicatedUponInsertion()
		{
			AssertUpdateLabels("BTS-001", "+foo", "foo");
			AssertUpdateLabels("BTS-001", "+FOO", "foo");
			AssertUpdateLabels("BTS-001", "+FOO +fabio +FABIO +fAbIo", "foo, fabio");
		}

		[Test]
		public void TestRemoveNonExistingLabelDoesNoHarm()
		{
			AssertUpdateLabels("BTS-001", "-XXX", "foo");
			AssertUpdateLabels("BTS-001", "-fabio", "foo");
		}
	}
}
