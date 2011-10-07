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
using System.Globalization;
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Language.Commands;
using Binboo.Language.MicrosoftTranslator;
using Binboo.Plugins.Tests.Foundation.Commands;
using Moq;
using NUnit.Framework;

namespace Binboo.Language.Tests.Commands
{
    [TestFixture]
    public class TranslateCommandTestCase : PluginCommandTestCaseBase<LanguageService>
    {
        [Test]
        public void TestSourceLanguageOmmited()
        {
            //TODO: Come up with a solution to get correct cache behavior.
            Reset<TranslateCommand>();
            AssertCommand("a ser traduzido ", "", "de", "texto em alemão");
        }

        [Test]
        public void TestTargetLanguageOmmited()
        {
            //TODO: Come up with a solution to get correct cache behavior.
            Reset<TranslateCommand>();
            AssertCommand("a ser traduzido ", "en", "", "texto em portuga");
        }
        
        [Test]
        public void TestSourceAndTargetLanguagesSpecified()
        {
            AssertCommand("to be translated ", "en", "pt", "Não importa.");
        }

        private void AssertCommand(string toBeTranslated, string from, string to, string translated)
        {
            var explicityFrom = LangCodeFrom(@from);
        	var explicityTo = LangCodeFrom(to);
            using (var command = NewCommand<TranslateCommand, string>(service => service.Translate(It.IsAny<string>(), toBeTranslated, explicityFrom, explicityTo, It.IsAny<string>(), It.IsAny<string>()), translated))
            {
                Mock<IContext> contextMock = ContextMockFor("translate-user", from + ":" + to + " \"" + toBeTranslated + "\"");
                var result = command.Process(contextMock.Object);
                Assert.That(result.Status, Is.EqualTo(CommandStatus.Success), result.HumanReadable);
            }
        }

    	private static string LangCodeFrom(string lang)
    	{
    		return string.IsNullOrEmpty(lang) ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : lang;
    	}
    }
}
