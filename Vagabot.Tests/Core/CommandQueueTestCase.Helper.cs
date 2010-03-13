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
using System.Threading;
using Binboo.Core;
using SKYPE4COMLib;

namespace Binboo.Tests.Core
{
	partial class CommandQueueTestCase
	{
		private static IChatMessage NewChatMessage(int value)
		{
			ChatMessage message = new Mocks.ChatMessageMock(null, value.ToString(), null);
			return message;
		}

		private void AssertBlock(WaitCallback testBlock)
		{
			ThreadPool.QueueUserWorkItem(
				state =>
				{
					testBlock(state);
					_finishedMethod.Set();
				},

				_queue);

			_finishedMethod.WaitOne();
			_assert();
		}

		private CommandQueue _queue;
		private readonly EventWaitHandle _exitEvent = new ManualResetEvent(false);
		private readonly EventWaitHandle _finishedMethod = new AutoResetEvent(false);
		private Action _assert = () => { };
	}
}
