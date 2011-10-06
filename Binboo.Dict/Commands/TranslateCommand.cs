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
            get { return "t"; }
        }

        
        /// <summary>
        /// Argument format:
        /// 
        /// $translate pt:de "some text"
        /// $translate :de "some text"
        /// </summary>
        public override ICommandResult Process(IContext context)
        {
        	var arguments = CollectAndValidateArguments(
        								context.Arguments,
        								languagePair => TranslatorParamValidator.LanguagePair,
        								toBeTranslated => ParamValidator.Custom(@"\s*""(?<param>[^""]+)""", false));



            return Run<string, Exception>(delegate
											{
												var langs = arguments["languagePair"].Value.Split(':');
												var translated = _languageSrv.Translate(
																				TranslateConfig.Instance.APIKey,
																				arguments["toBeTranslated"],
																				EnsureValid(langs[SourceIndex], context.User.CountryCode),
																				EnsureValid(langs[TargetIndex], context.User.CountryCode),
																				"text/plain",
																				"general");

												return translated;
											},
											
											result => CommandResult.Success(result)
			
			);
        }

        private static string EnsureValid(string sourceLanguage, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(sourceLanguage) ? defaultValue : sourceLanguage;
        }

    	private const byte SourceIndex = 0;
    	private const byte TargetIndex = 1;
    }
}
