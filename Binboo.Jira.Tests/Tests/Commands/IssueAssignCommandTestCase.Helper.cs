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
using System.Linq.Expressions;
using System.Text;
using Binboo.Core.Commands.Support;
using Binboo.Jira.Commands;
using Binboo.Jira.Integration;
using Binboo.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	public partial class IssueAssignCommandTestCase
	{
		private void AssertIssueAssignment(string ticket, string user, Expression<Func<IssueField, bool>> iterationPredicate)
		{
			AssertIssueAssignment("assign-user", ticket, iterationPredicate, user);
		}

		private void AssertIssueAssignment(string skypeUser, string ticket, Expression<Func<IssueField, bool>> iterationPredicate, string user)
		{
			Action<Mock<IJiraProxy>> mockSetups = MockSetupWithIterationPredicateFor(ticket, iterationPredicate);

			using (var commandMock = NewCommand<IssueAssignCommand>(mockSetups))
			{
				IContext context = ContextMockFor(skypeUser, ticket, user).Object;
				var result = commandMock.Process(context);
				Assert.AreEqual(ExpectedAssignmentMessageFor(ticket, user), result.HumanReadable);
			}
		}

		private void AssertIssueAssignment(string ticket, string user, string peer, string iteration)
		{
			AssertIssueAssignment("assign-user", ticket, user, peer, iteration);
		}

		private void AssertIssueAssignment(string skypeUser, string ticket, string user, string peer, string iteration)
		{
			using (var commandMock = NewCommand<IssueAssignCommand>(MockSetupsFor(ticket, peer)))
			{
				IContext context = ContextMockFor(skypeUser, ticket, user, peer, iteration).Object;
				var result = commandMock.Process(context);
				
				Assert.AreEqual(ExpectedAssignmentMessageFor(ticket, user), result.HumanReadable);
				Assert.AreEqual(ticket, result.PipeValue);
			}
		}

		private void AssertCachedAssigneeAndPeer(string skpypeUser, string issue, string expectedAssign, string expectedPeer)
		{
			using (var commandMock = NewCommand<IssueAssignCommand>(mock => mock.Setup(proxy => proxy.AssignIssue(It.IsAny<string>(), It.Is<IssueField>(field => field.Id == IssueField.Assignee.Id && field.Values[0] == expectedAssign), It.Is<IssueField>(field => field.Id == CustomFieldId.Peers.Id && field.Values[0] == expectedPeer), It.IsAny<IssueField>())).Returns(IssueTestService.Issue[issue])))
			{
				Assert.AreEqual(ExpectedAssignmentMessageFor(issue, expectedAssign), commandMock.Process(ContextMockFor(skpypeUser, issue).Object).HumanReadable);
			}
		}

		private void AssertNoAssigneeAndPeerCached(string skpypeUser, string issue)
		{
			using (var commandMock = NewCommand<IssueAssignCommand>())
			{
				StringAssert.StartsWith("Failed", commandMock.Process(ContextMockFor(skpypeUser, issue).Object).HumanReadable);
			}
		}

		private static string ExpectedAssignmentMessageFor(string ticketList, string user)
		{
			var sb = new StringBuilder();
			foreach (var ticket in ticketList.Split(','))
			{
				var ticketWithNoSpaces = ticket.Trim();
				if (NonExistingIssue == ticketWithNoSpaces)
				{
					sb.AppendFormat("Failed to asssign issue {0}\r\nFailed to get issue: {0}\r\n", ticketWithNoSpaces);
				}
				else
				{
					sb.AppendFormat("Issue {0} ('{1}')\r\nsuccessfuly assigned to {2}\r\n", ticketWithNoSpaces, IssueTestService.Issue[ticketWithNoSpaces].summary, user);
				}
			}

			return sb.Remove(sb.Length - 2, 2).ToString();
		}

		private static Action<Mock<IJiraProxy>>[] MockSetupsFor(string ticket, string peer)
		{
			string[] ticketList = ticket.Split(',');
			var setups = new Action<Mock<IJiraProxy>>[ticketList.Length];

			for (int i = 0; i < ticketList.Length; i++)
			{
				var currentTicket = ticketList[i].Trim();
				setups[i] = mock =>
				{
					var setup = mock.Setup(proxy => proxy.AssignIssue(
											currentTicket,
											It.Is<IssueField>(issueField => issueField.Id == IssueField.Assignee.Id),
											It.Is<IssueField>(p => p.Id == CustomFieldId.Peers.Id && (string.IsNullOrEmpty(peer) || p.Values[0] == peer)),
											It.Is<IssueField>(p => p == null || p.Id == CustomFieldId.Iteration.Id)));

					if (NonExistingIssue == currentTicket)
					{
						setup.Throws(new JiraProxyException("Failed to asssign issue " + NonExistingIssue, new JiraProxyException("Failed to get issue: " + NonExistingIssue)));
					}
					else
					{
						setup.Returns(IssueTestService.Issue[currentTicket]);
					}
				};
			}

			return setups;
		}

		private static Action<Mock<IJiraProxy>> MockSetupWithIterationPredicateFor(string ticket, Expression<Func<IssueField, bool >> iterationPredicate)
		{
			return mock => mock.Setup(proxy => proxy.AssignIssue(
													ticket,It.Is<IssueField>(assigneeField => assigneeField.Id == IssueField.Assignee.Id),
													It.Is<IssueField>(peerField => peerField.Id == CustomFieldId.Peers.Id),
													It.Is(iterationPredicate))).Returns(IssueTestService.Issue[ticket]);
		}
	}
}
