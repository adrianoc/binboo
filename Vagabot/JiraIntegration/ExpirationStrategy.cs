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

namespace Binboo.JiraIntegration
{
	internal class ExpirationStrategy<T>
	{
		public ExpirationStrategy(T item, Action<T> refresh)
		{
			_item = item;
			_refresh = refresh;
		}

		public T Item
		{
			get
			{
				Refresh();
				return _item;
			}
		}

		public void Refresh()
		{
			if (DateTime.Now.Subtract(_lastAccess) > TimeSpan.FromMinutes(ExpireTimeOut))
			{
				_refresh(_item);
			}

			_lastAccess = DateTime.Now;
		}

		private const int ExpireTimeOut = 7;

		private readonly T _item;
		private readonly Action<T> _refresh;
		private DateTime _lastAccess = DateTime.MinValue;
	}
}
