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
	internal class IssueResolution : JiraConstantBase<RemoteResolution, IssueResolution>
	{
		internal static IssueResolution Fixed;
		internal static IssueResolution WontFix;
		internal static IssueResolution Duplicate;
		internal static IssueResolution Incomplete;
		internal static IssueResolution CannotReproduce;

		internal static void Initialize(RemoteResolution[] resolutions)
		{
			InitializeBase(resolutions);
		}

		internal IssueResolution(string id, string description) : base(id, description)
		{
		}

		static IssueResolution()
		{
			ToBeInitialized<IssueResolution>(rr => Fixed, "fixed", Create) ;
			ToBeInitialized<IssueResolution>(rr => WontFix, "won't fix", Create);
			ToBeInitialized<IssueResolution>(rr => Duplicate, "duplicate", Create);
			ToBeInitialized<IssueResolution>(rr => Incomplete, "incomplete", Create);
			ToBeInitialized<IssueResolution>(rr => CannotReproduce, "cannot reproduce", Create);
		}

		private static object Create(string friendlyName, IEnumerable constants)
		{
			var id = (from RemoteResolution rr in constants
			         where rr.name.ToLower() == friendlyName
			         select rr.id).Single();

			return new IssueResolution(id, friendlyName);
		}

		protected override IssueResolution This()
		{
			return this;
		}
	}
}
