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
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Plugins.Tests.Foundation.Mocks;
using Moq;

namespace Binboo.Plugins.Tests.Foundation.Commands
{
    public class CommandMock<TCommand, TMock> : IDisposable where TCommand : IBotCommand where TMock : class
	{
		private readonly Mock<TMock> _mock;
		private readonly TCommand _command;
		private bool _exceptionThrown;

		public CommandMock(TCommand command, Mock<TMock> mock)
		{
			_mock = mock;
			_command = command;
			_command.Storage = new DummyStorage();
		}

		public ICommandResult Process(IContext context)
		{
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) => 
			{
				//_exceptionThrown = args.Exception.GetType() == typeof (AssertionException);
			    _exceptionThrown = true;
			};

		    return _command.Process(context);
		}

		public void Verify()
		{
			_mock.VerifyAll();
		}

		public void Dispose()
		{
			if (!_exceptionThrown)
			{
				_mock.VerifyAll();
			}
		}
	}
}