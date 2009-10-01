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
using System.Linq.Expressions;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;

namespace Binboo.Tests.Mocks
{
	class JiraCommandMock : JiraCommandBase
	{
		public JiraCommandMock(string id, Func<IContext, IDictionary<string, Argument>, string> returnProvider, params Expression<Func<int, ParamValidator>> []validators) : base(null, "Jira Command Mock")
		{
			_id = id;
			_returnProvider = returnProvider;
			_validators = validators;
		}

		public override string Id
		{
			get { return _id ?? "MockCommand"; }
		}

		protected override string ProcessCommand(IContext ctx)
		{
			_arguments = CollectAndValidateArguments(ctx.Arguments, _validators);
			return _returnProvider(ctx, _arguments);
		}

		public override string ToString()
		{
			return _id;
		}

		private readonly Expression<Func<int, ParamValidator>>[] _validators;
		private IDictionary<string, Argument> _arguments;
		private readonly Func<IContext, IDictionary<string, Argument>, string> _returnProvider;
		private readonly string _id;
	}
}
