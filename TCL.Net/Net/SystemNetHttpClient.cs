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
using System.Net;

namespace TCL.Net.Net
{
	public class SystemNetHttpClient : IHttpClient
	{
		public SystemNetHttpClient(HttpWebRequest request)
		{
			_request = request;
		}

		public void Post()
		{
			_response = null;

			_request.UserAgent = "Binboo";
			_request.Method = "post";
			_request.Headers.Add("X-Atlassian-Token", "no-check");
			_request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			_request.CookieContainer = _cookies;

			string contents = _variables.ToString();
			_request.ContentLength = contents.Length;
			
			using (var writer = new StreamWriter(_request.GetRequestStream()))
			{
				writer.Write(contents);
				writer.Flush();
			}

			_response = (HttpWebResponse) _request.GetResponse();
			LastStatus = _response.StatusCode;
		}

		public HttpStatus Status
		{
			get { return _status; }
		}

		public Stream ResponseStream
		{
			get
			{
				return _response.GetResponseStream();
			}
		}

		public ICookieContainer Cookies
		{
			get
			{
				return new CookieContainerImpl(_cookies);
			}

			set
			{
				if (value == null) return;
				_cookies =  ((CookieContainerImpl) value).Cookies;
			}
		}

		public HttpVariables Variables
		{
			get { return _variables; }
		}

		protected HttpStatusCode LastStatus
		{
			set
			{
				switch (value)
				{
					case HttpStatusCode.OK:
						_status = HttpStatus.Ok;
						break;

					case HttpStatusCode.MovedPermanently:
					case HttpStatusCode.Redirect:
					case HttpStatusCode.SeeOther:
					case HttpStatusCode.TemporaryRedirect:
						_status = HttpStatus.Redirect;
						break;

					default:
						throw new ArgumentException("HTTP status not supported: " + value);
				}
			}
		}

		private CookieContainer _cookies = new CookieContainer();
		private readonly HttpWebRequest _request;
		private HttpWebResponse _response;
		private HttpStatus _status;
		private HttpVariables _variables = new HttpVariables();
	}
}
