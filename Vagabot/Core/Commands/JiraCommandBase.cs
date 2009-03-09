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
using System.Text.RegularExpressions;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal abstract class JiraCommandBase : BotCommandBase
	{
		protected JiraCommandBase(IJiraProxy jira, string help) : base(help)
		{
			_jira = jira;
		}

		protected string CheckParameters(string[] args, params ParamValidator[] validators)
		{
			int requiredParamCount = RequiredCount(validators);
			if (args.Length < requiredParamCount || args.Length > validators.Length)
			{
				return string.Format("{0}: Invalid arguments count. Expected at least {1} and no more than {2}, got {3}{4}{4}{5}", 
				                     Id, 
				                     requiredParamCount, 
				                     validators.Length, 
				                     args.Length,
									 Environment.NewLine,
									 Help);
			}


			string ret = ValidateRequiredArguments(validators, args, requiredParamCount);
			if (ret != null) return ret;

			return ValidateOptionalArguments(args, validators, requiredParamCount);
		}

		private static string ValidateOptionalArguments(string[] args, IEnumerable<ParamValidator> validators, int requiredParamCount)
		{
			for (int i = requiredParamCount; i < args.Length; i++)
			{
				if (!validators.Skip(requiredParamCount).Any(validator => Regex.IsMatch(args[i], validator.Regex)))
				{
					return string.Format("Invalid optional argument '{0}' (Position: {1})", args[i], i);
				}
			}

			return null;
		}

		private string ValidateRequiredArguments(ParamValidator[] validators, string[] args, int requiredParamLength)
		{
			for (int i = 0; i < requiredParamLength; i++)
			{
				if (!Regex.IsMatch(args[i], validators[i].Regex))
				{
					return string.Format("{0}: Argument index {1} (vale = '{2}') is invalid (Validator: {3}).", Id, i, args[i], validators[i]);
				}
			}

			return null;
		}

		protected static string OptionalParameterOrDefault(IEnumerable<string> args, int startIndex, ParamValidator validator, string defaultValue)
		{
			Match m = OptionalParameterOrNull(args, startIndex, validator);
			return m == null ? defaultValue : m.Value;
		}

		protected static Match OptionalParameterOrNull(IEnumerable<String> args, int startIndex, ParamValidator validator)
		{
			return OptionalParameterOrNull(args.Skip(startIndex), validator);
		}

		/**
		 * 
		 */
		protected static Match OptionalParameterOrNull(IEnumerable<String> optionalArgs, ParamValidator validator)
		{
			foreach (string arg in optionalArgs)
			{
				if (validator.IsMatch(arg))
				{
					return Regex.Match(arg, validator.Regex);
				}
			}

			return null;
		}

		protected static int NonOptionalCount(IEnumerable<ParamValidator> validators)
		{
			return validators.Count(p => !p.Optional);
		}

		protected static string Run(Func<string> command)
		{
			try
			{
				return command();
			}
			catch (JiraProxyException jipe)
			{
				return jipe.Message + Environment.NewLine + jipe.InnerException.Message;
			}
		}

		protected static string Run<T>(Func<T> command, Func<T, string> messageMapping)
		{
			try
			{
				return messageMapping(command());
			}
			catch (JiraProxyException jipe)
			{
				return jipe.Message + Environment.NewLine + jipe.InnerException.Message;
			}
		}

		protected static string Run(Action command, string message)
		{
			try
			{
				command();
				return message;
			}
			catch (JiraProxyException jipe)
			{
				return jipe.Message + Environment.NewLine + jipe.InnerException.Message;
			}
		}

		private static int RequiredCount(IEnumerable<ParamValidator> validators)
		{
			return validators.Count(pv => !pv.Optional);
		}

		protected static string UrlFor(RemoteIssue issue)
		{
			return String.Format("{0}/browse/{1}", ConfigServices.Server, issue.key);
		}

		protected IJiraProxy _jira;
	}
}