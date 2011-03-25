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
using System.Linq;
using System.Linq.Expressions;

namespace Binboo.JiraIntegration
{
	internal class IssueField
	{
		internal static IssueField Assignee = FieldName((RemoteIssue r) => r.assignee);
		internal static IssueField Description = FieldName((RemoteIssue r) => r.description);
		internal static IssueField Summary = FieldName((RemoteIssue r) => r.summary);
		internal static IssueField Project = FieldName((RemoteIssue r) => r.project);
		internal static IssueField Reporter = FieldName((RemoteIssue r) => r.reporter);
		internal static IssueField Status = FieldName((RemoteIssue r) => r.status);
		internal static IssueField Resolution = FieldName((RemoteIssue r) => r.resolution);
		internal static IssueField FixedInVersion = FieldName((RemoteIssue r) => r.fixVersions);
		
		public static IssueField operator<=(IssueField field, string[] values)
		{
			return new IssueField(field.Id, values);
		}

		public static IssueField operator <=(IssueField field, string value)
		{
			return new IssueField(field.Id, value);
		}

		public static IssueField CustomField(string fieldId)
		{
			return new IssueField(fieldId);
		}

		public static IssueField CustomFieldFromIssue(RemoteIssue issue, CustomFieldId id)
		{
			return new IssueField(id, issue.CustomFieldValue(id));
		}

		public string Id
		{
			get { return _id; }
		}

		public string[] Values
		{
			get { return _values; }
		}

		public static IssueField operator >=(IssueField issueField, string[] values)
		{
			throw new NotImplementedException();
		}

		public static IssueField operator >=(IssueField issueField, string values)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return _id + _values.Aggregate(String.Empty, (acc, current) => acc + (String.IsNullOrEmpty(acc) ? ":" : ",") + current);
		}

		private static IssueField FieldName<T, R>(Expression<Func<T, R>> member)
		{
			var me = (MemberExpression) member.Body;
			return new IssueField(me.Member.Name);
		}
		
		private IssueField(string id) : this(id, null)
		{
		}
		
		private IssueField(string id, params string[] value)
		{
			_id = id;
			_values = value;
		}

		private readonly string _id;
		private readonly string[] _values;
	}
}
