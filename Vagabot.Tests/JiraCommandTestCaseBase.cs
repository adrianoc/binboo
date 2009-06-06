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

using Binboo.JiraIntegration;

namespace Binboo.Tests
{
	public class JiraCommandTestCaseBase
	{
		private static RemoteStatus[] IssueStatus = {
		                                            	new RemoteStatus {id="1", name = "open"}, 
		                                            	new RemoteStatus {id="2", name = "closed"}, 
		                                            	new RemoteStatus {id="3", name = "in progress"}, 
		                                            	new RemoteStatus {id="4", name = "resolved"}, 
		                                            	new RemoteStatus {id="5", name = "reopened"}, 
		                                            };

		private static RemoteIssueType[] IssueTypes = 
			{
				new RemoteIssueType {id = "1", name="bug"}, 
				new RemoteIssueType {id = "2", name="task"}, 
				new RemoteIssueType {id = "3", name="improvement"}, 
				new RemoteIssueType {id = "4", name="new feature"}, 
			};

		static JiraCommandTestCaseBase()
		{
			IssueType.Initialize(IssueTypes);
			JiraIntegration.IssueStatus.Initialize(IssueStatus);
			//CustomFieldId.Initialize(new RemoteField[] { });
			//IssueResolution.Initialize(new RemoteResolution[] {});
			//IssuePriority.Initialize(new RemotePriority[] {});
		}
	}
}