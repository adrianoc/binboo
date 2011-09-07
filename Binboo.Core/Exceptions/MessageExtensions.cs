using System;
using System.Text.RegularExpressions;

namespace Binboo.Core.Exceptions
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
