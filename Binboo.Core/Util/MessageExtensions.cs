/**
 * Copyright (c) 2012 Adriano Carlos Verona
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
using System.Text.RegularExpressions;

namespace Binboo.Core.Util
{
    public static class MessageExtensions
    {
        public static string PluginName(this string message)
        {
            var pluginMatch = Regex.Match(message, @"\$(?<pluginName>[a-zA-Z]+)\s*");
            if (!pluginMatch.Success)
            {
                throw new ArgumentException(string.Format("Missing plugin name in message '{0}'", message), "message");
            }

            return pluginMatch.Groups["pluginName"].Value;
        }

        public static string StripPluginName(this string message)
        {
            return Regex.Replace(message, PluginNameRegExp, "");
        }

        public static bool IsPluginCommand(this string message)
        {
            var pluginMatch = Regex.Match(message, PluginNameRegExp);
            return pluginMatch.Success;
        }

        private const string PluginNameRegExp = @"\$(?<pluginName>[A-Za-z0-9]+)\s*";

    }
}
