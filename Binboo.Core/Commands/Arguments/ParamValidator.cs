﻿/**
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Binboo.Core.Commands.Arguments
{
    public class ParamValidator
	{
        public static readonly ParamValidator Anything = new ParamValidator("\"(?<param>[^\\r\\n\"]+)\"|(?<param>[^\\s\"]+)");
        public static readonly ParamValidator AnythingStartingWithText = new ParamValidator("\"(?<param>[A-Za-z][^\"]+)\"|(?<param>[^0-9\\s\r\n\"]+)");
        public static readonly ParamValidator QuotedString = new ParamValidator("\"(?<param>[^\"]+)\"");

		public static ParamValidator Custom(string regex, bool optional)
		{
			return new ParamValidator(regex, optional);
		}

		public ParamValidator AsOptional()
		{
			return new DelegatingParamValidator(this, true);
		}

		public virtual string RegularExpression
		{
			get { return _regex; }
		}

		public bool Optional
		{
			get { return _optional; }
		}

		public virtual bool IsMatch(string candidate)
		{
			var match = Regex.Match(candidate, _regex);
			return match.Success && match.Value.Trim().Length == candidate.Trim().Length;
		}

		public ParamValidator ButNot(params ParamValidator[] ignored)
		{
			return new ButNotParamValidator(this, ignored);
		}

		public static implicit operator string(ParamValidator validator)
		{
			return validator.RegularExpression;
		}

		public override string ToString()
		{
			return "(" + _regex + "," + (_optional ? "optional" : "required") + ")";
		}

		public static ParamValidator From(IEnumerable<string> names)
		{
			return new ParamValidator(names.Aggregate("^$", (regex, friendlyName) => regex + "|" + RegExpFor(friendlyName)));
		}

		private static string RegExpFor(string name)
		{
			return string.Format("\"{{0,1}}(?<param>{0})\"{{0,1}}", name);
		}

		protected ParamValidator(string regex, bool optional)
		{
			_regex = regex;
			_optional = optional;
		}

		protected ParamValidator(string regex, params ParamValidator[] validators) : this(regex)
		{
			for (int i = 0; i < validators.Length; i++)
			{
				_regex = Regex.Replace(_regex, "%" + i + "%", validators[i].RegularExpression);
			}
		}

		private ParamValidator(string regex) : this(regex, false)
		{
		}

		private readonly string _regex;
		private readonly bool _optional;
	}
}
