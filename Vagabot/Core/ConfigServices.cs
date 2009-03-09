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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Binboo.Core.Commands;
using TCL.Cryptography;

namespace Binboo.Core
{
	class ConfigServices
	{
		public static string MatchIssueTrackerUser(string partOfName)
		{
			EnsureConfigIsLoaded();

			XmlNodeList users = FindConfigItems(UserNodeMappingXPath + "/@jiraName");
			if (users == null || users.Count == 0) return String.Empty;

			foreach (XmlNode user in users)
			{
				if (Regex.IsMatch(user.Value, partOfName, RegexOptions.IgnoreCase))
				{
					return user.Value;
				}
			}

			return String.Empty;
		}

		public static string IMUserToIssueTrackerUser(string name)
		{
			EnsureConfigIsLoaded();

			XmlNode userMappingNode = FindConfigItem(NameMappingXPathFor(name));
			return userMappingNode != null ? userMappingNode.Value : String.Empty;
		}
		
		public static string EndPoint
		{
			get
			{
				XmlNode endPointNode = FindConfigItem("//binboo/jira/endpoint/@value");
				if (endPointNode == null)
				{
					throw new ArgumentException("Unable to retrieve endpoint configuration from " + ConfigFilePath());	
				}
				
				return endPointNode.Value;
			}
		}

		public static JiraUser User
		{
			get
			{
				XmlNode user = FindConfigItem("//binboo/jira/user");
				if (user == null)
				{
					throw new ArgumentException("Unable to retrieve user information from " + ConfigFilePath());
				}

				return new JiraUser(
							user.Attributes["name"].Value,
							ToString(Decrypt(Convert.FromBase64String(user.Attributes["password"].Value))));		
			}

			set
			{
				XmlNode user = FindConfigItem("//binboo/jira/user");
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

		private static void SaveConfig()
		{
			_config.Save(ConfigFilePath());
		}

		private static XmlNode FindConfigItem(string xpath)
		{
			EnsureConfigIsLoaded();
			return _config.SelectSingleNode(InjectActiveConfigId(xpath));
		}

		private static XmlNodeList FindConfigItems(string xpath)
		{
			EnsureConfigIsLoaded();
			return _config.SelectNodes(InjectActiveConfigId(xpath));
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

				FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigFilePath()), Path.GetFileName(ConfigFilePath()));
				FileSystemEventHandler configChangedHandler = null;

				configChangedHandler = delegate 
										{
											watcher.Changed -= configChangedHandler;
											
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
			return String.Format("{0}[@skypeName='{1}']/@jiraName", UserNodeMappingXPath, name);
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

		public static string ResolveUser(string userName, Context context)
		{
			if (userName.ToLower() == "myself")
			{
				userName = IMUserToIssueTrackerUser(context.UserName);
				if (String.IsNullOrEmpty(userName))
				{
					throw new ArgumentException(String.Format("Unable to map skype user name '{0}' to jira user name. Check configuration file.", context.UserName));
				}
			}
			else
			{
				string mappedUser = MatchIssueTrackerUser(userName);
				if (!String.IsNullOrEmpty(mappedUser)) return mappedUser;
			}
						
			return userName;
		}

		private static XmlDocument _config;
		private const string UserNodeMappingXPath = "//binboo/jira/im-user-mapping/user";
	}
}
