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
using System;
using System.Linq;
using Binboo.Jira.Commands;
using Binboo.Jira.Integration;
using Moq;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	[TestFixture]
	public class CountIdsTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void Test()
		{
			using (var cmd = NewCommand<CountIDSCommand>(ExpectedCalls()))
			{
				var contextMock = ContextMockFor("user");
				var result = cmd.Process(contextMock.Object);

				var expectedOutput =
					@"tetyana
BTS-001      Open            1

                             1
shrek
BTS-002      Open            2

                             2
rodrigo
BTS-003      Closed          3

                             3
adriano
BTS-004      Closed          4
BTS-008      In Progress     8

                            12

Total: 18
	Open    : 11
	Closed  : 7
";
				Assert.AreEqual(expectedOutput, result.HumanReadable);
			}
		}

		private Action<Mock<IJiraProxy>>[] ExpectedCalls()
		{
			return new Action<Mock<IJiraProxy>>[]
		            {
		                mock => mock.Setup(proxy => proxy.IssuesForFilter(It.IsAny<string>())).Returns(Issues.Where(issue => issue.CustomFieldValue(CustomFieldId.Iteration)[0] == CurrentIteration.ToString()).ToArray())
		            };
		}
	}
}
