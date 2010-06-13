using NUnit.Framework;

namespace TCL.Net.Tests.Net.Http
{
	[TestFixture]
	public class ContentLengthTestCase : HttpVariablesTestCaseBase
	{
		[Test]
		public void TestSimple()
		{
			var http = Connect("localhost");
			
			AddVariable(http, "name", "johndoe");
			Assert.AreEqual("name=johndoe", http.Variables.ToString());
		}

		[Test]
		public void TestMultipleVariables()
		{
			var http = Connect("localhost");

			AddVariable(http, AddVariable(http, "name", "johndoe"), "age", "20");

			Assert.AreEqual("name=johndoe&age=20", http.Variables.ToString());
		}
	}

}
