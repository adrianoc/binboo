/**
 * Copyright (c) 2011 Adriano Carlos Verona
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Binboo.Tests.Mocks;
using NUnit.Framework;
using Application = Binboo.Core.Application;

namespace Binboo.Tests.Core
{
	[TestFixture]
	class ApplicationTestCase : TestWithUser
	{
		[Test]
		public void MissedMessagesAreProcessed()
		{
			var app = new Application("test");
			
			var chat = new ChatMock(NewUserMock());

			var skype = new SkypeMock(() => chat, MissedMessages()); 
		
			app.SetSkype(skype);
			app.AttachToSkype();

			Assert.IsTrue(chat.WaitForMessages(1000));

			AssertErrorResponse(chat, "Unknown command: cmd1.");
			AssertErrorResponse(chat, "Unknown command: cmd2.");
			AssertErrorResponse(chat, "Unknown command: cmd3.");
		}

		private static string[] MissedMessages()
		{
			return new[] {"no no no", "$test cmd1", "bla", "$test cmd2", "$test cmd3"};
		}

		private static void AssertErrorResponse(ChatMock chat, string errorMsg)
		{
			Assert.IsTrue(chat.SentMessages.Select(m => m.Body).Contains(errorMsg), string.Format("Expected response not fould: '{0}'", errorMsg));
		}

		[Test]
		public void TestCommandLinePipingSplit()
		{
			foreach(var testPair in _testSpecification)
			{
				AssertPipingSplit(testPair);
			}
		}

		private void AssertPipingSplit(string testCommandLine)
		{
			var expected = Regex.Split(testCommandLine, "\\|@").Select(expectedItem => expectedItem.Trim());
			var actual = Application.CommandsFor(testCommandLine).Select(actualCmd => actualCmd.Replace("@ ", ""));

			CollectionAssert.AreEqual(expected, actual, expected.Aggregate("Expected: ",  (acc, curr) => acc + " / " + curr) + actual.Aggregate("\r\n    Actual: ", (acc, curr) => acc + " / " + curr) );
		}

		private static IList<string> _testSpecification = new[]
		                                                     	{
		                                                     		"$cmd1 arg1",
		                                                     		"cmd1 arg1 |@ cmd2",
		                                                     		"cmd1 \"arg1 | bar\" |@ cmd2",
		                                                     		"cmd1 \"arg1 | bar |\"  arg2 |@ cmd2",
		                                                     		"cmd1 \"arg1 | bar |\"  arg2 |@ cmd2 arg3 |@ cmd3",
		                                                     		"cmd1 \"arg1 | bar |\"  \"|\" |@ cmd2 arg3 |@ cmd3",
																};
	}
}
