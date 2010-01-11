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
using Binboo.Core;
using NUnit.Framework;

namespace Binboo.Tests.Core
{
	[TestFixture]
	public class ConfigServicesTestCase
	{
		[Test]
		public void TestIMUserMapping()
		{
			Assert.AreEqual("Susan Murphy", ConfigServices.IMUserToIssueTrackerUser("susan"));
		}
		
		[Test]
		public void TestMyselfIsMappedBasedOnSkypeUserName()
		{
			Assert.AreEqual("BOB", ConfigServices.ResolveUser("myself", "bob"));
		}

		[Test]
		public void TestPairingUsers()
		{
			CollectionAssert.AreEquivalent(new[] { "Susan Murphy", "BOB", "Frank Abagnale Jr.", "carl@hanratty.com" }, ConfigServices.PairingUsers);
		}

		[Test]
		public void TestAlias()
		{
			Assert.AreEqual("The Lost Link", ConfigServices.ResolveUser("the.lost.link", "doesn't matter"));
		}

		[Test]
		public void TestAliasIsCaseInsensitive()
		{
			Assert.AreEqual("The Lost Link", ConfigServices.ResolveUser("The.Lost.link", "doesn't matter"));
		}
		
		[Test]
		public void TestJiraNameIsReturnedIfNoAliasMatches()
		{
			Assert.AreEqual("carl@hanratty.com", ConfigServices.ResolveUser("carl@hanratty.com", "doesn't matter"));
		}
	}
}
