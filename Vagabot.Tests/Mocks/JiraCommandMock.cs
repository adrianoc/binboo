using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
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
			_called.Set();
			_arguments = CollectAndValidateArguments(ctx.Arguments, _validators);
			return _returnProvider(ctx, _arguments);
		}

		public bool Wait(int timeout)
		{
			return _called.WaitOne(timeout);
		}

		public override string ToString()
		{
			return _id;
		}

		private readonly EventWaitHandle _called = new EventWaitHandle(false, EventResetMode.ManualReset);

		private readonly Expression<Func<int, ParamValidator>>[] _validators;
		private IDictionary<string, Argument> _arguments;
		private readonly Func<IContext, IDictionary<string, Argument>, string> _returnProvider;
		private readonly string _id;
	}
}
