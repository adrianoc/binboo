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

using Binboo.Core.Commands.Arguments;
using NUnit.Framework;

namespace Binboo.Core.Tests.Tests.Commands
{
	[TestFixture]
	partial class CommandPipingTestCase
	{
		[Test]
		public void TestSingleOutputNoExtraArguments()
		{
			AssertPiping("$test CMD1 s1 | CMD2", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "s1"));
		}
		
		[Test]
		public void TestSingleOutputWithExtraArguments()
		{
			AssertPiping("$test CMD1 s1 | CMD2 s2", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "s1", "s2"));
		}
		
		[Test]
		public void TestMultipleOutputSingleArgument()
		{
			AssertPiping("$test CMD1 m1, m2, m3 | CMD2", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "m1", "m2", "m3"));
		}

		[Test]
		public void TestMultipleOutputMultipleArguments()
		{
			AssertPiping("$test CMD1 s1 s2 m1, m2, m3 | CMD2", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "s1", "s2", "m1", "m2", "m3"));
		}

		[Test]
		public void TestOutputIndexing()
		{
			AssertPiping("$test CMD1 m1, m2 | CMD2 %1%", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "m2"));
			AssertPiping("$test CMD1 m1 | CMD2 m3,%0%,m2 | CMD3", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "m3", "m1", "m2") && AreEqual(rec.ArgumentsFor("CMD3"), "m3", "m1", "m2"));
			AssertPiping("$test CMD1 m1 | CMD2 m3,%0%,m2 | CMD3 %1%,m2,%0%", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "m3", "m1", "m2") && AreEqual(rec.ArgumentsFor("CMD3"), "m1", "m2", "m3"));
			AssertPiping("$test CMD1 s1 m1, m2 | CMD2 %0% %2%", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "s1", "m2"));
		}

		[Test]
		public void TestMultiplePipesSimple()
		{
			AssertPiping("$test CMD1 m1 | CMD2 | CMD3", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "m1") && AreEqual(rec.ArgumentsFor("CMD3"), "m1"));
		}

		[Test]
		public void TestMultiplePipesComplex()
		{
			AssertPiping("$test CMD1 s1 | CMD2 m1, m2 | CMD3", (msgs, rec) => AreEqual(rec.ArgumentsFor("CMD2"), "s1", "m1", "m2") && AreEqual(rec.ArgumentsFor("CMD3"), "s1", "m1", "m2"));
		}

		[Test]
		public void TestExplicitSArgumentExpansionSeparators()
		{
			AssertPiping(	"$test CMD1 s1 s2 | CMD_SINGLE_ARG PARAM_%[/ ]%",
							(msgs, rec) => AreEqual(rec.ArgumentsFor("CMD_SINGLE_ARG"), "PARAM_s1/ s2"),
							s1 => ParamValidator.Custom("s1", true),
							s2 => ParamValidator.Custom("s2", true),
							multiple => ParamValidator.Custom("PARAM_.*", true)
							);
		}
	}
}