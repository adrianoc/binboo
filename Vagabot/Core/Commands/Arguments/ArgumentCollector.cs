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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Binboo.Core.Exceptions;
using Binboo.Core.Util;

namespace Binboo.Core.Commands.Arguments
{
	internal class ArgumentCollector
	{
		private readonly IList<ParamValidator> _validators;
		private readonly IBotCommand _command;
		private readonly Expression<Func<int, ParamValidator>>[] _validatorExpressions;

		internal static ArgumentCollector For(Expression<Func<int, ParamValidator>>[] validatorExpressions, IBotCommand command)
		{
			return new ArgumentCollector(validatorExpressions, command);
		}

		private ArgumentCollector(Expression<Func<int, ParamValidator>>[] validatorExpressions, IBotCommand command)
		{
			_validators = ValidatorsFrom(validatorExpressions);
			_validatorExpressions = validatorExpressions;
			_command = command;
			
			ValidateValidatorsOrder();
		}

		internal IDictionary<string, Argument> Collect(string arguments)
		{
			var argumentMatches = MatchesFor(arguments);

			ValidateArgumentCount(argumentMatches);

			IDictionary<string, Argument> args = DefaultArgumentsFor(_validatorExpressions);
			for (int i=0, j = 0; i < _validators.Count && j < argumentMatches.Count; i++)
			{
				EnsureRequiredParameterIsPresent(i, argumentMatches[j].Value);
				if (_validators[i].IsMatch(argumentMatches[j].Value))
				{
					Match argMatch = Regex.Match(argumentMatches[j].Value, _validators[i].RegularExpression);
					SetArgumentValue(args, ArgumentNameFor(_validatorExpressions[i]), true, argMatch.Value(), argMatch);
					j++;
				}
			}

			return args;
		}

		private void ValidateValidatorsOrder()
		{
			IEnumerable<ParamValidator> optionalOrEmpty = _validators.SkipWhile(p => !p.Optional);
			if (optionalOrEmpty.Any(p => !p.Optional))
			{
				throw new ArgumentException("Mixed validators (optional/non-optional) are not supported. Validators should follow the order: no, no, ..., op, op, ... op");
			}
		}

		private MatchCollection MatchesFor(string arguments)
		{
			return Regex.Matches(arguments, ExtractingRegularExpressionFor(_validators));
		}

		private void EnsureRequiredParameterIsPresent(int index, string value)
		{
			if (IsRequiredParameterMissing(_validators[index], value))
			{
				throw new InvalidCommandArgumentsException(string.Format("{0}: Argument index {1} (vale = '{2}') is invalid (Validator: {3}).", _command.Id, index, value, _validators[index]));
			}
		}

		private static bool IsRequiredParameterMissing(ParamValidator validator, string value)
		{
			return !IsPresent(validator, value) && !validator.Optional;
		}

		private static IDictionary<string, Argument> DefaultArgumentsFor(IEnumerable<Expression<Func<int, ParamValidator>>> validatorExpressions)
		{
			IDictionary<string, Argument> arguments = new Dictionary<string, Argument>();
			foreach (var expression in validatorExpressions)
			{
				SetArgumentValue(arguments, ArgumentNameFor(expression), false, string.Empty, null);
			}

			return arguments;
		}

		private static void SetArgumentValue(IDictionary<string, Argument> arguments, string name, bool isPresent, string value, Match match)
		{
			arguments[name] = new Argument { Name = name, IsPresent = isPresent, Value = value, ArgMatch = match };
		}

		private static string ExtractingRegularExpressionFor(IEnumerable<ParamValidator> validators)
		{
			string regularExpression = validators.Aggregate("(?x-imsn:^$", (regex, validator) => regex + "|" + validator.RegularExpression) + ")";
			DumpRegularExpression(regularExpression);
			return regularExpression;
		}

		[Conditional("DEBUG")]
		private static void DumpRegularExpression(string expression)
		{
			Debug.WriteLine("Command Regular Expression :" + expression);
		}

		private void ValidateArgumentCount(ICollection matches)
		{
			int requiredCount = RequiredCount();
			if (matches.Count < requiredCount || matches.Count > _validators.Count)
			{
				throw new InvalidCommandArgumentsException(
					string.Format("{0}: Invalid arguments count. Expected at least {1} and no more than {2}, got {3}{4}{4}{5}{4}{4}{6}",
								  _command.Id,
								  requiredCount,
								  _validators.Count,
								  matches.Count,
								  Environment.NewLine,
								  MatchesToString(matches),
								  _command.Help));
			}
		}

		private string MatchesToString(ICollection matches)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Match match in matches)
			{
				sb.AppendFormat("{0}: {1}{2}", match.Index, match.Value, Environment.NewLine);
			}

			return sb.ToString();
		}

		private static bool IsPresent(ParamValidator validator, string value)
		{
			return validator.IsMatch(value);
		}

		private static IList<ParamValidator> ValidatorsFrom(IEnumerable<Expression<Func<int, ParamValidator>>> validatorExpressions)
		{
			IList<ParamValidator> validators = new List<ParamValidator>();
			foreach (var validatorExpression in validatorExpressions)
			{
				validators.Add(validatorExpression.Compile().Invoke(0));
			}
			return validators;
		}

		private static string ArgumentNameFor(Expression<Func<int, ParamValidator>> validatorExpression)
		{
			return validatorExpression.Parameters[0].Name;
		}

		private int RequiredCount()
		{
			return _validators.Count(pv => !pv.Optional);
		}
	}
}
