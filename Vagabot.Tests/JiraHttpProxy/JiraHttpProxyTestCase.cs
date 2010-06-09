using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;

using TCL.Net;
using TCL.Extensions;

using Binboo.JiraIntegration.JiraHttp;

namespace Binboo.Tests.JiraHttp
{
	[TestFixture]
	public class JiraHttpProxyTestCase
	{
		private const string InvalidLoginPage = "loginform";
		private const string UserName = "john";
		private const string Password = "doe";

		[Test]
		public void TestSuccessfulLogin()
		{
			Mock<IHttpClient> httpClient = new Mock<IHttpClient>();
			httpClient.Setup(client => client.Post("os_username=" + UserName, "os_password=" + Password));
			httpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream());
			httpClient.Setup(client => client.Cookies).Returns(new List<IHttpCookie>());

			IJiraHttpProxy jira = new JiraHttpProxy(httpClient.Object);

			jira.Login(UserName, Password);
			Assert.IsTrue(jira.IsLoggedIn);

			httpClient.VerifyAll();
		}
		
		[Test]
		public void TestFailedLogin()
		{
			Mock<IHttpClient> httpClient = new Mock<IHttpClient>();
			httpClient.Setup(client => client.Post("os_username=foo", "os_password=bar"));
			httpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream(InvalidLoginPage.ToBytes()));

			IJiraHttpProxy jira = new JiraHttpProxy(httpClient.Object);

			jira.Login("foo", "bar");
			Assert.IsFalse(jira.IsLoggedIn);

			httpClient.VerifyAll();
		}

		[Test]
		public void TestLink()
		{
			const string issueKey = "TIL-001";
			const string linkDescription = "Tested With";
			const int issueId = 42;
			
			Mock<IHttpClient> httpClient = new Mock<IHttpClient>();
			httpClient.Setup(mockHttpClient => mockHttpClient.ResponseStream).Returns(new MemoryStream());
			httpClient.Setup(mockHttpClient => mockHttpClient.Post(
														"linkDesc=" + linkDescription.Replace(' ', '+'),
														"linkKey=" + issueKey,
														"comment=",
														"commentLevel=",
														"id=" + issueId,
														"Link=Link"));

			IJiraHttpProxy jira = new JiraHttpProxy(httpClient.Object);
			jira.Login("foo", "bar");

			jira.CreateLink(issueId, linkDescription, issueKey);
			
			httpClient.VerifyAll();
		}
	}
}
