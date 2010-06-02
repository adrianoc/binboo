using NUnit.Framework;

namespace TCL.Net.Tests.Net.Http
{
	[TestFixture]
	public class ContentEncodingcs : HttpVariablesTestCaseBase
	{
		private const string SPACES_MIDLE_NAME = "spaces-in-the-middle";
		private const string SPACES_BORDER_NAME = "spaces-in-the-borders";
		private const string SPACES_MIDLE_VALUE = "a b";
		private const string SPACES_BORDER_VALUE = " ab ";

		[Test]
		public void TestSpaces()
		{
			HttpClient client = new HttpClient("");

			client.Variables[SPACES_MIDLE_NAME] = SPACES_MIDLE_VALUE;
			client.Variables[SPACES_BORDER_NAME] = SPACES_BORDER_VALUE;

			Assert.AreEqual(SPACES_MIDLE_VALUE + "=" + SPACES_MIDLE_VALUE.Replace(' ', '+') + "\r\n" + 
			                SPACES_BORDER_NAME + "=" + SPACES_BORDER_VALUE.Replace(' ' , '+'),
							
							client.Variables.ToString());
		}
	}
}
