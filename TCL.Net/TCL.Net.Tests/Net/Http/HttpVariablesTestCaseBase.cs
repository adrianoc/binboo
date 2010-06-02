namespace TCL.Net.Tests.Net.Http
{
	public class HttpVariablesTestCaseBase
	{
		protected static string AddVariable(HttpClient client, string content, string name, string value)
		{
			return content + "\r\n" + AddVariable(client, name, value);
		}

		protected static string AddVariable(HttpClient client, string name, string contents)
		{
			client.Variables[name] = contents;
			return name + "=" + contents;
		}
	}
}
