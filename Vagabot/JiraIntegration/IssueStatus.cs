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
	internal class IssueStatus : JiraConstantBase<RemoteStatus, IssueStatus>
	{
		public static IssueStatus Open;
		public static IssueStatus Closed;
		public static IssueStatus InProgress;
		public static IssueStatus Resolved;
		public static IssueStatus ReOpened;

		public static bool IsFinished(string status)
		{
			return status == Closed || status == Resolved;
		}

		private IssueStatus(string id, string name) : base(id, name)
		{
		}

		protected override IssueStatus This()
		{
			return this;
		}

		private static object Create(string name, IEnumerable constants)
		{
			var id = (from RemoteStatus ri in constants where ri.name.ToLower() == name select ri.id).Single();
			return new IssueStatus(id, name);
		}

		static IssueStatus()
		{
			ToBeInitialized<IssueStatus>(s => Open, "open", Create);
			ToBeInitialized<IssueStatus>(s => Closed, "closed", Create);
			ToBeInitialized<IssueStatus>(s => InProgress, "in progress", Create);
			ToBeInitialized<IssueStatus>(s => Resolved, "resolved", Create);
			ToBeInitialized<IssueStatus>(s => ReOpened, "reopened", Create);
		}

		public static void Initialize(RemoteStatus []statuses)
		{
			InitializeBase(statuses);
		}
	}
}
