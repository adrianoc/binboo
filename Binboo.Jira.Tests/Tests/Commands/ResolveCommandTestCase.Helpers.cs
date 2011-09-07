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
using System.Linq;
using System.Text.RegularExpressions;
using Binboo.Jira.Commands;
using Binboo.Jira.Integration;
using Binboo.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	public partial class ResolveCommandTestCase
	{
		private void AssertResolve(string ticket, IssueResolution resolution)
		{
			AssertResolve(ticket, resolution, String.Empty);
		}

		private void AssertResolve(string ticket, IssueResolution resolution, string comment)
		{
			AssertResolve(ticket, resolution, comment, string.Empty);
		}

		private void AssertResolve(string ticket, IssueResolution resolution, string comment, string fixedInVersions)
		{
			string noQuotesComment = StripQuotes(comment);
			using (var commandMock = NewCommand<ResolveIssueCommand>(ExpectedMethodCalls(ticket, noQuotesComment, resolution, fixedInVersions)))
			{
				var contextMock = ContextMockFor("resolving-user", String.Format("{0} \"{1}\"{2} {3}", ticket, resolution.Description.ToLower(), fixedInVersions, String.IsNullOrEmpty(comment) ? "" : (" " + comment)));
				contextMock.Setup(ctx => ctx.User.Name).Returns("unit.test.user");

				var expectedOutput = string.Format("Issue {0} ('{1}') resolved as '{2}'.", ticket, IssueTestService.Issue[ticket].summary, resolution.Description);

				var result = commandMock.Process(contextMock.Object);

				Assert.AreEqual(expectedOutput, result.HumanReadable);
				Assert.AreEqual(ticket, result.PipeValue);
			}
		}

		private static Action<Mock<IJiraProxy>> ExpectedMethodCalls(string ticket, string noQuotesComment, IssueResolution resolution, string fixedInVersions)
		{
			return proxyMock => proxyMock.Setup(
									p => p.ResolveIssue(
												ticket, 
												It.Is<String>(remmark => remmark == noQuotesComment), 
												resolution,
												It.Is<IEnumerable<string>>(
												    versions => string.IsNullOrEmpty(fixedInVersions)
												                    ? true
																	: versions.Any(version => IsValidVersion(fixedInVersions, version)))
										)
								).Returns(IssueTestService.Issue[ticket]);
		}

		private static bool IsValidVersion(string fixedInVersions, string version)
		{
			var versionPattern = @"\b(?<!\.)" + version.Replace(".", @"\.") + @"(?!\.)";
			return Regex.IsMatch(fixedInVersions, versionPattern) && !version.StartsWith(".");
		}

		private static string StripQuotes(string comment)
		{
			return comment.Replace("\"", "");
		}
	}
}
