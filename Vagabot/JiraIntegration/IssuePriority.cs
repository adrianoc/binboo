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
	internal class IssuePriority : JiraConstantBase<RemotePriority, IssuePriority>
	{
		internal static IssuePriority Blocker;
		internal static IssuePriority Critical;
		internal static IssuePriority Major;
		internal static IssuePriority Minor;
		internal static IssuePriority Trivial;

		static IssuePriority()
		{
			ToBeInitialized<IssuePriority>(ip => Blocker, Creator);
			ToBeInitialized<IssuePriority>(ip => Critical, Creator);
			ToBeInitialized<IssuePriority>(ip => Major, Creator);
			ToBeInitialized<IssuePriority>(ip => Minor, Creator);
			ToBeInitialized<IssuePriority>(ip => Trivial, Creator);
		}

		private static IssuePriority Creator(string friendlyName, IEnumerable constants)
		{
			var issuePriority = (
						from RemotePriority priority in constants
						where string.Compare(priority.name, friendlyName, true) == 0
						select priority.id).Single();

			return new IssuePriority(issuePriority, friendlyName);
		}

		internal static void Initialize(RemotePriority[] priorities)
		{
			InitializeBase(priorities);
		}

		private IssuePriority(string id, string description) : base(id, description)
		{
		}

		protected override IssuePriority This()
		{
			return this;
		}
	}
}
