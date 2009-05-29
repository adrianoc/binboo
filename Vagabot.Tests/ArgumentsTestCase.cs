﻿/**
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

using System;
using System.Linq;
using System.Linq.Expressions;
using Binboo.Core.Commands.Arguments;
using Binboo.Tests.Mocks;
using NUnit.Framework;

namespace Binboo.Tests
{
	[TestFixture]
	public class ArgumentsTestCase : JiraCommandBaseTest
	{
		[Test]
		public void TestNoArguments()
		{
			AssertArguments("$test IamACommand", "anything:\r\n", anything => ParamValidator.Anything.AsOptional());
		}

		[Test]
		public void TestSingleArgument()
		{
			AssertArguments("$test IamACommand yeah", "yeah:{yeah}\r\n", yeah => ParamValidator.Anything);
		}

		[Test]
		public void TestMultipleArguments()
		{
			AssertArguments("$test IamACommand arg#1 \"arg#2\" arg#3", "a1:{arg#1}\r\na2:{arg#2}\r\na3:{arg#3}\r\n", a1 => ParamValidator.Anything, a2 => ParamValidator.Anything, a3 => ParamValidator.Anything);
		}

		[Test]
		public void TestMultipleItemsSeparatedByComma()
		{
			AssertArguments(
					"$test IamACommand TEST-10, TEST-11, TEST-12 \"TEST-20, TEST-21\" arg#3", 
					"a1:{TEST-10}{TEST-11}{TEST-12}\r\na2:{TEST-20, TEST-21}\r\na3:{arg#3}\r\n", 
					a1 => ParamValidator.MultipleIssueId,
					a2 => ParamValidator.Anything,
					a3 => ParamValidator.Custom("arg\\#3", false));
		}

		[Test]
		public void TestSingleItemInMultipleItemsArg()
		{
			AssertArguments(
					"$test IamACommand TEST-10",
					"a1:{TEST-10}\r\n",
					a1 => ParamValidator.MultipleIssueId);

			AssertArguments(
					"$test IamACommand TEST-10, TEST-11",
					"a1:{TEST-10}{TEST-11}\r\n",
					a1 => ParamValidator.MultipleIssueId);

			AssertArguments(
					"$test IamACommand TEST-10 \"TEST-20, TEST-21\"",
					"a1:{TEST-10}\r\na2:{TEST-20, TEST-21}\r\n",
					a1 => ParamValidator.MultipleIssueId,
					a2 => ParamValidator.Anything);

			AssertArguments(
					"$test IamACommand TEST-10 arg#3",
					"a1:{TEST-10}\r\na3:{arg#3}\r\n",
					a1 => ParamValidator.MultipleIssueId,
					a3 => ParamValidator.Custom("arg\\#3", false));
		}

		[Test]
		public void TestSingleItemWith4Digits()
		{
			AssertArguments(
					"$test IamACommand TEST-1234",
					"a1:{TEST-1234}\r\na2:\r\n",
					a1 => ParamValidator.MultipleIssueId,
					a2 => ParamValidator.Custom("arg\\#2", true));
		}


		[Test]
		public void TestLessActualArgumentsThanParameters()
		{
			AssertArguments(
					"$test IamACommand arg#1 arg#2",
					"a1:{arg#1}\r\na2:{arg#2}\r\na3:\r\n",
					a1 => ParamValidator.Custom(@"arg\#1", false),
					a2 => ParamValidator.Custom(@"arg\#2", false),
					a3 => ParamValidator.Custom(@"arg\#3", true));
		}

		[Test]
		public void TestMixedOptionalAndNonOptionalArgumentsThrows()
		{
			JiraCommandMock command = new JiraCommandMock("IamACommand", ArgumentEcho, 
													a1 => ParamValidator.IssueId,
													a2 => ParamValidator.IssueId.AsOptional(),
													a3 => ParamValidator.IssueId);

			_app.AddCommand(command);
			_chat.Reset();

			_mockSkype.SendMessage("adriano", "$test IamACommand x y z");
			command.Wait(500);
			Assert.AreEqual(1, _errors.Count, _errors.Aggregate((acc, err) => acc + "\r\n" + err));
			Assert.IsTrue(_errors[0].Contains("Mixed"));
		}

		private void AssertArguments(string message, string expected, params Expression<Func<int, ParamValidator>>[] validatorExpressions)
		{
			JiraCommandMock command = new JiraCommandMock("IamACommand", ArgumentEcho, validatorExpressions);
			_app.AddCommand(command);
			_chat.Reset();

			_mockSkype.SendMessage("adriano", message);
			Assert.IsTrue(command.Wait(1000));
			Assert.AreEqual(expected, _chat.SentMessages.Single().Body);
		}
	}
}
