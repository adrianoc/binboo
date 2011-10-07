using System.Xml.Serialization;

namespace Binboo.Language.Configuration
{
	[XmlRoot("translate")]
	public class TranslateCommandConfig
	{
		[XmlAttribute("api-key")]
		public string ApiKey { get; set; }
		
		[XmlAttribute("endpoint")]
		public string EndPoint { get; set; }
	}
}
