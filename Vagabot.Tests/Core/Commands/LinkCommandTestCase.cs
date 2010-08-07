﻿/**
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
using Binboo.Core.Commands;
using Binboo.Core.Configuration;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	public class LinkCommandTestCase : JiraCommandTestCaseBase
	{
		private const string ExpectedLinkDescription = "links to";

		[Test]
		public void TestSuccessfulLink()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			mockedJiraSoapProxy.Setup(jiraProxy => jiraProxy.CreateLink("TIL-001", "Tested With", "TIL-002", false));

			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			Mock<IContext> mockedContext = ContextMockFor("foo", "TIL-001", "\"Tested With\"", "TIL-002");
			string actualResult = linkCommand.Process(mockedContext.Object);

			Assert.AreEqual("[Link] Link created successfully: TIL-001 Tested With TIL-002", actualResult);

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
			string actualResult = linkCommand.Process(mockedContext.Object);

			Assert.IsTrue(actualResult.Contains("Exception Thrown"));

			mockedJiraSoapProxy.VerifyAll();			
		}

		[Test]
		public void TestLinkAliasesConfig()
		{
			LinkConfiguration config = LinkConfiguration.From(ConfigServices.CommandConfigurationFor("Link"));

			Assert.AreEqual("add=>create new", (string) config.AliasFor("add"));
			Assert.AreEqual("remove=>delete", (string) config.AliasFor("remove"));
		}

		[Test]
		public void TestNonExistingLinkAliasConfig()
		{
			LinkConfiguration config = LinkConfiguration.From(ConfigServices.CommandConfigurationFor("Link"));
			var actual = config.AliasFor("non-existing");
			
			Assert.AreSame(LinkAlias.NullAlias, actual);
			Assert.IsTrue(actual.IsNull);
		}

		[Test]
		public void TestLinkAliases()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			mockedJiraSoapProxy.Setup(jiraProxy => jiraProxy.CreateLink("TIL-001", ExpectedLinkDescription, "TIL-002", false));

			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			Mock<IContext> mockedContext = ContextMockFor("foo", "TIL-001", "associate", "TIL-002");
			string actualResult = linkCommand.Process(mockedContext.Object);

			Assert.AreEqual(string.Format("[Link] Link created successfully: TIL-001 {0} TIL-002", ExpectedLinkDescription), actualResult);

			mockedJiraSoapProxy.VerifyAll();
		}

		[Test]
		public void TestHelpMessageContainsAliases()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			var linkCommand = new LinkIssueCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			var linkConfiguration = LinkConfiguration.From(ConfigServices.CommandConfigurationFor("link"));

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