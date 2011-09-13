using System;

namespace Binboo.Jira.Configuration
{
	class HttpInterfaceConfiguration : IHttpInterfaceConfiguration
	{
		public HttpInterfaceConfiguration(Func<string, string> operationExtractor)
		{
			_operationExtractor = operationExtractor;
		}

		public string LinkUrl
		{
			get { return _operationExtractor("link"); }
		}

		public string LoginUrl
		{
			get { return _operationExtractor("login"); }
		}
		
		private readonly Func<string, string> _operationExtractor;
	}
}
