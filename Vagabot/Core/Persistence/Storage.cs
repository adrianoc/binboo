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
using System.Linq;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

namespace Binboo.Core.Persistence
{
	internal partial class Storage : IStorage
	{
		internal Storage(IObjectContainer container, string id)
		{
			_id = id;
			_container = container;
		}

		public object Value
		{
			get
			{
				StorageEntry storageEntry = EnsureStorageEntry();
				return storageEntry.Value;
			}

			set
			{
				var storageEntry = EnsureStorageEntry();
				storageEntry.Value = value;
				_container.Commit();
			}
		}

		public object this[string name]
		{
			get
			{
				StorageEntry storageEntry = EnsureStorageEntry();
				return storageEntry[name];
			}

			set
			{
				var storageEntry = EnsureStorageEntry();
				storageEntry[name] = value;
				_container.Commit();
			}
		}

		public bool Contains(string name)
		{
			var entry = EnsureStorageEntry();
			return entry.Contains(name);
		}

		private StorageEntry EnsureStorageEntry()
		{
			var storageEntries = StorageEntries().ToList();
			if (storageEntries.Count == 0)
			{
				var storageEntry = new StorageEntry(_id);
				_container.Store(storageEntry);
				_container.Commit();

				return storageEntry;
			}
			
			return storageEntries[0];
		}

		private IDb4oLinqQuery<StorageEntry> StorageEntries()
		{
			return from StorageEntry se in _container
				   where se.Id == _id
				   select se;
		}

		private readonly IObjectContainer _container;
		private readonly string _id;
	}
}
