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
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Commands.Support;

namespace Binboo.Tests.Core.Support
{
	class ArgumentRecorder
	{
		public static ArgumentRecorder NewInstance()
		{
			return new ArgumentRecorder();
		}
		
		public Func<IContext, IDictionary<string, Argument>, ICommandResult> HandlerFor(string command)
		{
			return (context, args) => 
			{
				_arguments[command] = args;
			    return CommandResult.Success(command, args.Values.Where(arg => arg.IsPresent).SelectMany(arg => arg.Values)); 
			};
		}

		public IEnumerable<string> Arguments(string command)
		{
			if (!_arguments.ContainsKey(command))
			{
				throw new ArgumentException(string.Format("No arguments found for command {0}", command), "command");
			}

			return from arg in _arguments[command].Values
			       where arg.IsPresent
			       from value in arg.Values
			       select value;
		}

		private ArgumentRecorder()
		{
		}

		private IDictionary<string, IDictionary<string, Argument>> _arguments = new Dictionary<string, IDictionary<string, Argument>>();
	}
}
