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
using System.Text.RegularExpressions;
using System.Xml;

namespace Binboo.Core.Configuration
{
    public class CoreConfig : ICoreConfig
	{
		public string StoragePath
		{
			get
			{
				var storagePathNode = FindConfigItem("core", "storage/@path");
				if (storagePathNode == null)
				{
					throw new ArgumentException("Unable to retrieve storage path from configuration file: " + ConfigFilePath());
				}

				return Regex.Replace(storagePathNode.Value, "%(.*)%", match => Environment.GetEnvironmentVariable(match.Groups[1].Captures[0].Value));
			}
		}

		public XmlNode FindConfigItem(string plugin, string xpath)
		{
			EnsureConfigIsLoaded();
			return _config.SelectSingleNode(InjectActiveConfigId(AppendBaseConfigPath(plugin, xpath)));
		}

		public XmlNodeList FindConfigItems(string plugin, string xpath)
		{
			EnsureConfigIsLoaded();
			return _config.SelectNodes(InjectActiveConfigId(AppendBaseConfigPath(plugin, xpath)));

		}

    	public void SaveConfig()
		{
			_config.Save(ConfigFilePath());
		}

		private static string AppendBaseConfigPath(string plugin, string xpath)
		{
			return ConfigPathPrefix + "/" + plugin + "/" + xpath;
		}

		private static string InjectActiveConfigId(string xpath)
		{
			int index = ConfigPathPrefix.Length;
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

		public override string ToString()
		{
			return ConfigFilePath();
		}

		private static XmlDocument _config;
		private const string ConfigPathPrefix = "//binboo/configuration";
	}
}
