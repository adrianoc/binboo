using NUnit.Framework;

namespace TCL.Net.Tests.Net.Http
{
	[TestFixture]
	public class ContentLengthTestCase : HttpVariablesTestCaseBase
	{
		[Test]
		public void TestSimple()
		{
			var http = new HttpClient("localhost");
			http.Method = HttpMethod.Post;
			
			string variablesContent = AddVariable(http, "name", "jhondoe");

			Assert.AreEqual(variablesContent, http.Variables.ToString());
			Assert.AreEqual(12, http.ContentLength);
		}

		[Test]
		public void TestMultipleVariables()
		{
			var http = new HttpClient("localhost");
			http.Method = HttpMethod.Post;

			string variablesContent = AddVariable(http, "name", "jhondoe");
			variablesContent = AddVariable(http, variablesContent, "age", "20");

			Assert.AreEqual(variablesContent, http.Variables.ToString());
			Assert.AreEqual(18, http.ContentLength);
		}
	}

}
