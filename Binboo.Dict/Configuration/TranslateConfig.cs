using Binboo.Core.Configuration;

namespace Binboo.Dict.Configuration
{
	class TranslateConfig
	{
		public static TranslateConfig Instance = new TranslateConfig(ConfigurationFactory.Create());
		
		private readonly ICoreConfig _coreConfig;

		public TranslateConfig(ICoreConfig coreConfig)
		{
			_coreConfig = coreConfig;
		}
		
		public string APIKey
		{
			get
			{
				return _coreConfig.FindConfigItem("translate", "api-key/@value").Value;
			}
		}
	}
}
