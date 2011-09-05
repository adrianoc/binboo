using Binboo.Core.Framework;
using Binboo.Core.Plugins;
using Moq;
using NUnit.Framework;

namespace Binboo.Core.Tests.Tests
{
    [TestFixture]
    public class ContextTestCase
    {
        [Test]
        public void Arguments_Initialized_CorrectArgs()
        {
            var user = new Mock<IUser>();
            const string arguments = "arguments";
            var ctx = new Context(user.Object, null, arguments);

            Assert.That(ctx.Arguments, Is.EqualTo(arguments));
        }
        
        [Test]
        public void Plugin_Initialized_CorrectValue()
        {
            var plugin = new Mock<IPlugin>();
            var ctx = new Context(null, plugin.Object, null);

            Assert.That(ctx.Plugin, Is.SameAs(plugin.Object));
        }
    }
}
