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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Tests.Framework;
using Binboo.Jira.Commands;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Arguments
{
    [TestFixture]
    class ArgumentsTestCase : ArgumentCollectingTestCaseBase
    {
        [Test]
        public void TestMultipleItemsSeparatedByComma()
        {
            AssertArguments(
                    "$test IamACommand TEST-10, TEST-11, TEST-12 \"TEST-20, TEST-21\" arg#3",
                    "a1:{TEST-10}{TEST-11}{TEST-12}\r\na2:{TEST-20, TEST-21}\r\na3:{arg#3}\r\n",
                    a1 => JiraParamValidator.MultipleIssueId,
                    a2 => ParamValidator.Anything,
                    a3 => ParamValidator.Custom("arg\\#3", false));
        }

        [Test]
        public void TestInvalidMultipleItemsSeparatedByComma()
        {
            AssertInvalidArgumentCount(
                    "$test IamACommand TEST-10, arg#2",
                    "IamACommand: Unmached arguments: \r\n\r\nTEST-10, arg#2\r\n       ^^^^^^^",
                    a1 => JiraParamValidator.MultipleIssueId);
        }

        [Test]
        public void TestMissingCommaBetweenMultipleItems()
        {
            AssertInvalidArgumentCount(
                    "$test IamACommand TEST-10 TEST-20 8",
                    "IamACommand: Invalid arguments count. Expected at least 1 and no more than 2, got 3\r\n\r\n0: TEST-10\r\n8: TEST-20\r\n16: 8\r\n\r\n\r\n",
                    a1 => JiraParamValidator.MultipleIssueId,
                    a2 => JiraParamValidator.Order.AsOptional());
        }

        [Test]
        public void TestSingleItemInMultipleItemsArg()
        {
            AssertArguments(
                    "$test IamACommand TEST-10",
                    "a1:{TEST-10}\r\n",
                    a1 => JiraParamValidator.MultipleIssueId);

            AssertArguments(
                    "$test IamACommand TEST-10, TEST-11",
                    "a1:{TEST-10}{TEST-11}\r\n",
                    a1 => JiraParamValidator.MultipleIssueId);

            AssertArguments(
                    "$test IamACommand TEST-10 \"TEST-20, TEST-21\"",
                    "a1:{TEST-10}\r\na2:{TEST-20, TEST-21}\r\n",
                    a1 => JiraParamValidator.MultipleIssueId,
                    a2 => ParamValidator.Anything);

            AssertArguments(
                    "$test IamACommand TEST-10 arg#3",
                    "a1:{TEST-10}\r\na3:{arg#3}\r\n",
                    a1 => JiraParamValidator.MultipleIssueId,
                    a3 => ParamValidator.Custom("arg\\#3", false));
        }

        [Test]
        public void TestSingleItemWith4Digits()
        {
            AssertArguments(
                    "$test IamACommand TEST-1234",
                    "a1:{TEST-1234}\r\na2:\r\n",
                    a1 => JiraParamValidator.MultipleIssueId,
                    a2 => ParamValidator.Custom("arg\\#2", true));
        }

        [Test]
        public void TestMixedOptionalAndNonOptionalArgumentsThrows()
        {
            AssertInvalidArgumentCount("$test IamACommand x y z", "Mixed validators (optional/non-optional) are not supported. Validators should follow the order: no, no, ..., op, op, ... op",
                                                    a1 => JiraParamValidator.IssueId,
                                                    a2 => JiraParamValidator.IssueId.AsOptional(),
                                                    a3 => JiraParamValidator.IssueId);

        }

        [Test]
        public void TestType()
        {
            AssertArguments(
                    "$test IamACommand type=task",
                    "type:{task}\r\n",
                    type => JiraParamValidator.Type);
        }

        private void AssertInvalidArgumentCount(string message, string expectedResult, params Expression<Func<int, ParamValidator>>[] validatorExpressions)
        {
            var actual = string.Empty;
            SendCommandAndCollectResult(message, validatorExpressions, (msg, recorder) =>
            {
                actual = msg;
                return true;
            });
            Assert.That(actual, Is.EqualTo(expectedResult));
        }

        private void AssertArguments(string message, string expectedEchoedArgs, params Expression<Func<int, ParamValidator>>[] validatorExpressions)
        {
            var sb = new StringBuilder();
            SendCommandAndCollectResult(message, validatorExpressions, (msg, recorder) =>
            {
                foreach (var entry in recorder.Arguments().Values)
                {
                    sb.Append(ArgumentEcho(entry));
                }

                return true;
            });
            Assert.That(sb.ToString(), Is.EqualTo(expectedEchoedArgs));
        }

        private static string ArgumentEcho(IEnumerable<KeyValuePair<string, Argument>> args)
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
    }
} 
