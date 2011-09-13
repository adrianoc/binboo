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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Binboo.Jira.Configuration;
using HtmlAgilityPack;
using TCL.Net;
using TCL.Net.Extensions;
using TCL.Net.Net;

namespace Binboo.Jira.Integration.JiraHttp
{
    public class JiraHttpProxy : IJiraHttpProxy
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
			client.Variables["os_username"] = userName;
			client.Variables["os_password"] = password;
			client.Post();

			UpdateLoginStatus(client);
			RememberCookies(client);
			
			Log("[Login]", client.ResponseStream);
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

		public string CreateLink(int issueId, string linkDesc, string issueKey, bool versobe)
		{
			var client = _clientFactory.Connect(_config.LinkUrl);
			client.Cookies = _cookies;

			client.Variables["linkDesc"] = linkDesc;
			client.Variables["linkKey"] = issueKey;
			client.Variables["comment"] = "";
			client.Variables["commentLevel"] = "";
			client.Variables["id"] = issueId.ToString();
			client.Variables["Link"] = "Link";

			client.Post();

			return ResponseFor(client.ResponseStream, versobe);
		}

		private static string ResponseFor(Stream responseStream, bool versobe)
		{
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.Load(responseStream);

			HtmlNode found = htmlDocument.DocumentNode.SelectSingleNode("//select[@name='linkDesc']");
			if (found != null)
			{
				string escapedErrorMessage = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='errMsg']").InnerText.Trim();
				var errorMessage = HttpUtility.HtmlDecode(escapedErrorMessage);

				HtmlNodeCollection validLinkOptions = found.SelectNodes("option");
				if (validLinkOptions == null) return null;

				return errorMessage + (versobe 
										? "\r\nValid link descriptions: \r\n" + validLinkOptions.Aggregate("", (agg, current) => agg + "      \"" + current.NextSibling.InnerText.Trim() + "\"\r\n")
										: ""); 
			}

			return null;
		}

		[Conditional("DEBUG")]
		private void Log(string msg, Stream data)
		{
			string readToEnd = new StreamReader(data).ReadToEnd();
			Console.WriteLine(msg, readToEnd);
		}

		public override string ToString()
		{
			return _clientFactory.ToString();
		}

		private readonly IHttpClientFactory _clientFactory;
		private bool _isLoggedIn;
		private ICookieContainer _cookies;
		private readonly IHttpInterfaceConfiguration _config;
	}
}
