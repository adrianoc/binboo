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
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;
using TCL.Net;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	public class LinkCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestSuccessfulLink()
		{
			Mock<IJiraProxy> mockedJiraSoapProxy = MockedJiraSoapProxy();
			mockedJiraSoapProxy.Setup(jiraProxy => jiraProxy.CreateLink("TIL-001", "Tested With", "TIL-002"));

			var linkCommand = new IssueLinkCommand(
										mockedJiraSoapProxy.Object,
										"Testing issue linking.");

			Mock<IContext> mockedContext = ContextMockFor("foo", "TIL-001", "\"Tested With\"", "TIL-002");
			string actualResult = linkCommand.Process(mockedContext.Object);

			Assert.AreEqual("Link created successfully: TIL-001 Tested With TIL-002", actualResult);

			mockedJiraSoapProxy.VerifyAll();
		}

		private static Mock<IHttpClient> MockedHttpClient()
		{
			return new Mock<IHttpClient>(MockBehavior.Strict);
		}

		private static Mock<IJiraProxy> MockedJiraSoapProxy()
		{
			return new Mock<IJiraProxy>();
		}
	}
}
