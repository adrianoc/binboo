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
using NUnit.Framework;

namespace TCL.Net.Tests.Net.Http
{
	[TestFixture]
	public class ContentEncodingcs : HttpVariablesTestCaseBase
	{
		private const string SPACES_IN_BETWEEN_NAME = "spaces-in-the-middle";
		private const string SPACES_BORDER_NAME = "spaces-at-the-borders";
		private const string SPACES_IN_BETWEEN = "a b";
		private const string SPACES_BORDER_VALUE = " ab ";

		[Test]
		public void TestSpaces()
		{
			IHttpClient client = Connect("");

			client.Variables[SPACES_IN_BETWEEN_NAME] = SPACES_IN_BETWEEN;
			client.Variables[SPACES_BORDER_NAME] = SPACES_BORDER_VALUE;

			Assert.AreEqual(
							SPACES_IN_BETWEEN_NAME + "=" + SPACES_IN_BETWEEN.Replace(" ", "%20") + "&" + 
							SPACES_BORDER_NAME + "=" + SPACES_BORDER_VALUE.Replace(" ", "%20"),
							
							client.Variables.ToString());
		}
	}
}
