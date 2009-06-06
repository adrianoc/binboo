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
	internal class ButNotParamValidator : ParamValidator
	{
		private readonly IEnumerable<ParamValidator> _ignored;
		private readonly ParamValidator _delegate;

		internal ButNotParamValidator(ParamValidator @delegate, params ParamValidator[] ignored) : base(string.Empty)
		{
			_ignored = ignored;
			_delegate = @delegate;
		}

		public override bool IsMatch(string candidate)
		{
			if (!_delegate.IsMatch(candidate)) return false;

			foreach (var validator in _ignored)
			{
				if (validator.IsMatch(candidate)) return false;
			}

			return true;
		}

		public override string RegularExpression
		{
			get { return _delegate.RegularExpression; }
		}

		public override string ToString()
		{
			return _delegate + _ignored.Aggregate(" Ignoring:", (acc, current) => acc + " " + current);
		}
	}
}
