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
using System.Text;
using System.Text.RegularExpressions;

namespace Binboo.Core.Commands
{
    public class CommandResult : ICommandResult
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

		public T PipeThrough<T>(string originalArgs, Func<string, T> func) where T: class
		{
			return TryPipeThroughIndexed(originalArgs, func)
			       ?? PipeAll(originalArgs, func);


		}

    	private T PipeAll<T>(string originalArgs, Func<string, T> func)
    	{
			var pipeValue = PipeValue;
			return func(string.IsNullOrEmpty(pipeValue) ? originalArgs : pipeValue + " " + originalArgs);
    	}

    	private T TryPipeThroughIndexed<T>(string originalArgs, Func<string, T> func)
    	{
    		var toBeReplaced = ArgumentIndexRegexp.Matches(originalArgs);
    		return toBeReplaced.Count > 0 
						? PipeThroughIndexed(originalArgs, toBeReplaced, func) 
						: default(T);
    	}

    	private T PipeThroughIndexed<T>(string originalArgs, MatchCollection toBeReplaced, Func<string, T> func)
		{
			var pipeThroug = new StringBuilder(originalArgs);
				
			int amountOfseted = 0;
			foreach (Match tbr in toBeReplaced)
			{
				amountOfseted += TryPipeExplicityIndexedArg(amountOfseted, tbr, pipeThroug);
				amountOfseted += TryPipeExplicityArgExpansion(amountOfseted, tbr, pipeThroug);
			}

			return func(pipeThroug.ToString());
		}

    	private int TryPipeExplicityArgExpansion(int amountOfseted, Match tbr, StringBuilder pipeThroug)
    	{
    		var group = tbr.Groups["expand"];
			if (!group.Success) return 0;

    		var expanded = string.Join(@group.Value, _pipeValue);
			return ReplaceArg(amountOfseted, tbr, pipeThroug, expanded);
    	}

    	private int TryPipeExplicityIndexedArg(int amountOfseted, Match tbr, StringBuilder pipeThroug)
    	{
    		var group = tbr.Groups["index"];
			if (!group.Success) return 0;

    		string newValue = PipedValueFor(@group.Value);
			return ReplaceArg(amountOfseted, tbr, pipeThroug, newValue);

    		pipeThroug.Replace(tbr.Value, newValue, tbr.Index + amountOfseted, tbr.Length);
    		return newValue.Length - tbr.Value.Length;
    	}

		private static int ReplaceArg(int amountOfseted, Match tbr, StringBuilder pipeThroug, string expanded)
		{
			pipeThroug.Replace(tbr.Value, expanded, tbr.Index + amountOfseted, tbr.Length);
			return expanded.Length - tbr.Value.Length;
		}

		private string PipedValueFor(string index)
		{
			return _pipeValue[Int32.Parse(index)];
		}

		public static CommandResult None
		{
			get { return new CommandResult(string.Empty); }
		}

        public static ICommandResult Exception(Exception ex)
		{
			var message = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : "");
			return new CommandResult(message) { Status = CommandStatus.Exception };
		}

        public static ICommandResult Fail(string message)
		{
			return new CommandResult(message) { Status = CommandStatus.Error };
		}

		public static ICommandResult Success(string message, IEnumerable<string> pipeValues)
		{
			return Success(message, pipeValues.ToArray());
		}

		public static ICommandResult Success(string message, params string[] pipeValues)
		{
			return new CommandResult(message, pipeValues) { Status = CommandStatus.Success };
		}

		private CommandResult(string message, params string[] pipeValues)
		{
			_humanReadable = message;
			_pipeValue = pipeValues;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}, '{2}', '{3}']", GetType().Name, Status, HumanReadable, PipeValue);
		}

		private static readonly Regex ArgumentIndexRegexp = new Regex(@"%(?<index>[0-9]+)%|%\[(?<expand>[^\]]+)\]%");
		private readonly string _humanReadable;
		private readonly string[] _pipeValue;
	}
}
