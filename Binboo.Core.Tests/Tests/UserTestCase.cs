using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Binboo.Core.Tests.Tests
{
    [TestFixture]
    public class UserTestCase
    {
        [Test]
        public void Name_Initialized_ValidString()
        {
            var user = new User("John", "Doe");
            Assert.That(user.Name, Is.EqualTo("John"));
        }
        
        [Test]
        public void CountryCode_Initialized_ValidString()
        {
            var user = new User("John", "Doe", "pt-br");
            Assert.That(user.CountryCode, Is.EqualTo("pt-br"));
        }
        
        [Test]
        public void CountryCode_NotInitialized_CurrentThreadCountryCodeIsUsed()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("de");
            var user = new User("John", "Doe");
            Assert.That(user.CountryCode, Is.EqualTo("de"));
        }
    }
}
