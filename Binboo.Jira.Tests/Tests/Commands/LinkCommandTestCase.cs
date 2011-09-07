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
using System;
using Binboo.Core.Commands.Support;
using Binboo.Jira.Commands;
using Binboo.Core.Configuration;
using Binboo.Jira.Integration;
using Moq;
using NUnit.Framework;
using TCL.Net.Extensions;

namespace Binboo.Jira.Tests.Tests.Commands
{
	[TestFixture]
	public class LinkCommandTestCase : JiraCommandTestCaseBase
	{
		private const string SourceIssue = "TIL-001";
		private const string TargetIssue = "TIL-002";
		private const string ExpectedLinkDescription = "links to";

		[Test]
		public void TestSuccessfulLink()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			mockedJiraSoapProxy.Setup(jiraProxy => jiraProxy.CreateLink(SourceIssue, "Tested With", TargetIssue, false));

			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			Mock<IContext> mockedContext = ContextMockFor("foo", SourceIssue, "\"Tested With\"", TargetIssue);
			var result = linkCommand.Process(mockedContext.Object);

			Assert.AreEqual("[Link] Link created successfully: TIL-001 Tested With TIL-002", result.HumanReadable);
			Assert.AreEqual(SourceIssue + ", " + TargetIssue, result.PipeValue);

			mockedJiraSoapProxy.VerifyAll();
		}

		[Test]
		public void TestSourceIssueDoNotExists()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			const string NonExistingIssueKey = "TIL-001";
			mockedJiraSoapProxy.Setup(jiraProxy => jiraProxy.CreateLink(NonExistingIssueKey, "test-exception", "TIL-002", false)).Throws(new JiraProxyException("Exception Thrown", new Exception()));

			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			Mock<IContext> mockedContext = ContextMockFor("foo", NonExistingIssueKey, "test-exception", "TIL-002");
			var actualResult = linkCommand.Process(mockedContext.Object);

			Assert.IsTrue(actualResult.HumanReadable.Contains("Exception Thrown"));

			mockedJiraSoapProxy.VerifyAll();			
		}

		[Test]
		public void TestLinkAliasesConfig()
		{
			var config = ConfigServices.CommandConfigurationFor("link").Deserialize<LinkConfiguration>();

			Assert.AreEqual("add=>create new", (string) config.AliasFor("add"));
			Assert.AreEqual("remove=>delete", (string) config.AliasFor("remove"));
		}

		[Test]
		public void TestNonExistingLinkAliasConfig()
		{
			var config = ConfigServices.CommandConfigurationFor("link").Deserialize<LinkConfiguration>();
			var actual = config.AliasFor("non-existing");
			
			Assert.AreSame(LinkAlias.NullAlias, actual);
			Assert.IsTrue(actual.IsNull);
		}

		[Test]
		public void TestLinkAliases()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			mockedJiraSoapProxy.Setup(jiraProxy => jiraProxy.CreateLink(SourceIssue, ExpectedLinkDescription, TargetIssue, false));

			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			Mock<IContext> mockedContext = ContextMockFor("foo", SourceIssue, "associate", TargetIssue);
			var result = linkCommand.Process(mockedContext.Object);

			Assert.AreEqual(string.Format("[Link] Link created successfully: {0} {1} {2}", SourceIssue, "associate", TargetIssue), result.HumanReadable);
			Assert.AreEqual(SourceIssue + ", " + TargetIssue, result.PipeValue);

			mockedJiraSoapProxy.VerifyAll();
		}

		[Test]
		public void TestHelpMessageContainsAliases()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			var linkConfiguration = ConfigServices.CommandConfigurationFor("link").Deserialize<LinkConfiguration>();

			foreach (var alias in linkConfiguration.Aliases)
			{
				StringAssert.Contains(alias.Original, linkCommand.Help);
			}
		}

		private static Mock<IJiraProxy> MockedJiraSoapProxy()
		{
			return new Mock<IJiraProxy>();
		}
	}
}
