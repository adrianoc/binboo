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
using System.Linq.Expressions;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Commands.Support;
using Binboo.Core.Tests.Tests.Support;

namespace Binboo.Core.Tests.Framework
{
    class ArgumentPipingCommand : BotCommandBase
    {
        private readonly string commandName = String.Empty;
        private readonly ArgumentRecorder recorder;

        public ArgumentPipingCommand(string commandName, Expression<Func<int, ParamValidator>>[] validatorExpressions, ArgumentRecorder recorder) : base(String.Empty)
        {
            this.commandName = commandName;
            this.recorder = recorder;
            this.validatorExpressions = (validatorExpressions != null && validatorExpressions.Length > 0) ? validatorExpressions : null;
        }

        public override string Id
        {
            get { return commandName; }
        }

        public override ICommandResult Process(IContext context)
        {
            var handler = recorder.HandlerFor(commandName);
            var arguments = CollectAndValidateArguments(context.Arguments, validatorExpressions ?? defaultValidatorExpressions());
            
            return handler(context, arguments);
        }

        private readonly Expression<Func<int, ParamValidator>>[] validatorExpressions;

        private static Expression<Func<int, ParamValidator>>[] defaultValidatorExpressions()
        {
            return new Expression<Func<int, ParamValidator>>[]
            {
                s1 => ParamValidator.Custom("(?:\\s*)s1", true),
                s2 => ParamValidator.Custom("(?<param>s2)", true),
                s3 => ParamValidator.Custom("(?:\\s*)s3", true),
                s3 => ParamValidator.Custom("(?:\\s*)s3", true),
                s4 => ParamValidator.Custom("(?:\\s*)s4", true),
                multiple => ParamValidator.Custom(@"(?<param>m[0-9])((?:\s*,\s*)(?<param>m[0-9]))*", true),
            };
        }

    }
}
