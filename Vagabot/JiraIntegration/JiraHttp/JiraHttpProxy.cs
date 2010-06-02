using System;
using System.Collections.Generic;
using TCL.Net.Extensions;
using TCL.Net;

namespace Binboo.JiraIntegration.JiraHttp
{
	class JiraHttpProxy : IJiraHttpProxy
	{
		public JiraHttpProxy(string targetUrl, IHttpClient httpClient)
		{
			_httpClient = httpClient;
			_targetUrl = targetUrl;
		}

		public void Login(string userName, string password)
		{
			_httpClient.Post(string.Format("os_username={0}",userName), string.Format("os_password={0}",password));
			
			UpdateLoginStatus();
			RememberCookies();
		}

		private void RememberCookies()
		{
			_cookies = _httpClient.Cookies;
		}

		private void UpdateLoginStatus()
		{
			_isLoggedIn = !_httpClient.ResponseStream.Contains("loginform");
		}

		public bool IsLoggedIn
		{
			get { return _isLoggedIn; }
		}

		public void LinkIssues(int issueId, string linkDesc, string issueKey)
		{
			_httpClient.Cookies = _cookies;
			_httpClient.Post(
					"linkDesc=" + linkDesc.Replace(' ', '+'),
					"linkKey=" + issueKey,
					"comment=",
					"commentLevel=",
					"id=" + issueId,
					"Link=Link");

			//TODO: Add response to log        
		}

		private readonly IHttpClient _httpClient;
		private string _targetUrl;
		private bool _isLoggedIn;
		private IList<IHttpCookie> _cookies;
	}
}
