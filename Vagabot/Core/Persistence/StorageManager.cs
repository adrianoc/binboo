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
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;

namespace Binboo.Core.Persistence
{
	internal class StorageManager : IStorageManager
	{
		public void Dispose()
		{
			if (_db != null)
			{
				_db.Close();
				_db = null;
			}
		}

		public IStorage StorageFor(string id)
		{
			EnsureDbAvailable();
			return new Storage(_db, id);
		}

		public void DeleteAll()
		{
			Dispose();
			File.Delete(DatabaseFilePath());
		}

		private void EnsureDbAvailable()
		{
			if (_db == null)
			{
				_db = Db4oEmbedded.OpenFile(NewConfig(), DatabaseFilePath());
			}
		}

		private static IEmbeddedConfiguration NewConfig()
		{
			var configuration = Db4oEmbedded.NewConfiguration();
			configuration.Common.Add(new TransparentPersistenceSupport());
			configuration.Common.ObjectClass(typeof (Storage.StorageEntry)).ObjectField("_id").Indexed(true);
			configuration.Common.ObjectClass(typeof(Storage.StorageEntry)).UpdateDepth(Int32.MaxValue);

			return configuration;
		}

		private static string DatabaseFilePath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "BinbooPersistence.odb");
		}

		private IObjectContainer _db;
	}
}
