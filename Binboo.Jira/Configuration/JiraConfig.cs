/**
 * Copyright (c) 2011 Adriano Carlos Verona
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Binboo.Core;
using Binboo.Core.Configuration;
using TCL.Cryptography;

namespace Binboo.Jira.Configuration
{
	class JiraConfig : IJiraConfig
	{
		private static IJiraConfig _instance = new JiraConfig(ConfigurationFactory.Create());
		
		private readonly ICoreConfig _coreConfig;

		public JiraConfig(ICoreConfig coreConfig)
		{
			_coreConfig = coreConfig;
		}

		public string IMUserToIssueTrackerUser(string name)
		{
			XmlNode userMappingNode = _coreConfig.FindConfigItem("jira", NameMappingXPathFor(name));
			return userMappingNode != null ? userMappingNode.Value : String.Empty;
		}

		public string ResolveUser(string userName, string IMUserName)
		{
			if (userName.ToLower() == "myself")
			{
				userName = IMUserToIssueTrackerUser(IMUserName);
				if (String.IsNullOrEmpty(userName))
				{
					throw new ArgumentException(String.Format("Unable to map skype user name '{0}' to jira user name. Check configuration file.", IMUserName));
				}
			}
			else
			{
				string mappedUser = UserNameForAlias(userName);
				if (!String.IsNullOrEmpty(mappedUser)) return mappedUser;
			}

			return userName;
		}


		public IEnumerable<string> PairingUsers
		{
			get
			{
				foreach (XmlNode userNode in FindPairingUsers())
				{
					yield return IssueTrackerUserNameOrAlias(userNode);
				}
			}
		}

		public string EndPoint
		{
			get
			{
				XmlNode endPointNode = _coreConfig.FindConfigItem("jira", "endpoint/@value");
				if (endPointNode == null)
				{
					throw new ArgumentException("Unable to retrieve endpoint configuration from " + _coreConfig);
				}

				return endPointNode.Value;
			}
		}

		public String Server
		{
			get
			{
				MatchCollection matches = Regex.Matches(EndPoint, "(http://[^/]*|http://.*).*");
				return matches.Count > 0 && matches[0].Groups.Count >= 2
				       	? matches[0].Groups[1].Value
				       	: "[Invalid Jira Server URL] " + EndPoint;
			}
		}

		public TextReader CommandConfigurationFor(string commandName)
		{
			var commandConfigurarion = _coreConfig.CommandConfigurationFor("jira", commandName);
			return new StringReader(commandConfigurarion.OuterXml);
		}

		public User User
		{
			get
			{
				XmlNode user = _coreConfig.FindConfigItem("jira", "user");
				if (user == null)
				{
					throw new ArgumentException("Unable to retrieve user information from " + _coreConfig);
				}

				return new User(
							user.Attributes["name"].Value,
							ToString(Decrypt(Convert.FromBase64String(user.Attributes["password"].Value))));
			}

			set
			{
				XmlNode user = _coreConfig.FindConfigItem("jira", "user");
				user.Attributes["name"].Value = value.Name;
				user.Attributes["password"].Value = Convert.ToBase64String(Encrypt(ToBytes(value.Password)));

				_coreConfig.SaveConfig();
			}
		}

		public IHttpInterfaceConfiguration HttpInterfaceConfiguration
		{
			get
			{
				return new HttpInterfaceConfiguration(operation => _coreConfig.FindConfigItem("jira", string.Format("http/{0}/@url", operation)).Value);
			}
		}


		public static IJiraConfig Instance
		{
			get
			{
				return _instance;
			}
		}

		private static string IssueTrackerUserNameOrAlias(XmlNode userNode)
		{
			XmlNode userNameNode = userNode.Attributes.GetNamedItem(AliasAttributeName) ??
								   userNode.Attributes[UserNameAttributeName];

			return userNameNode.Value;
		}

		private XmlNodeList FindPairingUsers()
		{
			return _coreConfig.FindConfigItems("jira", UserNodeMappingXPath + "[@pair='true']");
		}

		private static string NameMappingXPathFor(string name)
		{
			return String.Format("{0}[@skypeName='{1}']/@{2}", UserNodeMappingXPath, name, UserNameAttributeName);
		}

		private string UserNameForAlias(string userName)
		{
			XmlNodeList users = _coreConfig.FindConfigItems("jira", String.Format("{0}[translate(@{1},'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{2}']", UserNodeMappingXPath, AliasAttributeName, userName.ToLower()));
			if (users.Count == 0) return userName;

			return users[0].Attributes[UserNameAttributeName].Value;
		}

		private static byte[] Decrypt(byte[] data)
		{
			return DPAPI.Decrypt(Store.User, data);
		}

		private static byte[] Encrypt(byte[] data)
		{
			return DPAPI.Encrypt(Store.User, data);
		}

		private static byte[] ToBytes(string value)
		{
			return Encoding.ASCII.GetBytes(value);
		}

		private static string ToString(byte[] data)
		{
			return Encoding.ASCII.GetString(data);
		}

		private const string UserNodeMappingXPath = "im-user-mapping/user";
		private const string UserNameAttributeName = "jiraName";
		private const string AliasAttributeName = "alias";
	}
}
