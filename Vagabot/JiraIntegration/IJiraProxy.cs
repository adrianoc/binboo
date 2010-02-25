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

using System.Collections.Generic;

namespace Binboo.JiraIntegration
{
	internal interface IJiraProxy
	{
		void LogOut();
		RemoteIssue[] SearchIssues(string content);
		RemoteProject[] GetProjectList();
		RemoteIssue[] IssuesForFilter(string filterId);
		RemoteIssue GetIssue(string ticket);
		RemoteIssue FileIssue(string reporter, string project, string summary, string description, string type, int order);
		void ResolveIssue(string ticket, string comment, IssueResolution resolution, IEnumerable<string> fixedInVersions);
		void AssignIssue(string ticket, IssueField assignee, IssueField peer, IssueField iteration);
		void UpdateIssue(string ticketNumber, string comment, params IssueField[] fields);
		void AddComment(string ticket, string comment);
		string GetComments(string ticket);
	}
}