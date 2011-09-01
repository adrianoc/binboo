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
using Binboo.Core.Tests.Framework;
using Binboo.Core.Tests.Tests.Support;
using NUnit.Framework;

namespace Binboo.Core.Tests.Tests.Commands
{
	partial class CommandPipingTestCase : ArgumentCollectingTestCaseBase
	{
		private void AssertPiping(string commandLine, Func<string, ArgumentRecorder, bool> argumentChecker)
		{
		    SendCommandAndCollectResult(commandLine, null, argumentChecker);
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
    }
}