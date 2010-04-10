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
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Binboo.Core.Persistence
{
	partial class Storage
	{
		internal class StorageEntry : IActivatable
		{
			public StorageEntry(string id)
			{
				_id = id;
			}

			public string Id
			{
				get
				{
					Activate(ActivationPurpose.Read);
					return _id;
				}
			}

			public object Value
			{
				get
				{
					Activate(ActivationPurpose.Read);
					return _value;
				}

				set
				{
					Activate(ActivationPurpose.Write);
					_value = value;
				}
			}

			public object this[string name]
			{
				get
				{
					Activate(ActivationPurpose.Read);
					return _namedValues[name];
				}

				set
				{
					Activate(ActivationPurpose.Write);
					_namedValues[name] = value;	
				}
			}

			public bool Contains(string name)
			{
				Activate(ActivationPurpose.Read);
				return _namedValues.ContainsKey(name);
			}

			public void Bind(IActivator activator)
			{
				if (activator == _activator) return;

				if (_activator != null && activator != null)
				{
					throw new InvalidOperationException();
				}

				_activator = activator;
			}

			public void Activate(ActivationPurpose purpose)
			{
				if (_activator != null)
				{
					_activator.Activate(purpose);
				}
			}

			private readonly string _id;

			private object _value;
			private readonly IDictionary<string, object> _namedValues = new Dictionary<string, object>();

			[NonSerialized]
			private IActivator _activator;
		}
	}
}
