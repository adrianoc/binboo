namespace TCL.Extensions
{
	public static class StringExtensions
	{
		public static byte[] ToBytes(this string value)
		{
			return System.Text.Encoding.UTF8.GetBytes(value);
		}
	}
}
