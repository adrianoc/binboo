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

using Binboo.Core.Commands.Arguments;
using Binboo.Jira.Commands;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands.Arguments
{
	[TestFixture]
	public class ParamValidatorTestCase
	{
		[Test]
		public void Test()
		{
			Assert.IsTrue(ParamValidator.QuotedString.IsMatch("\"[bla] bla bla\""));
		}

		[Test]
		public void TestButNotAsOptional()
		{
            ParamValidator notOrderOrType = ParamValidator.Anything.ButNot(JiraParamValidator.Order, JiraParamValidator.Type).AsOptional();
			Assert.IsFalse(notOrderOrType.IsMatch("02"));
			Assert.IsFalse(notOrderOrType.IsMatch("type=improvement"));
			Assert.IsTrue(notOrderOrType.IsMatch("anything"));
		}

		[Test]
		public void TestButNot()
		{
            ParamValidator notOrderOrType = ParamValidator.Anything.ButNot(JiraParamValidator.Order, JiraParamValidator.Type);
			Assert.IsFalse(notOrderOrType.IsMatch("02"));
			Assert.IsFalse(notOrderOrType.IsMatch("type=improvement"));
			Assert.IsTrue(notOrderOrType.IsMatch("anything"));
		}

		[Test]
		public void TestAsOptionalRegularExpressionIsNotNullOrEmpty()
		{
			ParamValidator optional = ParamValidator.Anything.AsOptional();
			Assert.IsFalse(string.IsNullOrEmpty(optional.RegularExpression));
		}

		[Test]
		public void TestFromList()
		{
			const string value1 = "Value1";
			const string one = "Value One";

			ParamValidator fromList = ParamValidator.From(new [] {value1, one});

			Assert.IsTrue(fromList.IsMatch(value1));
			Assert.IsTrue(fromList.IsMatch(one));

			Assert.IsTrue(fromList.IsMatch(Quote(value1)));
			Assert.IsTrue(fromList.IsMatch(Quote(one)));
		}

		[Test]
		public void TestAnythingStatingWithText()
		{
			AssertAnythingStartingWithText("UperCase");
			AssertAnythingStartingWithText("lowerCase");
			AssertAnythingStartingWithText("\"lower Case, quoted \"");
			AssertAnythingStartingWithText("\"Uper Case, quoted \"");
			AssertAnythingStartingWithText("\"with multiple\r\nlines between text\"");
			AssertAnythingStartingWithText("\"with numbers in between 1 text\"");
			AssertAnythingStartingWithText("\"with numbers \r\nand new lines\"");
			AssertAnythingStartingWithText("\"with numbers in the end: 2\"");
		}

		private static void AssertAnythingStartingWithText(string candidate)
		{
			Assert.IsTrue(ParamValidator.AnythingStartingWithText.IsMatch(candidate), candidate);
		}

		private static string Quote(string str)
		{
			return "\"" + str + "\"";
		}
	}
}
