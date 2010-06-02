using System.Collections.Generic;
using System.IO;

namespace TCL.Net
{
	public interface IHttpClient
	{
		void Post(params string[] values);
		HttpStatus Status { get; }
		Stream ResponseStream { get; }
		IList<IHttpCookie> Cookies { get; set; }
	}

	public enum HttpStatus
	{
		OK = 200,
		REDIRECT
	}
}
