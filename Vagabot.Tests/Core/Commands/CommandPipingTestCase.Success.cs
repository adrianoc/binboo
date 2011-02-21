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
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	partial class CommandPipingTestCase
	{
		[Test]
		public void TestSingleOutputNoExtraArguments()
		{
			AssertPiping("$test CMD1 s1 | CMD2", rec => AreEqual(rec.Arguments("CMD2"), "s1"));
		}
		
		[Test]
		public void TestSingleOutputWithExtraArguments()
		{
			AssertPiping("CMD1 s1 | CMD2 s2", rec => AreEqual(rec.Arguments("CMD2"), "s1", "s2"));
		}
		
		[Test]
		public void TestMultipleOutputSingleArgument()
		{
			Assert.Fail();
		}

		[Test]
		public void TestMultipleOutputMultipleArguments()
		{
			Assert.Fail();
		}

		[Test]
		public void TestOutputIndexing()
		{
			Assert.Fail();
		}

		[Test]
		public void TestMultiplePipes()
		{
			Assert.Fail();
		}
		
		[Test]
		public void TestPipeSymbolInQuotedStrings()
		{
			AssertPiping("$test CMD1 \"no | piping\" | CMD2", rec => AreEqual(rec.Arguments("CMD2"), "\"no | piping\""));
			AssertPiping("$test CMD1 s1 \"no | piping\" | CMD2", rec => AreEqual(rec.Arguments("CMD2"), "s1", "\"no | piping\""));
		}
	}
}