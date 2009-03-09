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

using System.Linq;
using System.Text;
using System.Threading;
using Binboo.Tests.Mocks;
using NUnit.Framework;
using Application=Binboo.Core.Application;

namespace Binboo.Tests
{
	[TestFixture]
	public class JiraCommandBaseTest
	{
		private Application _app;
		private SkypeMock _mockSkype;
		private ChatMock _chat;

		[SetUp]
		public void SetUp()
		{
			_app = new Application("test");

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

		[Test]
		public void TestInvalidCommand()
		{
			_mockSkype.SendMessage("adriano", "$test InvalidCommand");
			Thread.Sleep(100);
			Assert.AreEqual("Unknown command: InvalidCommand.", _chat.SentMessages.Single().Body);
		}

		[Test]
		public void TestSingleArgument()
		{
			BotCommandMock command = new BotCommandMock("IamACommand", context => context.Arguments[0]);
			_app.AddCommand(command);

			_mockSkype.SendMessage("adriano", "$test IamACommand yeah");
			Assert.IsTrue(command.Wait(1000));
			Assert.AreEqual("yeah", _chat.SentMessages.Single().Body);
		}

		[Test]
		public void TestMultipleArguments()
		{
			var command = new BotCommandMock("IamACommand", context =>
			                                                           	{
																			var sb = new StringBuilder(50);
			                                                           		foreach (var arg in context.Arguments)
			                                                           		{
			                                                           			sb.AppendLine(arg);
			                                                           		}
			                                                           		return sb.ToString();
			                                                           	});

			_app.AddCommand(command);

			_mockSkype.SendMessage("adriano", "$test IamACommand arg#1 \"arg #2\" arg#3");
			Assert.IsTrue(command.Wait(1000));
			Assert.AreEqual("arg#1\r\narg #2\r\narg#3\r\n", _chat.SentMessages.Single().Body);
		}
	}
}