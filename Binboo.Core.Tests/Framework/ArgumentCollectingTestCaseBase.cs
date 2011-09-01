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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Tests.Tests;
using Binboo.Core.Tests.Tests.Support;
using Binboo.Tests.Mocks;
using NUnit.Framework;

namespace Binboo.Core.Tests.Framework
{
    public class ArgumentCollectingTestCaseBase : TestWithUser
    {
		[SetUp]
		public void Setup()
		{
		    _app = Application.WithPluginsFrom(new TypeCatalog(typeof (ArgumentEchoingPlugin)));
			_app.Error += (sender, e) => _errors.Add(e.Message + "\r\n" + e.Details);

			_chat = new ChatMock(NewUserMock());
			_mockSkype = new SkypeMock(() => _chat);
			_app.SetSkype(_mockSkype);
			_app.AttachToSkype();
		}

		[TearDown]
		public void TearDown()
		{
			_app.Dispose();
		}

		protected void SendCommandAndCollectResult(string commandLine, Expression<Func<int, ParamValidator>>[] validatorExpressions, Func<string, ArgumentRecorder, bool> argumentChecker)
		{
            try
            {
                var recorder = ArgumentRecorder.NewInstance();
                ArgumentEchoingPlugin.ArgumentRecorder = recorder;
                ArgumentEchoingPlugin.ValidatorExpressions = validatorExpressions;

                _chat.Reset();

                _mockSkype.SendMessage("user", commandLine);
                const int timeout = 1000;
                var gotAnswer = _chat.WaitForMessages(timeout);

                Assert.AreEqual(0, _errors.Count, _errors.Aggregate("", (acc, curr) => curr + "\r\n" + acc));

                Assert.IsTrue(gotAnswer, "No answer for message '{0}' in {1} ms", commandLine, timeout);

                foreach (var msg in _chat.SentMessages)
                {
                    Console.WriteLine(msg.Body);
                }

                Assert.IsTrue(argumentChecker(_chat.SentMessages.ElementAt(0).Body, recorder),
                              string.Format("Command Line: {0}\r\n   Recorded: {1}", commandLine, recorder));
            }
            catch(Exception)
            {
                ArgumentEchoingPlugin.ValidatorExpressions = null;
                ArgumentEchoingPlugin.ArgumentRecorder = null;
                throw;
            }
		}

	    private Application _app;
		private SkypeMock _mockSkype;
		private ChatMock _chat;
		private readonly IList<string> _errors = new List<string>();
	}
}
