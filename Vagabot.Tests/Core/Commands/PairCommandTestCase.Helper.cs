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
using System.Text.RegularExpressions;
using Binboo.Core.Configuration;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	partial class PairCommandTestCase
	{
		private static void AssertPairsAreValid(string expectedMessageRegExp, string actual)
		{
			MatchCollection matches = Regex.Matches(actual, expectedMessageRegExp);
			CollectionAssert.AreEquivalent(ConfigServices.PairingUsers.Select(userName => FirstUsableName(userName)), DevNamesFrom(matches[0].Groups));
		}

		private static string FirstUsableName(string jiraName)
		{
			int separatorIndex = jiraName.IndexOfAny(new[] {' ', '@'});
			return separatorIndex > 0 ? jiraName.Substring(0, separatorIndex) : jiraName;
		}

		private static IEnumerable<string> DevNamesFrom(GroupCollection groups)
		{
			int skip = 1;
			foreach (Group @group in groups)
			{
				if (skip > 0)
				{
					skip--;
					continue;
				}
				yield return @group.Value;
			}
		}

		private static string ExpectedMessageRegExp()
		{
			return @"Pairs: \[(.*[^,]), (.*[^,])\], \[(.*[^,]), (.*[^,])\]";
		}
	}
}
