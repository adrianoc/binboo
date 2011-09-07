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
using Binboo.Core.Commands.Support;
using Binboo.Jira.Integration;
using Binboo.Jira.Tests.Mocks;
using Moq;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	internal class CommandMock<T> : IDisposable where T : IBotCommand
	{
		private readonly Mock<IJiraProxy> _mock;
		private readonly T _command;
		private bool _exceptionThrown;

		public CommandMock(T command, Mock<IJiraProxy> mock)
		{
			_mock = mock;
			_command = command;
			_command.Storage = new DummyStorage();
		}

		public ICommandResult Process(IContext context)
		{
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) => 
			{
				_exceptionThrown = args.Exception.GetType() == typeof (AssertionException);
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