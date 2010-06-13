/**
 * Copyright (c) 2010 Adriano Carlos Verona
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
using System.Text;

namespace TCL.Net
{
	public class HttpVariables : IEnumerable<string>
	{
		public string this[string name]
		{
			get
			{
				if (_variables.ContainsKey(name)) return _variables[name];
				return string.Empty;
			}

			set
			{
				_variables[name] = value;
			}
		}

		public override string ToString()
		{
			var output = new StringBuilder();
			foreach (var pair in _variables)
			{
				output.AppendFormat("{0}={1}&", pair.Key, Uri.EscapeDataString(pair.Value));
			}

			return output.Remove(output.Length - 1, 1).ToString();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return _variables.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private readonly IDictionary<string, string> _variables = new Dictionary<string, string>();
	}
}
