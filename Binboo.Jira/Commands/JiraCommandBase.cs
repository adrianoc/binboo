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
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Support;
using Binboo.Core.Configuration;
using Binboo.Core.Exceptions;
using Binboo.Jira.Integration;

namespace Binboo.Jira.Commands
{
	internal abstract class JiraCommandBase : BotCommandBase
	{
		protected JiraCommandBase(IJiraProxy jira, string help) : base(help)
		{
			_jira = jira;
		}

		public override sealed ICommandResult Process(IContext context)
		{
			try
			{
				return ProcessCommand(context);
			}
			catch (InvalidCommandArgumentsException icae)
			{
				return CommandResult.Exception(icae);
			}
		}

		protected abstract ICommandResult ProcessCommand(IContext context);

		protected static string Run(Func<string> command)
		{
		    return Run<JiraProxyException>(command);
		}

		protected static ICommandResult Run<TResult>(Func<TResult> command, Func<TResult, ICommandResult> messageMapping)
		{
            return Run<TResult, JiraProxyException>(command, messageMapping);
        }

		protected static string Run<T>(Func<T> command, Func<T, string> messageMapping)
		{
		    return Run<T, JiraProxyException>(command, messageMapping);
		}

		protected static string Run(Action command, string message)
		{
            return Run<JiraProxyException>(command, message);
        }

		protected static string UrlFor(RemoteIssue issue)
		{
			return String.Format("{0}/browse/{1}", ConfigServices.Server, issue.key);
		}

		internal IJiraProxy Proxy
		{
			set
			{
				_jira = value;
			}
		}

		protected IJiraProxy _jira;

		protected static string CommaSeparated(IEnumerable<string> list)
		{
			return list.Aggregate("", (acc, current) => acc + ((acc.Length > 0) ? ", " : "") + current);
		}
	}
}