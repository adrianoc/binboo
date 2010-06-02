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
		private const string TargetUrl = "";
		private const string UserName = "john";
		private const string Password = "doe";

		[Test]
		public void TestSuccessfulLogin()
		{
			Mock<IHttpClient> httpClient = new Mock<IHttpClient>();
			httpClient.Setup(client => client.Post("os_username=" + UserName, "os_password=" + Password));
			httpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream());
			httpClient.Setup(client => client.Cookies).Returns(new List<IHttpCookie>());

			IJiraHttpProxy jira = new JiraHttpProxy(TargetUrl, httpClient.Object);

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

			IJiraHttpProxy jira = new JiraHttpProxy(TargetUrl, httpClient.Object);

			jira.Login("foo", "bar");
			Assert.IsFalse(jira.IsLoggedIn);

			httpClient.VerifyAll();
		}

		[Test]
		public void TestLink()
		{
			const string issueKey = "TIL-001";
			const string linkName = "Tested With";
			const int issueId = 42;
			
			Mock<IHttpClient> httpClient = new Mock<IHttpClient>();
			httpClient.Setup(client => client.ResponseStream).Returns(new MemoryStream());
			httpClient.Setup(client => client.Post(
												"linkDesc=" + linkName.Replace(' ', '+'),
												"linkKey=" + issueKey,
												"comment=",
												"commentLevel=",
												"id=" + issueId,
												"Link=Link"));

			IJiraHttpProxy jira = new JiraHttpProxy(TargetUrl, httpClient.Object);
			jira.Login("foo", "bar");

			jira.LinkIssues(issueId, linkName, issueKey);
			httpClient.VerifyAll();
		}
	}
}
