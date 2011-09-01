/**
 * Copyright (c) 2011 Adriano Carlos Verona
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
using System.Collections.Generic;
using System.Linq;
using Binboo.JiraIntegration;
using Moq;
using NUnit.Framework;

namespace Binboo.Tests.Core.Commands
{
	[TestFixture]
	class VersionCommandTestCase : JiraCommandTestCaseBase
	{
		private const string NonExistingVersion = "42.43.44";
		private const string TestVersion = "1.2.3";

		[Test]
		public void TestAddVersion()
		{
			Assert.IsFalse(VersionExists(NonExistingVersion));
			AddVersion(NonExistingVersion, TestVersion);
			Assert.IsTrue(VersionExists(NonExistingVersion));
		}

		[Test]
		public void TestListVersions()
		{
		}

		[Test]
		public void TestReleaseVersion()
		{
			Assert.IsFalse(VersionReleased(TestVersion));
			ReleaseVersion(TestVersion);
			Assert.IsTrue(VersionReleased(TestVersion));
		}

		private bool VersionReleased(string version)
		{
			return _versions[version].released;
		}

		private bool VersionExists(string version)
		{
			return _versions.ContainsKey(version);
		}

		private void AddVersion(string version, string after)
		{

			var expectedMethodCalls = new Action<Mock<IJiraProxy>>[]
			                          	{
			                          		pm => pm.Setup(
			                          		 			proxy => proxy.AddVersion("PROJ", It.Is<string>(actualVersion => version == actualVersion))).Callback((string prj, string ver) => AddVersionToCollection(prj, ver))
										};

			using (var commandMock = NewCommand<VersionCommand>(expectedMethodCalls))
			{
				//#jira version add PRJ
				//#jira version list PRJ
				//#jira version release PRJ 8.1.189
				var contextMock = ContextMockFor("version-user", String.Format("add PROJ {0}", version));
				contextMock.Setup(ctx => ctx.UserName).Returns("unit.test.user");

				var expectedOutput = string.Format("Issue {0} ('{1}') resolved as '{2}'.", ticket, IssueTestService.Issue[ticket].summary, resolution.Description);

				var result = commandMock.Process(contextMock.Object);

				Assert.AreEqual(expectedOutput, result.HumanReadable);
				Assert.AreEqual(ticket, result.PipeValue);
			}
		}


		private void AddVersionToCollection(string project, string version)
		{
			_versions[project] = NewRemoteVersion(version);
		}

		private static RemoteVersion NewRemoteVersion(string version)
		{
			return new RemoteVersion {name = version, archived = false, id = version, released = false};
		}


		private readonly IDictionary<string, RemoteVersion> _versions = new Dictionary<string, RemoteVersion> { {"PROJ", NewRemoteVersion(TestVersion) } };

		private static Action<Mock<IJiraProxy>> ExpectedMethodCalls(string ticket, string noQuotesComment, IssueResolution resolution, string fixedInVersions)
		{
			return proxyMock => proxyMock.Setup(
									p => p.ResolveIssue(
												ticket,
												It.Is<String>(remmark => remmark == noQuotesComment),
												resolution,
												It.Is<IEnumerable<string>>(
													versions => string.IsNullOrEmpty(fixedInVersions)
																	? true
																	: versions.Any(version => IsValidVersion(fixedInVersions, version)))
										)
								).Returns(IssueTestService.Issue[ticket]);
		}
	}
}
