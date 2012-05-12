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

using Binboo.Core;
using Binboo.Core.Configuration;
using Binboo.Jira.Configuration;
using Binboo.Jira.Plugin;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Plugin
{
	[TestFixture]
	public partial class JiraConfigTestCase
	{
		private IJiraConfig  _jiraConfig;

		[SetUp]
		public void SetUp()
		{
			_jiraConfig = new JiraConfig(ConfigurationFactory.Create());
		}

		[Test]
		public void TestIMUserMapping()
		{
			Assert.AreEqual("Susan Murphy", _jiraConfig.IMUserToIssueTrackerUser("susan"));
		}

		[Test]
		public void TestMyselfIsMappedBasedOnSkypeUserName()
		{
			Assert.That(_jiraConfig.ResolveUser("myself", "bob"), Is.EqualTo("BOB"));
		}
		
		[Test]
		public void TestPairingUsers()
		{
			CollectionAssert.AreEquivalent(new[] { "Susan Murphy", "BOB", "Frank Abagnale Jr.", "carl@hanratty.com" }, _jiraConfig.PairingUsers);
		}

		[Test]
		public void TestAlias()
		{
			Assert.That(_jiraConfig.ResolveUser("the.lost.link", "doesn't matter"), Is.EqualTo("The Lost Link"));
		}
		
		[Test]
		public void TestAliasIsCaseInsensitive()
		{
			Assert.That(_jiraConfig.ResolveUser("The.Lost.link", "doesn't matter"), Is.EqualTo("The Lost Link"));
		}

		[Test]
		public void TestJiraNameIsReturnedIfNoAliasMatches()
		{
			Assert.That(_jiraConfig.ResolveUser("carl@hanratty.com", "doesn't matter"), Is.EqualTo("carl@hanratty.com"));
		}

		[Test]
		public void TestUser()
		{
			Assert.That(_jiraConfig.User.Name, Is.EqualTo("john doe"));
			Assert.That(_jiraConfig.User.Password, Is.EqualTo("who"));
		}

		[Test]
		public void TestJiraHttpUrls()
		{
			AssertJiraHttpLink(cfg => cfg.LinkUrl);
			AssertJiraHttpLink(cfg => cfg.LoginUrl);
		}
	}
}
