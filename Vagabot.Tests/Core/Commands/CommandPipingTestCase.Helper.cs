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
using System.Linq;
using System.Linq.Expressions;
using Binboo.Core;
using Binboo.Core.Commands.Arguments;
using Binboo.Tests.Core.Support;
using Binboo.Tests.Mocks;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	partial class CommandPipingTestCase : TestWithUser
	{
		[SetUp]
		public void Setup()
		{
			_app = new Application("test");
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

		private void AssertPiping(string commandLine, Func<string, ArgumentRecorder, bool> argumentChecker)
		{
			ArgumentRecorder recorder = ArgumentRecorder.NewInstance();
			
			var command1 = new JiraCommandMock("CMD1", recorder.HandlerFor("CMD1"), _validatorExpressions);
			var command2 = new JiraCommandMock("CMD2", recorder.HandlerFor("CMD2"), _validatorExpressions);
			var command3 = new JiraCommandMock("CMD3", recorder.HandlerFor("CMD3"), _validatorExpressions);

			_app.AddCommand(command1);
			_app.AddCommand(command2);
			_app.AddCommand(command3);
			_chat.Reset();

			_mockSkype.SendMessage("user", commandLine);
			var gotAnswer = _chat.WaitForMessages(2000);
			
			Assert.AreEqual(0, _errors.Count, _errors.Aggregate("", (acc, curr) => curr + "\r\n" + acc));
			
			Assert.IsTrue(gotAnswer, "No answer for message '{0}' in 2000 ms", commandLine);

			foreach(var msg in _chat.SentMessages)
			{
				Console.WriteLine(msg.Body);
			}

			Assert.IsTrue(argumentChecker(_chat.SentMessages.ElementAt(0).Body, recorder), string.Format("Command Line: {0}\r\n   Recorded: {1}", commandLine, recorder));
		}

		private static bool AreEqual(IEnumerable<string> lhs, params string[] rhs)
		{
			var actual = lhs.ToArray();

			for(int i =0; i < actual.Length; i++)
			{
				if (!actual[i].Equals(rhs[i]))
				{
					Console.WriteLine("Enumerables differs at position {0}\r\n\tExpected: {1}\r\n\t  Actual: {2}", i, rhs[i], actual[i]);
					return false;
				}
			}

			Assert.AreEqual(rhs.Length, actual.Length);
			return true;
		}

		private Application _app;
		private SkypeMock _mockSkype;
		private ChatMock _chat;
		private readonly IList<string> _errors = new List<string>();
		private readonly Expression<Func<int, ParamValidator>>[] _validatorExpressions = new Expression<Func<int, ParamValidator>>[]
		                                                                       	{
																					s1 => ParamValidator.Custom("(?:\\s*)s1", true), 
																					s2 => ParamValidator.Custom("(?<param>s2)", true), 
																					s3 => ParamValidator.Custom("(?:\\s*)s3", true), 
																					s3 => ParamValidator.Custom("(?:\\s*)s3", true), 
																					s4 => ParamValidator.Custom("(?:\\s*)s4", true), 
																					multiple => ParamValidator.Custom(@"(?<param>m[0-9])((?:\s*,\s*)(?<param>m[0-9]))*", true),
																				};

	}
}