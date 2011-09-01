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
using System.IO;
using Binboo.Core.Configuration;
using Binboo.Jira.Integration.JiraHttp;
using Moq;
using NUnit.Framework;
using TCL.Net;
using TCL.Extensions;

namespace Binboo.Jira.Tests.Tests.Integration.JiraHttp
{
	[TestFixture]
	public class JiraHttpProxyTestCase
	{
		private const string LoginPage = "login.aspx";
		private const string LinkPage = "link.aspx";
		private const string InvalidLoginPage = "loginform";
		private const string UserName = "john";
		private const string Password = "doe";

		[Test]
		public void TestSuccessfulLogin()
		{
			var httpClient = new Mock<IHttpClient>();

			IJiraHttpProxy jira = SetupMockForLogin(httpClient, LoginPage, UserName, Password);

			jira.Login(UserName, Password);
			Assert.IsTrue(jira.IsLoggedIn);

			httpClient.VerifyAll();
		}

		[Test]
		public void TestFailedLogin()
		{
			var httpClient = new Mock<IHttpClient>();
			const string userName = "foo";
			const string password = "bar";

			var jiraProxy = SetupMockForLogin(httpClient, LoginPage, userName, password, InvalidLoginPage);

			jiraProxy.Login(userName, password);
			Assert.IsFalse(jiraProxy.IsLoggedIn);

			httpClient.VerifyAll();
		}

		[Test]
		public void TestLink()
		{
			const string issueKey = "TIL-001";
			const string linkDescription = "Tested With";
			const int issueId = 42;

			var loginHttpClient = new Mock<IHttpClient>();
			var linkHttpClient = new Mock<IHttpClient>();
			var clientFactory = new Mock<IHttpClientFactory>();
			
			clientFactory.Setup(factory => factory.Connect(LoginPage)).Returns(loginHttpClient.Object);
			
			var config = new Mock<IHttpInterfaceConfiguration>();
			config.Setup(cfg => cfg.LoginUrl).Returns(LoginPage);
			config.Setup(cfg => cfg.LinkUrl).Returns(LinkPage);

			HttpVariables loginVariables = new HttpVariables();
			loginHttpClient.Setup(client => client.Variables).Returns(loginVariables);
			loginHttpClient.Setup(client => client.Post()).Callback(delegate
			{
				Assert.AreEqual("john", loginVariables["os_username"]);
				Assert.AreEqual("doe", loginVariables["os_password"]);
			});
			
			loginHttpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream());
			var jiraProxy = new JiraHttpProxy(clientFactory.Object, config.Object);
			jiraProxy.Login(UserName, Password);

			clientFactory.Setup(factory => factory.Connect(LinkPage)).Returns(linkHttpClient.Object);
			HttpVariables linkVariables = new HttpVariables();
			linkHttpClient.Setup(client => client.Variables).Returns(linkVariables);
			linkHttpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream());

			linkHttpClient.Setup(client => client.Post()).Callback(delegate
			{
				Assert.AreEqual(linkDescription, linkVariables["linkDesc"]);
				Assert.AreEqual(issueKey, linkVariables["linkKey"]);
				Assert.AreEqual("", linkVariables["comment"]);
				Assert.AreEqual("", linkVariables["commentLevel"]);
				Assert.AreEqual(issueId.ToString(), linkVariables["id"]);
				Assert.AreEqual("", linkVariables[""]);
				Assert.AreEqual("Link", linkVariables["Link"]);
			});

			jiraProxy.CreateLink(issueId, linkDescription, issueKey, true);

			loginHttpClient.VerifyAll();
			linkHttpClient.VerifyAll();
		}

		private static IJiraHttpProxy SetupMockForLogin(Mock<IHttpClient> httpClient, string loginPage, string userName, string password)
		{
			return SetupMockForLogin(httpClient, loginPage, userName, password, string.Empty);
		}

		private static IJiraHttpProxy SetupMockForLogin(Mock<IHttpClient> httpClient, string loginPage, string userName, string password, string returnContent)
		{
			var clientFactory = new Mock<IHttpClientFactory>();
			clientFactory.Setup(factory => factory.Connect(loginPage)).Returns(httpClient.Object);
			clientFactory.Setup(factory => factory.Connect(It.Is<string>(page => page != loginPage))).Throws(new Exception("Unexpected Connect() call."));

			//httpClient.Setup(client => client.Post("os_username=" + userName, "os_password=" + password));
			httpClient.Setup(client => client.Variables).Returns(new HttpVariables());
			httpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream(returnContent.ToBytes()));

			if (returnContent.Length == 0)
			{
				//httpClient.Setup(client => client.Cookies).Returns(new List<IHttpCookie>());
			}

			var config = new Mock<IHttpInterfaceConfiguration>();
			config.Setup(cfg => cfg.LoginUrl).Returns(loginPage);

			return new JiraHttpProxy(clientFactory.Object, config.Object);
		}
	}
}
