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
using System.Threading;
using SKYPE4COMLib;

namespace Binboo.Core
{
	public class CommandQueue
	{
		public CommandQueue(EventWaitHandle quitEvent)
		{
			_quitEvent = quitEvent;
		}

		public void Add(IChatMessage command)
		{
			lock (_commands)
			{
				_commands.Enqueue(command);
			}
			_messagesAvaiable.Release();
		}

		public IChatMessage Next()
		{
			int reason = WaitHandle.WaitAny(new WaitHandle[] { _quitEvent, _messagesAvaiable}, 500, true);
			if (reason == WaitHandle.WaitTimeout || reason == 0) return null;

			lock (_commands)
			{
				return _commands.Dequeue();
			}
		}

		private readonly Queue<IChatMessage> _commands = new Queue<IChatMessage>();
		private readonly Semaphore _messagesAvaiable = new Semaphore(0, 10);
		private readonly EventWaitHandle _quitEvent;
	}
}
