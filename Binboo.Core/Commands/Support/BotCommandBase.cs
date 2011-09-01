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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Persistence;

namespace Binboo.Core.Commands.Support
{
    public abstract class BotCommandBase : IBotCommand
	{
		public abstract string Id { get; }

		public abstract ICommandResult Process(IContext context);

		protected BotCommandBase(string help)
		{
			_help = ReplaceCommand(help, Id);
		}

		virtual public void Initialize()
		{
			// give subcasses a chance to take any action upon initialization.
		}

		public IStorage Storage
		{
			set 
			{
				_storage = value;
			}

			protected get
			{
				return _storage;
			}
		}

		public virtual string Help
		{
			get { return _help; }
		}

        protected IDictionary<string, Argument> CollectAndValidateArguments(string argumentLine, params Expression<Func<int, ParamValidator>>[] validatorExpressions)
        {
            return ArgumentCollector.For(validatorExpressions, this).Collect(argumentLine);
        }

		private static string ReplaceCommand(string contents, string command)
		{
			return Regex.Replace(contents, "%cmd%", command);
		}

		private readonly string _help;
		private IStorage _storage;
	}
}