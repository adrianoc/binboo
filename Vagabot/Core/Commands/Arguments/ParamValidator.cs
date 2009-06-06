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

using System.Collections.Generic;
using System.Linq;

namespace Binboo.Core.Commands.Arguments
{
	internal class ParamValidator
	{
		internal static readonly ParamValidator Project = new ParamValidator("^[A-Za-z]{3,4}");

		internal static readonly ParamValidator Anything = new ParamValidator("\"(?<param>[^\\r\\n\"]+)\"|(?<param>[^\\s\"]+)");
		internal static readonly ParamValidator AnythingStartingWithText = new ParamValidator("\"(?<param>[^\\s0-9\\r\\n\"]+)\"|(?<param>[^\\s0-9][^\\s\"]+)");
		internal static readonly ParamValidator IssueId = new ParamValidator(@"%0%-[0-9]+", Project);
		internal static readonly ParamValidator IssueStatus = new ParamValidator("open|closed|all");
		internal static readonly ParamValidator Iteration = new ParamValidator("^[0-9]+", true);
		internal static readonly ParamValidator MultipleIssueId = new ParamValidator(@"(?<issues>(?:\b(?<param>[A-Za-z]{1,4}-[0-9]{1,4})\s*,?\s*)+\b)");
		internal static readonly ParamValidator Order = new ParamValidator(@"\b0?[1-9]\b", true);
		internal static readonly ParamValidator Peer = Anything.AsOptional();
		internal static readonly ParamValidator Type = new ParamValidator(@"type\s*=\s*(?<param>bug|task|improvement|b|t|i)\z", true);
		internal static readonly ParamValidator UserName = new ParamValidator(@"([A-za-z](?:\s*[A-Za-z]*[0-9]*)*)");

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
			return System.Text.RegularExpressions.Regex.IsMatch(candidate, _regex);
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

		public static ParamValidator From(IEnumerable<string> ids)
		{
			return new ParamValidator(ids.Aggregate("^$", (regex, id) => regex + "|" + id));
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
				_regex = System.Text.RegularExpressions.Regex.Replace(_regex, "%" + i + "%", validators[i].RegularExpression);
			}
		}

		private ParamValidator(string regex) : this(regex, false)
		{
		}

		private readonly string _regex;
		protected readonly bool _optional;
	}
}
