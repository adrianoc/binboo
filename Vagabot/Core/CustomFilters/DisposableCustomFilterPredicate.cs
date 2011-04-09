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
using System.IO;
using System.Security;
using log4net;

namespace Binboo.Core.CustomFilters
{
	public class DisposableCustomFilterPredicate<T> : ICustomFilterPredicate<T>, IDisposable
	{
		public DisposableCustomFilterPredicate(ICustomFilterPredicate<T> predicate, AppDomain domain, string assemblyPath)
		{
			_predicate = predicate;
			_domain = domain;
			_assemblyPath = assemblyPath;
		}

		public bool Include(T candidate)
		{
			try
			{
				return _predicate.Include(candidate);
			}
			catch(TypeInitializationException tie)
			{
				LogException(tie);
			}
			catch(SecurityException sex)
			{
				LogException(sex);
			}
			return false;
		}

		private void LogException(Exception ex)
		{
			_logger.Error("Exception caught while running custom filter.", ex);
		}

		public void Dispose()
		{
			if (_domain != null)
			{
				AppDomain.Unload(_domain);
				File.Delete(_assemblyPath);
			}
		}

		private readonly ILog _logger = LogManager.GetLogger(typeof(DisposableCustomFilterPredicate<>));
		private readonly ICustomFilterPredicate<T> _predicate;
		private readonly AppDomain _domain;
		private readonly string _assemblyPath;
	}

}
