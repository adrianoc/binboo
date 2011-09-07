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
using Binboo.Jira.Commands;
using Binboo.Tests.Core.Commands;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	[TestFixture]
	public partial class FileIssueTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSpacesInbetweenTypeLiteralAndValueDontCrash()
		{
			AssertSpacesInTypeSpecification("type= task");
			AssertSpacesInTypeSpecification("type = task");
			AssertSpacesInTypeSpecification("type  =task");
		}

		[Test]
		public void TestSingleWordInDescription()
		{
			AssertFileIssue("BTST", "File issue command test.", "single-word", "task", 1);
		}

		[Test]
		public void TestSuccessful()
		{
			AssertFileIssue("BTST", "File issue command test.", "simple issue creation.", "task", 1);
		}

		[Test]
		public void TestDefaultDescription()
		{
			AssertFileIssue("BTST", "doesn't matter", ArgumentHelper.Missing(string.Empty), "improvement", 1);
		}

		[Test]
		public void TestQuotedDescription()
		{
			AssertFileIssue("BTST", "doesn't matter", "This is a 'quoted' description", "improvement", 1);
		}

		[Test]
		public void TestJustRequiredArguments()
		{
			AssertFileIssue("BTST", "required argument 1", ArgumentHelper.Missing(string.Empty), ArgumentHelper.Missing("bug"), ArgumentHelper.Missing(FileIssueCommand.DefaultIssueOrder));
		}

		[Ignore("Behavior when type is invalid is to use Bug as default. we should throw an exception.")]
		[ExpectedException(typeof(NullReferenceException))]
		public void TestInvalidType()
		{
			ExecuteFileIssueCommand("BTST", "doesn't matter", "This is a description", "INVALID_TYPE", 1);
		}
	}
}
