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

using System.Collections;
using System.Linq;

namespace Binboo.JiraIntegration
{
	internal class CustomFieldId : JiraConstantBase<RemoteField, CustomFieldId>
	{
		internal static CustomFieldId OriginalIDsEstimate;
		internal static CustomFieldId Peers;
		internal static CustomFieldId Iteration;
		internal static CustomFieldId Order;
		internal static CustomFieldId Labels;

		public static void Initialize(RemoteField[] statuses)
		{
			InitializeBase(statuses);
		}
		
		internal CustomFieldId(string id, string description) : base(id, description)
		{
		}

		static CustomFieldId()
		{
			ToBeInitialized<CustomFieldId>(c => OriginalIDsEstimate, "original ids estimate", Create);
			ToBeInitialized<CustomFieldId>(c => Peers, "peers", Create);
			ToBeInitialized<CustomFieldId>(c => Iteration, "iteration", Create);
			ToBeInitialized<CustomFieldId>(c => Order, "order", Create);
			ToBeInitialized<CustomFieldId>(c => Labels, "labels", Create);
		}

		private static object Create(string name, IEnumerable constants)
		{
			var id = (from RemoteField field in constants
			          where field.name.ToLower() == name
			          select field.id).Single();

			return new CustomFieldId(id, name);
		}

		protected override CustomFieldId This()
		{
			return this;
		}
	}
}