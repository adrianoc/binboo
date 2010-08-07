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

// Parts of the code adapted from http://www.codeproject.com/KB/recipes/soundex.aspx
using System;
using System.Text;

namespace TCL.Extensions
{
	public static class StringExtensions
	{
		public static byte[] ToBytes(this string value)
		{
			return Encoding.UTF8.GetBytes(value);
		}

		public static string SoundEx(this string s)
		{
			var output = new StringBuilder();
			if (s.Length > 0)
			{
				output.Append(Char.ToUpper(s[0]));
				for (int i = 1; i < s.Length && output.Length < 4; i++)
				{
					string c = EncodeChar(s[i]);

					switch (Char.ToLower(s[i - 1]))
					{
						case 'a':
						case 'e':
						case 'i':
						case 'o':
						case 'u':
							output.Append(c);
							break;

						case 'h':
						case 'w':
						default:
							if (output.Length == 1)
							{
								if (EncodeChar(output[output.Length - 1]) != c)
								{
									output.Append(c);
								}
							}
							else
							{
								if (output[output.Length - 1].ToString() != c)
								{
									output.Append(c);
								}
							}
							break;
					}
				}

			}

			return output.ToString().PadRight(4, '0');
		}

		private static string EncodeChar(char c)
		{
			switch (Char.ToLower(c))
			{
				case 'b':
				case 'f':
				case 'p':
				case 'v':
					return "1";
				case 'c':
				case 'g':
				case 'j':
				case 'k':
				case 'q':
				case 's':
				case 'x':
				case 'z':
					return "2";
				case 'd':
				case 't':
					return "3";
				case 'l':
					return "4";
				case 'm':
				case 'n':
					return "5";
				case 'r':
					return "6";
				default:
					return string.Empty;
			}
		}
	}
}
