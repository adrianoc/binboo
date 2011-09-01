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
using System.Text.RegularExpressions;

namespace Binboo.Core.Commands.Arguments
{
    public class Argument
	{
        public bool IsPresent { get; set; }
		public string Name { private get; set; }
        public string Value { get; set; }
		public Match ArgMatch { private get; set; }

		public static implicit operator string(Argument arg)
		{
			return arg.Value;
		}

		public IEnumerable<string> Values
		{
			get
			{
				if (!IsPresent) yield break;
				
				if (ArgMatch.Groups["param"].Captures.Count == 0)
				{
					yield return ArgMatch.Value;
					yield break;
				}

				foreach (Capture capture in ArgMatch.Groups["param"].Captures)
				{
					yield return capture.Value;
				}
				yield break;
			}
		}

		public override string ToString()
		{
			return Name + ": " + Value;
		}

	}
}