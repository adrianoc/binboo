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
using System.Collections.Generic;
using Binboo.Core.Configuration;
using TCL.Net;
using TCL.Net.Extensions;

namespace Binboo.JiraIntegration.JiraHttp
{
	class JiraHttpProxy : IJiraHttpProxy
	{
		public JiraHttpProxy(IHttpClientFactory clientFactory, IHttpInterfaceConfiguration config)
		{
			_config = config;
			_clientFactory = clientFactory;
		}

		public void Login(string userName, string password)
		{
			//TODO: IDisposable ?
			var client = _clientFactory.Connect(_config.LoginUrl);
			client.Post(string.Format("os_username={0}", userName), string.Format("os_password={0}", password));

			UpdateLoginStatus(client);
			RememberCookies(client);
		}

		private void RememberCookies(IHttpClient client)
		{
			_cookies = client.Cookies;
		}

		private void UpdateLoginStatus(IHttpClient client)
		{
			_isLoggedIn = !client.ResponseStream.Contains("loginform");
		}

		public bool IsLoggedIn
		{
			get { return _isLoggedIn; }
		}

		public void CreateLink(int issueId, string linkDesc, string issueKey)
		{
			var client = _clientFactory.Connect(_config.LinkUrl);
			client.Cookies = _cookies;
			client.Post(
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
			return _clientFactory.ToString();
		}

		private readonly IHttpClientFactory _clientFactory;
		private bool _isLoggedIn;
		private IList<IHttpCookie> _cookies;
		private readonly IHttpInterfaceConfiguration _config;
	}
}
