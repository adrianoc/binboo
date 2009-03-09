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
using Binboo.Core.Commands;

namespace Binboo.Tests.Mocks
{
	internal class BotCommandMock : IBotCommand
	{
		public BotCommandMock(string id, Func<Context, string> returnProvider)
		{
			_id = id;
			_returnProvider = returnProvider;
		}

		public string Help
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Id
		{
			get { return _id; }
		}

		public string Process(Context context)
		{
			_called.Set();
			return _returnProvider(context);
		}

		public bool Wait(int timeout)
		{
			return _called.WaitOne(timeout);
		}

		private readonly EventWaitHandle _called = new EventWaitHandle(false, EventResetMode.ManualReset);
		private readonly string _id;
		private readonly Func<Context, string> _returnProvider;
	}
}