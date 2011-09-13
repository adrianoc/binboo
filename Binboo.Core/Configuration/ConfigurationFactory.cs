namespace Binboo.Core.Configuration
{
	public static class ConfigurationFactory
	{
		public static ICoreConfig Create()
		{
			return new CoreConfig();
		}
	}
}
