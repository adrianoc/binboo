/**
 * Copyright (c) 2009 Adriano Carlos Verona
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
using System.Text;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Tests.Mocks;
using NUnit.Framework;
using Application=Binboo.Core.Application;

namespace Binboo.Tests
{
	public class JiraCommandBaseTest
	{
		protected Application _app;
		protected SkypeMock _mockSkype;
		protected ChatMock _chat;
		protected IList<string> _errors = new List<string>();

		[SetUp]
		public void SetUp()
		{
			_app = new Application("test");
			_app.Error += (sender, e) => _errors.Add(e.Message + "\r\n" + e.Details);

			//var mockChat = new Mock<Chat>();
			//mockChat.Expect(
			//    c => c.SendMessage(It.IsAny<string>())).Callback(delegate
			//    {
			//        var mockChatMessage = new Mock<ChatMessage>();
			//        mockChatMessage.Expect(cm => cm.Body).Returns("");
			//        _messages.Add(mockChatMessage.Object);
			//    });

			_chat = new ChatMock(new UserMock("test", false));
			_mockSkype = new SkypeMock(() => _chat);
			_app.SetSkype(_mockSkype);
			_app.AttachToSkype();
		}

		//[Test]
		//public void TestInvalidCommand()
		//{
		//    _mockSkype.SendMessage("adriano", "$test InvalidCommand");
		//    Thread.Sleep(100);
		//    Assert.AreEqual("Unknown command: InvalidCommand.", _chat.SentMessages.Single().Body);
		//}
		internal static string ArgumentEcho(Context context, IDictionary<string, Argument> args)
		{
			var sb = new StringBuilder(50);
			foreach (var arg in args)
			{
				sb.AppendFormat("{0}:", arg.Key);
				foreach (var item in arg.Value.Values)
				{
					sb.AppendFormat("{{{0}}}", item);
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		protected void SendMessageAndAssertNoErrors(string user, string message)
		{
			_mockSkype.SendMessage(user, message);
			if (_errors.Count > 0)
			{
				Assert.Fail( _errors.Aggregate((acc, current) => acc + "\r\n" + current));
			}
		}
	}
}