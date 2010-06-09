using NUnit.Framework;

namespace TCL.Net.Tests.Net.Http
{
	[TestFixture]
	public class ContentEncodingcs : HttpVariablesTestCaseBase
	{
		private const string SPACES_IN_BETWEEN_NAME = "spaces-in-the-middle";
		private const string SPACES_BORDER_NAME = "spaces-in-the-borders";
		private const string SPACES_IN_BETWEEN = "a b";
		private const string SPACES_BORDER_VALUE = " ab ";

		[Test]
		public void TestSpaces()
		{
			HttpClient client = new HttpClient("");

			client.Variables[SPACES_IN_BETWEEN_NAME] = SPACES_IN_BETWEEN;
			client.Variables[SPACES_BORDER_NAME] = SPACES_BORDER_VALUE;

			Assert.AreEqual(SPACES_IN_BETWEEN_NAME + "=" + SPACES_IN_BETWEEN.Replace(" ", "%20") + "\r\n" + 
			                SPACES_BORDER_NAME + "=" + SPACES_BORDER_VALUE.Replace(" ", "%20"),
							
							client.Variables.ToString());
		}
	}
}
