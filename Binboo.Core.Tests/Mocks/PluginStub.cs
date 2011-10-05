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
using Binboo.Core.Commands;
using Binboo.Core.Persistence;
using Binboo.Core.Plugins;
using Moq;

namespace Binboo.Core.Tests.Mocks
{
	class PluginStub : AbstractBasePlugin
	{
		public PluginStub(params string[] commands) : base(null)
		{
			foreach (var command in commands)
			{
				var cmdMock = new Mock<IBotCommand>(MockBehavior.Loose);
				cmdMock.Setup(cmd => cmd.Id).Returns(command);

				var storageManMock = new Mock<IStorageManager>();
				storageManMock.Setup(sm => sm.StorageFor(command)).Returns((IStorage) null);
				AddCommand(storageManMock.Object, cmdMock.Object);
			}
		}

		public override string Name
		{
			get { return "plugin-stub"; }
		}

		public override void Initialize()
		{
			throw new System.NotImplementedException();
		}
	}
}
