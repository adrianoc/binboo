using System.IO;

namespace TCL.Net.Extensions
{
	public static class StreamExtensions
	{
		public static bool Contains(this Stream source, string value)
		{
			return new StreamReader(source).ReadToEnd().Contains(value);
		}
	}
}
