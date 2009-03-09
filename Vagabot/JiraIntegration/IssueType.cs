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
	internal class IssueType : JiraConstantBase<RemoteIssueType, IssueType>
	{
		internal static IssueType Bug;
		internal static IssueType Task;
		internal static IssueType NewFeature;
		internal static IssueType Improvement;

		public static void Initialize(RemoteIssueType[] types)
		{
			InitializeBase(types);
		}

		internal IssueType(string id, string description) : base(id, description)
		{
		}

		static IssueType()
		{
			ToBeInitialized<IssueType>(it => Bug, "bug", Create);
			ToBeInitialized<IssueType>(it => Task, "task", Create);
			ToBeInitialized<IssueType>(it => NewFeature, "new feature", Create);
			ToBeInitialized<IssueType>(it => Improvement, "improvement", Create);
		}

		private static object Create(string name, IEnumerable constants)
		{
			var id = (from RemoteIssueType rit in constants
			          where rit.name.ToLower() == name
			          select rit.id).Single();

			return new IssueType(id, name);
		}

		protected override IssueType This()
		{
			return this;
		}
	}
}

