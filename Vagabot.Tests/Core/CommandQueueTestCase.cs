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
using Binboo.Core;
using NUnit.Framework;
using SKYPE4COMLib;

namespace Binboo.Tests.Core
{
	[TestFixture]
	public partial class CommandQueueTestCase
	{
		[Test]
		public void TestExit()
		{
			AssertBlock(state =>
			            	{
			            		_queue.Add(new ChatMessage());
			            		_exitEvent.Set();

			            		IChatMessage message = _queue.Next();

			            		_assert = () => Assert.IsNull(message);
			            	});
		}

		[Test]
		public void TestNoMessage()
		{
			AssertBlock( state =>
			{
				IChatMessage actual = _queue.Next();
			    _assert = () => Assert.IsNull(actual);
			 });
		}

		[Test]
		public void TestMultipleMessagesSingleThread()
		{
			const int msgCount = 3;
			foreach (var value in Enumerable.Range(0, msgCount))
			{
				_queue.Add(NewChatMessage(value));
			}
			
			AssertBlock(state =>
			{
				_assert = () =>
				          	{
				          		foreach (var actualValue in Enumerable.Range(0, msgCount))
				          		{
				          			IChatMessage actual = _queue.Next();
									Assert.IsNotNull(actual, string.Format("Expected: {0}", actualValue));
									Assert.AreEqual(actualValue.ToString(), actual.Body);
				          		}

				          		Assert.IsNull(_queue.Next());
				          	};
			});
		}

		[SetUp]
		public void Setup()
		{
			_exitEvent.Reset();
			_queue = new CommandQueue(_exitEvent);
		}
	}
}
