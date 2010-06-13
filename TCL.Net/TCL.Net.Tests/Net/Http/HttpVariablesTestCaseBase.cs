namespace TCL.Net.Tests.Net.Http
{
	public class HttpVariablesTestCaseBase
	{
		protected static string AddVariable(IHttpClient client, string content, string name, string value)
		{
			return content + "&" + AddVariable(client, name, value);
		}

		protected static string AddVariable(IHttpClient client, string name, string contents)
		{
			client.Variables[name] = contents;
			return name + "=" + contents;
		}

		protected IHttpClient Connect(string url)
		{
			return new TCL.Net.Net.SystemNetHttpClient(null);
		}
	}
}
