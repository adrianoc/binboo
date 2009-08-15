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
using Binboo.Core.Commands;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Commands
{
	[TestFixture]
	public class IssueAssignCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void TestJustRequiredArguments()
		{
			AssertIssueAssignment("PRJ-123", "binboo_test_user", string.Empty, string.Empty);
		}

		[Test]
		public void TestBulkAssignment()
		{
			AssertIssueAssignment("PRJ-123, PRJ-124, PRJ-125", "binboo_test_user", "42", string.Empty);
		}
		
		[Test]
		public void TestBulkAssignmentWithInvalidIssue()
		{
			AssertIssueAssignment("PRJ-123, " +  NonExistingIssue + ", PRJ-125", "binboo_test_user", "42", string.Empty);
		}

		[Test]
		public void TestFullArgumentLine()
		{
			AssertIssueAssignment("FAL-123", "binboo_fal", "12", "fal_peer");
		}

		[Test]
		public void TestIterationIsSetAfterFirstAssignmetn()
		{
			AssertIssueAssignment("PRJ-123", "binboo_test_user", "42", string.Empty);
			AssertIssueAssignment("PRJ-123", "binboo_test_user", p => p.Values[0] == "42");
		}

		private void AssertIssueAssignment(string ticket, string user, Expression<Predicate<IssueField>> iterationPredicate)
		{
			using (var commandMock = NewCommand<IssueAssignCommand>(
				mock => mock.Setup(proxy => proxy.AssignIssue(
												ticket,
												It.Is<IssueField>(assigneeField => assigneeField.Id == IssueField.Assignee.Id),
												It.Is<IssueField>(peerField => peerField.Id == CustomFieldId.Peers.Id),
												It.Is(iterationPredicate)
												))))
			{

				IContext context = ContextMockFor(ticket, user).Object;
				string result = commandMock.Process(context);

				Assert.AreEqual(ExpectedAssignmentMessageFor(ticket, user), result);
				commandMock.Verify();
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
					sb.AppendFormat("Successfuly assigned issue {0} to {1}\r\n", ticketWithNoSpaces, user);
				}
			}

			return sb.Remove(sb.Length - 2, 2).ToString();
		}

		private void AssertIssueAssignment(string ticket, string user, string iteration, string peer)
		{
			CommandMock<IssueAssignCommand> commandMock = NewCommand<IssueAssignCommand>(MockSetupsFor(ticket, peer));

			IContext context = ContextMockFor(ticket, user, peer, iteration).Object;
			string result = commandMock.Process(context);
			Assert.AreEqual(ExpectedAssignmentMessageFor(ticket, user), result);
		}

		private static Action<Mock<IJiraProxy>>[] MockSetupsFor(string ticket, string peer)
		{
			string[] ticketList = ticket.Split(',');
			var setups = new Action<Mock<IJiraProxy>>[ticketList.Length];

			for(int i =0; i < ticketList.Length; i++)
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
				};
			}

			return setups;
 		}

		private const string NonExistingIssue = "NON-666";
	}
}
