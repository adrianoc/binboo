/**
 * Copyright (c) 2010 Adriano Carlos Verona
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
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	public class EstimateCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSingleIssue()
		{
			string issue = "TST-01";
			const string estimation = "42";
			
			using (var command = NewCommand<EstimateCommand>(
								estimateMock => estimateMock.Setup(jira => jira.UpdateIssue(issue, string.Empty, It.Is<IssueField[]>(fields => fields.Length == 1 && fields[0].Values[0] == estimation)))
							))
			{
				var context = ContextMockFor("estimate-testecase", issue, estimation);

				var result = command.Process(context.Object);
				Assert.AreEqual(string.Format("[{0}] Estimation set to {1}\r\n", issue, estimation), result);
			}
		}
	}
}
