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
using Binboo.Jira.Commands;
using Binboo.Jira.Integration;
using Binboo.Plugins.Tests.Foundation.Commands;

namespace Binboo.Jira.Tests.Tests.Commands
{
	public class JiraCommandTestCaseBase : PluginCommandTestCaseBase<IJiraProxy>
	{
		private static readonly RemoteStatus[] WellKnownIssueStatus = 
												{
													new RemoteStatus {id="1", name = "open"}, 
		                                            new RemoteStatus {id="2", name = "closed"}, 
													new RemoteStatus {id="3", name = "in progress"}, 
													new RemoteStatus {id="4", name = "resolved"}, 
													new RemoteStatus {id="5", name = "reopened"}, 
												};

		private static readonly RemoteIssueType[] WellKnownIssueTypes = 
													{
														new RemoteIssueType {id = "1", name="bug"}, 
														new RemoteIssueType {id = "2", name="task"}, 
														new RemoteIssueType {id = "3", name="improvement"}, 
														new RemoteIssueType {id = "4", name="new feature"}, 
													};

		private static readonly RemoteField[] WellKnownCustomFields = new[]
		                                                  	{
		                                                  		new RemoteField { id = "1", name = "peers" },
		                                                  		new RemoteField { id = "2", name = "iteration" },
		                                                  		new RemoteField { id = "3", name = "original ids estimate" },
		                                                  		new RemoteField { id = "4", name = "order" },
		                                                  		new RemoteField { id = "5", name = "Labels" },
		                                                  	};


		private static RemoteIssue[] _issues;

		private static readonly RemoteResolution [] WellKnownResolutions = new []
		                                           	{
		                                           		new RemoteResolution {id="1", description = "fixed", name="Fixed"},
		                                           		new RemoteResolution {id="2", description = "won't fix", name="Won't Fix"},
		                                           		new RemoteResolution {id="3", description = "duplicate", name="Duplicate"},
		                                           		new RemoteResolution {id="4", description = "incomplete", name="Incomplete"},
		                                           		new RemoteResolution {id="5", description = "cannot reproduce", name="Cannot Reproduce"},
													};


		protected const int CurrentIteration = 2;
		private const int NextIteration = CurrentIteration + 1;

		static JiraCommandTestCaseBase()
		{
			IssueType.Initialize(WellKnownIssueTypes);
			IssueStatus.Initialize(WellKnownIssueStatus);
			CustomFieldId.Initialize(WellKnownCustomFields);
			IssueResolution.Initialize(WellKnownResolutions);
			//IssuePriority.Initialize(new RemotePriority[] {});
		}

		public static RemoteIssue[] Issues
		{
			get 
			{ 
				if (_issues == null)
				{
					_issues = new [] {
		                            	new RemoteIssue {key = "BTS-001", status = IssueStatus.Open, summary = "", assignee = "tetyana", customFieldValues = CustomFields(CurrentIteration, 1, "foo")}, 
		                                new RemoteIssue {key = "BTS-002", status = IssueStatus.Open, summary = "", assignee = "shrek", customFieldValues = CustomFields(CurrentIteration, 2, "foo", "bar")},  
		                                new RemoteIssue {key = "BTS-003", status = IssueStatus.Closed, summary = "", assignee = "rodrigo", customFieldValues = CustomFields(CurrentIteration, 3, "foo","bar","foobar")}, 
		                                new RemoteIssue {key = "BTS-004", status = IssueStatus.Closed, summary = "", assignee = "adriano", customFieldValues = CustomFields(CurrentIteration, 4, "foo","bar","foobar", "baz")}, 
		                                new RemoteIssue {key = "BTS-005", status = IssueStatus.Resolved, summary = "", assignee = "carl", customFieldValues = CustomFields(NextIteration, 5, "foo")}, 
		                                new RemoteIssue {key = "BTS-006", status = IssueStatus.ReOpened, summary = "", assignee = "patrick", customFieldValues = CustomFields(NextIteration, 6, "bar")}, 
		                                new RemoteIssue {key = "BTS-007", status = IssueStatus.InProgress, summary = "", assignee = "anat", customFieldValues = CustomFields(NextIteration, 7, "foobar")}, 
		                                new RemoteIssue {key = "BTS-008", status = IssueStatus.InProgress, summary = "", assignee = "adriano", customFieldValues = CustomFields(CurrentIteration, 8,"wth")}, 
		                             };					
				}
				return _issues; 
			}
		}

		protected override T FromCacheOrNew<T>()
		{
		    var command = (JiraCommandBase) (object) base.FromCacheOrNew<T>();
		    command.Proxy = _mock.Object;
			return (T) (object) command;
		}

		private static RemoteCustomFieldValue[] CustomFields(int iteration, int estimation, params string[] labels)
		{
			var labelsField = new RemoteCustomFieldValue
			                 	{
			                 		customfieldId = CustomFieldId.Labels.Id,
			                 		key = CustomFieldId.Labels.Description,
			                 		values = new[] { string.Join(" ", labels) }
								};

			var iterationField = new RemoteCustomFieldValue
			                     	{
										customfieldId = CustomFieldId.Iteration.Id,
										key = CustomFieldId.Iteration.Description,
										values = new[] { iteration.ToString() }
			                     	};
			
			var estimationField = new RemoteCustomFieldValue
			                     	{
										customfieldId = CustomFieldId.OriginalIDsEstimate.Id,
										key = CustomFieldId.OriginalIDsEstimate.Description,
										values = new[] { estimation.ToString() }
			                     	};

			return new[] { iterationField, labelsField, estimationField };
		}
	}
}