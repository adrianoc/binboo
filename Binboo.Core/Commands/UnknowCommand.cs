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
using System.Linq;
using System.Text;
using Binboo.Core.Persistence;
using TCL.Extensions;

namespace Binboo.Core.Commands
{
	class UnknowCommand : IBotCommand
	{
		public UnknowCommand(string commandName, ISet<IBotCommand> commands)
		{
			_commandName = commandName;
			_commands = commands;
		}

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		public IStorage Storage
		{
			set { throw new NotImplementedException(); }
		}

		public string Help
		{
			get { throw new NotImplementedException(); }
		}

		public string Id
		{
			get { throw new NotImplementedException(); }
		}

		public ICommandResult Process(IContext context)
		{
			return CommandResult.Fail(ReportCommandNotFound());
		}

		private string ReportCommandNotFound()
		{
			StringBuilder output = new StringBuilder();
			output.AppendFormat("Unknown command: {0}.", _commandName);

			var inputSoundex = _commandName.SoundEx();

			var probableCommand = _commands.Where(candidate => candidate.Id.SoundEx() == inputSoundex).Select(pair => pair.Id).SingleOrDefault();
			if (probableCommand != null)
			{
				output.AppendFormat("\r\nDid you mean \"{0}\" ?", probableCommand);
			}

			return output.ToString();
		}

		private readonly string _commandName;
		private readonly ISet<IBotCommand> _commands;
	}
}
