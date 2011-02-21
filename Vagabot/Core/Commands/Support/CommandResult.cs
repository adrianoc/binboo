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

namespace Binboo.Core.Commands.Support
{
	class CommandResult : ICommandResult
	{
		public CommandStatus Status
		{
			get; private set;
		}

		public string HumanReadable
		{
			get { return _humanReadable;  }
		}

		public string PipeValue
		{
			get { return string.Join(", ", _pipeValue); }
		}

		public T PipeThrough<T>(string args, Func<string, T> func)
		{
			return func(PipeValue + " " + args);
		}

		public static CommandResult None
		{
			get { return new CommandResult(string.Empty); }
		}

		internal static ICommandResult Exception(Exception ex)
		{
			var message = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
			return new CommandResult(message) { Status = CommandStatus.Exception };
		}

		internal static ICommandResult Fail(string message)
		{
			return new CommandResult(message) { Status = CommandStatus.Error };
		}

		internal static ICommandResult Success(string message, IEnumerable<string> pipeValues)
		{
			return Success(message, pipeValues.ToArray());
		}

		internal static ICommandResult Success(string message, params string[] pipeValues)
		{
			return new CommandResult(message, pipeValues) { Status = CommandStatus.Success };
		}

		private CommandResult(string message, params string[] pipeValues)
		{
			_humanReadable = message;
			_pipeValue = pipeValues;
		}

		private readonly string _humanReadable;
		private readonly string[] _pipeValue;
	}
}
