/**
 * Copyright (c) 2009 Adriano Carlos Verona
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
using TCL.Cryptography;

namespace Binboo.Core.Configuration
{
    public static class ConfigServices
	{
		public static TextReader CommandConfigurationFor(string commandName)
		{
			EnsureConfigIsLoaded();

			XmlNode commandConfigurarion = FindConfigItem(string.Format("commands/{0}", commandName.ToLowerInvariant()));
			return new StringReader(commandConfigurarion.OuterXml);
		}

		public static string IMUserToIssueTrackerUser(string name)
		{
			EnsureConfigIsLoaded();

			XmlNode userMappingNode = FindConfigItem(NameMappingXPathFor(name));
			return userMappingNode != null ? userMappingNode.Value : String.Empty;
		}

		public static string StoragePath
		{
			get
			{
				XmlNode storagePathNode = FindConfigItem("storage/@path");
				if (storagePathNode == null)
				{
					throw new ArgumentException("Unable to retrieve storage path from configuration file: " + ConfigFilePath());
				}

				return Regex.Replace(storagePathNode.Value, "%(.*)%", match => Environment.GetEnvironmentVariable(match.Groups[1].Captures[0].Value));
			}
		}

		public static string EndPoint
		{
			get
			{
				XmlNode endPointNode = FindConfigItem("endpoint/@value");
				if (endPointNode == null)
				{
					throw new ArgumentException("Unable to retrieve endpoint configuration from " + ConfigFilePath());	
				}
				
				return endPointNode.Value;
			}
		}

		public static User User
		{
			get
			{
				XmlNode user = FindConfigItem("user");
				if (user == null)
				{
					throw new ArgumentException("Unable to retrieve user information from " + ConfigFilePath());
				}

				return new User(
							user.Attributes["name"].Value,
							ToString(Decrypt(Convert.FromBase64String(user.Attributes["password"].Value))));		
			}

			set
			{
				XmlNode user = FindConfigItem("user");
				user.Attributes["name"].Value = value.Name;
				user.Attributes["password"].Value = Convert.ToBase64String(Encrypt(ToBytes(value.Password)));

				SaveConfig();
			}
		}

		public static String Server
		{
			get
			{
				MatchCollection matches = Regex.Matches(EndPoint, "(http://[^/]*|http://.*).*");
				return matches.Count > 0 && matches[0].Groups.Count >= 2
				       	? matches[0].Groups[1].Value
				       	: "[Invalid Jira Server URL] " + EndPoint;
			}
		}

		public static string ResolveUser(string userName, string IMUserName)
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

		public static IEnumerable<string> PairingUsers
		{
			get
			{
				foreach (XmlNode userNode in FindPairingUsers())
				{
					yield return IssueTrackerUserNameOrAlias(userNode);
				}
			}
		}

		public static IHttpInterfaceConfiguration HttpInterfaceConfiguration
		{
			get
			{
				return new HttpInterfaceConfiguration(operation => FindConfigItem(string.Format("http/{0}/@url", operation)).Value);
			}
		}

		private static string IssueTrackerUserNameOrAlias(XmlNode userNode)
		{
			XmlNode userNameNode = userNode.Attributes.GetNamedItem(AliasAttributeName) ??
			                       userNode.Attributes[UserNameAttributeName];

			return userNameNode.Value;
		}

		private static XmlNodeList FindPairingUsers()
		{
			return FindConfigItems(UserNodeMappingXPath + "[@pair='true']");
		}

		private static string UserNameForAlias(string userName)
		{
			EnsureConfigIsLoaded();

			XmlNodeList users = FindConfigItems(string.Format("{0}[translate(@{1},'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{2}']", UserNodeMappingXPath, AliasAttributeName, userName.ToLower()));
			if (users.Count == 0) return userName;

			return users[0].Attributes[UserNameAttributeName].Value;
		}

		private static void SaveConfig()
		{
			_config.Save(ConfigFilePath());
		}

		private static XmlNode FindConfigItem(string xpath)
		{
			EnsureConfigIsLoaded();
			return _config.SelectSingleNode(InjectActiveConfigId(AppendBaseConfigPath(xpath)));
		}

		private static string AppendBaseConfigPath(string xpath)
		{
			return ConfigPathPrefix + "/" + xpath;
		}

		private static XmlNodeList FindConfigItems(string xpath)
		{
			EnsureConfigIsLoaded();
			return _config.SelectNodes(InjectActiveConfigId(AppendBaseConfigPath(xpath)));
		}

		private static string InjectActiveConfigId(string xpath)
		{
			int index = xpath.IndexOf("jira");
			return xpath.Insert(xpath.IndexOf("/", index), String.Format("[@id='{0}']", GetActiveConfigId()));
		}

		private static string GetActiveConfigId()
		{
			XmlNode activeConfigIdNode = _config.SelectSingleNode("//binboo/active/@refid");
			if (activeConfigIdNode == null)
			{
				throw new Exception("Invalid configuration file. <Active id='ID' /> node not found.");
			}

			return activeConfigIdNode.Value;
		}

		private static void EnsureConfigIsLoaded()
		{
			if (_config == null)
			{
				_config = new XmlDocument();
				_config.Load(ConfigFilePath());
				
				var watcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigFilePath()), Path.GetFileName(ConfigFilePath()));
				FileSystemEventHandler configChangedHandler = delegate
				{
					watcher = null;
					_config = null;
				};

				watcher.Changed += configChangedHandler;
				watcher.NotifyFilter = NotifyFilters.LastWrite;
				watcher.EnableRaisingEvents = true;
			}
		}

		private static string ConfigFilePath()
		{
			return MakeAbsolutePath(Environment.CurrentDirectory, "Binboo.config.xml");
		}

		private static string MakeAbsolutePath(string directory, string path)
		{
			return Path.Combine(directory, path);
		}

		private static string NameMappingXPathFor(string name)
		{
			return String.Format("{0}[@skypeName='{1}']/@{2}", UserNodeMappingXPath, name, UserNameAttributeName);
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

		private static XmlDocument _config;
		private const string UserNameAttributeName = "jiraName";
		private const string AliasAttributeName= "alias";
		private const string UserNodeMappingXPath = "im-user-mapping/user";
		private const string ConfigPathPrefix = "//binboo/jira";
	}
}
