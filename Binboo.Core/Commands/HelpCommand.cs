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
using System.Text.RegularExpressions;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Commands.Support;
using Binboo.Core.Plugins;

namespace Binboo.Core.Commands
{
	public class HelpCommand : BotCommandBase
	{
	    public HelpCommand(IPlugin parent) : base(Binboo_Core.Help)
		{
		    _parent = parent;
		}

	    public override string Id
		{
			get { return "Help"; }
		}

	    public override ICommandResult Process(IContext context)
	    {
			IDictionary<string, Argument> arguments = CollectAndValidateArguments(context.Arguments, cmd => ParamValidator.Anything.AsOptional());
			var helpMsg = new StringBuilder("Command        Description" + Environment.NewLine + 
   	                                        "-------        -----------" + Environment.NewLine + Environment.NewLine);

			foreach(IBotCommand command in HelpFor(arguments["cmd"]))
			{
				helpMsg.AppendFormat("{0,-15}{1}{2}{2}", command.Id, AddTabAtNewLine(command.Help), Environment.NewLine);
			}

			return CommandResult.Success(helpMsg.ToString());
		}

		private IEnumerable<IBotCommand> HelpFor(Argument command)
		{
			return command.IsPresent ? FindCommand(command) : _parent.Commands;
		}

        private IEnumerable<IBotCommand> FindCommand(string commandId)
		{
			commandId = commandId.ToLower();
			return _parent.Commands.Where(cmd => cmd.Id.ToLower() == commandId);
		}

		private static string AddTabAtNewLine(string str)
		{
			return Regex.Replace(str, Environment.NewLine, string.Format("{0,-17}", Environment.NewLine));
		}

	    private readonly IPlugin _parent;
    }
}
