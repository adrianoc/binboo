using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TCL.Net
{
	public class HttpClient : IHttpClient
	{
		public HttpClient(string url)
		{
			_url = url;
			Method = HttpMethod.Get;
		}

		public HttpMethod Method
		{
			get; set;
		}

		public HttpVariables Variables
		{
			get { return _variables; }
		}

		public int ContentLength
		{
			get
			{
				int length = 0;
				foreach (var variable in _variables)
				{
					length += variable.Length + 1 + _variables[variable].Length;
				}

				return length;
			}
		}

		public void Open()
		{
			throw new NotImplementedException();
		}

		public void Post(params string[] values)
		{
			throw new NotImplementedException();
		}

		public HttpStatus Status
		{
			get { throw new NotImplementedException(); }
		}

		public Stream ResponseStream
		{
			get { throw new NotImplementedException(); }
		}

		public IList<IHttpCookie> Cookies
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		private string _url;
		private readonly HttpVariables _variables = new HttpVariables();
	}

	public class HttpVariables : IEnumerable<string>
	{
		public string this[string name]
		{
			get
			{
				if (_variables.ContainsKey(name)) return _variables[name];
				return null;
			}

			set 
			{
				_variables[name] = value; 
			}
		}

		public override string ToString()
		{
			var output = new StringBuilder();
			foreach (var pair in _variables)
			{
				output.AppendFormat("{0}={1}\r\n", pair.Key, Uri.EscapeDataString(pair.Value));
			}

			return output.Remove(output.Length - 2, 2).ToString();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return _variables.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		private readonly IDictionary<string, string> _variables = new Dictionary<string, string>();
	}
}
