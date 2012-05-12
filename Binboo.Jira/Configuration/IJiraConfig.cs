using System.Collections.Generic;
using System.IO;
using Binboo.Core;

namespace Binboo.Jira.Configuration
{
	interface IJiraConfig
	{
		string IMUserToIssueTrackerUser(string name);
		string ResolveUser(string userName, string IMUserName);
		IEnumerable<string> PairingUsers { get; }
		string EndPoint { get; }
		string Server { get; }
		User User { get; set; }
		TextReader CommandConfigurationFor(string commandName);
		IHttpInterfaceConfiguration HttpInterfaceConfiguration { get; }
	}
}
