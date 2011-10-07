using System.Xml;

namespace Binboo.Core.Configuration
{
	public interface ICoreConfig
	{
		string StoragePath { get; }
		XmlNode FindConfigItem(string plugin, string xpath);
		XmlNodeList FindConfigItems(string plugin, string xpath);
		XmlNode CommandConfigurationFor(string pluginName, string commandName);
		void SaveConfig();
	}
}
