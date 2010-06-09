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
using System.Collections.Generic;
using TCL.Net.Extensions;
using TCL.Net;

namespace Binboo.JiraIntegration.JiraHttp
{
	class JiraHttpProxy : IJiraHttpProxy
	{
		public JiraHttpProxy(IHttpClient httpClient)
		{
			_httpClient = httpClient;
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

		public void CreateLink(int issueId, string linkDesc, string issueKey)
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

		public override string ToString()
		{
			return _httpClient.ToString();
		}

		private readonly IHttpClient _httpClient;
		private bool _isLoggedIn;
		private IList<IHttpCookie> _cookies;
	}
}
