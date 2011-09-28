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
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Commands.Support;
using Binboo.Dict.Configuration;
using Binboo.Dict.MicrosoftTranslator;

namespace Binboo.Dict.Commands
{
    public class TranslateCommand : BotCommandBase
    {
        private readonly LanguageService _languageSrv;

        public TranslateCommand(LanguageService languageSrv, string help) : base(help)
        {
            _languageSrv = languageSrv;
        }

        public override string Id
        {
            get { return "translate"; }
        }

        
        /// <summary>
        /// Argument format:
        /// 
        /// $translate blabla de>pt
        /// $translate blabla >pt
        /// $translate blabla ble ble pt:de
        /// </summary>
        public override ICommandResult Process(IContext context)
        {
            var arguments = CollectAndValidateArguments(
                                    context.Arguments,
                                    toBeTranslated => ParamValidator.Custom(@"(?<param>^((?![a-z]{2}\>)\w|\s)+)", false),
                                    sourceLanguage => TranslatorParamValidator.SourceLanguage,
                                    targetLanguage => TranslatorParamValidator.TargetLanguage);

            return CommandResult.Success(Run<Exception>(delegate
            {
                var translated = _languageSrv.Translate(
												TranslateConfig.Instance.APIKey,
                                                arguments["toBeTranslated"],
                                                EnsureValid(arguments["sourceLanguage"], context.User.CountryCode),
                                                arguments["targetLanguage"],
                                                "text/plain",
                                                "general");

                return translated;
            }));
        }

        private static string EnsureValid(string sourceLanguage, string defaultValue)
        {
            //TODO:  Fix regular expression. Tried just wrap \> (in SourceLanguageValidator) with (?=\>)
            //       It works, but then another step in checking the paramenters fails because we check
            //       "" against SourceLanguageParameter (empty string) and it fails.
            return string.IsNullOrWhiteSpace(sourceLanguage) || sourceLanguage == ">" ? defaultValue : sourceLanguage;
        }
    }
}
