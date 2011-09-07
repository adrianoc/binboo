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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;

using Binboo.Core;
using Binboo.Core.Commands;

namespace Binboo.Plugins.Tests.Foundation.Commands
{
	public class PluginCommandTestCaseBase<TMock> where TMock : class
	{
	    protected Mock<TMock> _mock;
		private static readonly IDictionary<Type, IBotCommand> Commands = new Dictionary<Type, IBotCommand>();

		protected static Mock<IContext> ContextMockFor(string userName, params string[] arguments)
		{
		    return ContextMockFor(userName, "pt", arguments);
		}

	    private static Mock<IContext> ContextMockFor(string userName, string countryCode, params string[] arguments)
	    {
	        var contextMock = new Mock<IContext>();
	        contextMock.Setup(context => context.Arguments).Returns(ZipArguments(arguments));
	        contextMock.Setup(context => context.User.Name).Returns(userName);
	        contextMock.Setup(context => context.User.CountryCode).Returns(countryCode);
			
	        return contextMock;
	    }

	    private static string ZipArguments(string[] arguments)
		{
			return arguments.Length > 0 ? arguments.Aggregate("", (acc, current) => acc + " " + current).Substring(1) : "";
		}

        public CommandMock<TCommand, TMock> NewCommand<TCommand, TResult>(Expression<Func<TMock, TResult>> expectedMethodCall, TResult valueToReturn) where TCommand : IBotCommand
		{
            _mock = new Mock<TMock>();
			_mock.Setup(expectedMethodCall).Returns(valueToReturn);

            return new CommandMock<TCommand, TMock>(FromCacheOrNew<TCommand>(), _mock);
		}

	    public CommandMock<TCommand, TMock> NewCommand<TCommand>(params Action<Mock<TMock>>[] mockSetups) where TCommand: IBotCommand
		{
			_mock = new Mock<TMock>();
			foreach (var setup in mockSetups)
			{
				setup(_mock);
			}

			return new CommandMock<TCommand, TMock>(FromCacheOrNew<TCommand>(), _mock);
		}

	    public void Reset<T>()
		{
			if (Commands.ContainsKey(typeof(T)))
			{
				Commands.Remove(typeof(T));
			}
		}

	    protected virtual T FromCacheOrNew<T>() where T : IBotCommand
		{
            var commandType = typeof(T);
	        if (!Commands.ContainsKey(commandType))
			{
			    Commands[commandType] = (T) Activator.CreateInstance(commandType, new object[] {_mock.Object, commandType.Name});
			}

	        return (T) Commands[commandType];
		}
	}
}