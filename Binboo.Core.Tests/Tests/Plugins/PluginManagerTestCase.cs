using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Binboo.Core.Commands;
using Binboo.Core.Plugins;
using NUnit.Framework;

namespace Binboo.Core.Tests.Tests.Plugins
{
    [TestFixture]
    public class PluginManagerTestCase
    {
        [Test]
        public void Create_NoCatalog_NoPluginsFound()
        {
            var manager = PluginManagerFactory.Create(null);
            Assert.That(0, Is.EqualTo(manager.Plugins.Count));
        }

        [Test]
        public void Create_TypeCatalogWith1Plugin_SinglePluginFound()
        {
            var manager = PluginManagerFactory.Create(new TypeCatalog(typeof(TestPlugin)));
            Assert.That(manager.Plugins.Count, Is.EqualTo(1));
        }
    }

    [Export(typeof(IPlugin))]
    public class TestPlugin : IPlugin
    {
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IBotCommand> Commands
        {
            get { throw new NotImplementedException(); }
        }

        public ICommandResult ExecuteCommand(string commandName, IContext context)
        {
            throw new NotImplementedException();
        }

    	public bool Enabled
    	{
    		get { throw new NotImplementedException(); }
    		set { throw new NotImplementedException(); }
    	}

    	public void Initialize()
    	{
    		throw new NotImplementedException();
    	}
    }
}
