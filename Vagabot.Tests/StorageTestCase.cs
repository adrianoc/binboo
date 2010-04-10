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
using Binboo.Core.Persistence;
using NUnit.Framework;

namespace Binboo.Tests
{
	[TestFixture]
	public partial class StorageTestCase
	{
		[Test]
		public void TestStorageIsPerCommand()
		{
			const string command1Value = "command1 value";
			const string command2Value = "command2 value";

			using (IStorageManager manager = new StorageManager())
			{
				var value1 = SetValueFor(manager, Command1Id, command1Value);
				var value2 = SetValueFor(manager, Command2Id, command2Value);

				Assert.AreEqual(value1, command1Value);
				Assert.AreEqual(value2, command2Value);
			}

			using (IStorageManager manager = new StorageManager())
			{
				IStorage stg1 = manager.StorageFor(Command1Id);
				IStorage stg2 = manager.StorageFor(Command2Id);

				Assert.AreEqual(stg1.Value, command1Value);
				Assert.AreEqual(stg2.Value, command2Value);
			}
		}

		[Test]
		public void TestStorage()
		{
			const string subfield = "cmd1-subfield";
			const string subfieldValue = "command1 subfield value";

			using (IStorageManager manager = new StorageManager())
			{
				IStorage storage = manager.StorageFor(Command1Id);
				storage.Value = "command1 value";
				storage[subfield] = subfieldValue;
			}

			using (IStorageManager manager = new StorageManager())
			{
				IStorage storage = manager.StorageFor(Command1Id);
				Assert.AreEqual("command1 value", storage.Value);
				Assert.AreEqual(subfieldValue, storage[subfield]);
			}
		}

		[Test]
		public void TestStorageUpdate()
		{
			const string subfield = "cmd1-subfield";
			const string subfieldValue = "command1 subfield value";

			using (IStorageManager manager = new StorageManager())
			{
				IStorage storage = manager.StorageFor(Command1Id);
				storage.Value = "command1 value";
				storage[subfield] = subfieldValue;
			}

			using (IStorageManager manager = new StorageManager())
			{
				IStorage storage = manager.StorageFor(Command1Id);
				Assert.AreEqual("command1 value", storage.Value);
				Assert.AreEqual(subfieldValue, storage[subfield]);

				storage.Value = "new value";
				storage[subfield] = "new-" + subfieldValue;
			}
			
			using (IStorageManager manager = new StorageManager())
			{
				IStorage storage = manager.StorageFor(Command1Id);
				Assert.AreEqual("new value", storage.Value);
				Assert.AreEqual("new-" + subfieldValue, storage[subfield]);
			}
		}

		[Test]
		public void TestDefaultValues()
		{
		    using (IStorageManager manager = new StorageManager())
			{
				IStorage stg1 = manager.StorageFor("cmd1");
				Assert.IsNull(stg1.Value);
				Assert.IsFalse(stg1.Contains("value1"));
			}
		}

		[SetUp]
		public void Setup()
		{
			using (IStorageManager manager = new StorageManager())
			{
				manager.DeleteAll();
			}
		}
		
		private const string Command1Id = "cmd1";
		private const string Command2Id = "cmd2";
	}
}
