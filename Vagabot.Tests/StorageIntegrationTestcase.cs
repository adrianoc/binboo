/**
 * Copyright (c) 2010 Adriano Carlos Verona
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
using System.Linq;
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Persistence;
using Binboo.Tests.Mocks;
using NUnit.Framework;

namespace Binboo.Tests
{
	[TestFixture]
	class StorageIntegrationTestcase : TestWithUser
	{
		private const string AppPrefix = "teststorage";
		private static readonly int[] Values = new[] {42, 39, 6, 41};

		[Test]
		public void Test()
		{
			Setup();
			RunTest();
		}

		private void RunTest()
		{
			using (Application app = new Application(AppPrefix))
			{
				MockStorageCommand command = new MockStorageCommand();
				app.AddCommand(command);

				for (int i = 0; i < Values.Length; i++)
				{
					Assert.AreEqual(Values[i].ToString(), command[i]);
				}
			}
		}

		private void Setup()
		{
			using(Application app = new Application(AppPrefix))
			{
				var chat = new ChatMock(NewUserMock());
				var mockSkype = new SkypeMock(() => chat);
				app.SetSkype(mockSkype);
				app.AttachToSkype();

				app.AddCommand(new MockStorageCommand());

				mockSkype.SendMessage("adriano", "$" + AppPrefix + " test" + Values.Aggregate("", (acc, current) => acc + " " + current));
				chat.WaitForMessages(1000);
			}
		}
	}

	internal class MockStorageCommand : IBotCommand
	{
		public void Initialize()
		{
		}

		public IStorage Storage
		{
			set { _storage = value; }
		}

		public string Help
		{
			get { return ""; }
		}

		public string Id
		{
			get { return "test"; }
		}

		public string Process(IContext context)
		{
			var values = context.Arguments.Split(' ');
			for (int i = 0; i < values.Length; i++)
			{
				_storage["arg #" + i] = values[i];
			}

			return values.Length.ToString();
		}

		public string this[int i]
		{
			get { return (string) _storage["arg #" + i]; }
		}
		
		private IStorage _storage;
	}
}
