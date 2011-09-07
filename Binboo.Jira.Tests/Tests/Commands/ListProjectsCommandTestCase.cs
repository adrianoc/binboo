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
using System.Linq;
using Binboo.Jira.Commands;
using Binboo.Jira.Integration;
using NUnit.Framework;

namespace Binboo.Jira.Tests.Tests.Commands
{
	[TestFixture]
	public class ListProjectsCommandTestCase : JiraCommandTestCaseBase
	{
		[Test]
		public void Test()
		{
			using(var commandMock = NewCommand<ListProjectsCommand>(mock => mock.Setup(proxy => proxy.GetProjectList()).Returns(_projects)))
			{
				var contextMock = ContextMockFor("list-user");
				var result = commandMock.Process(contextMock.Object);
				
				StringAssert.Contains("OK", result.HumanReadable);
				Assert.AreEqual(string.Join(", ", _projects.Select( p => p.key).ToArray() ), result.PipeValue);
			}
		}

		private RemoteProject[] _projects = new []
		                                    	{
		                                    		new RemoteProject {key="P1", lead = "P1 lead", description = "P1 description"}, 
													new RemoteProject {key="P2", lead = "P2 lead", description = "P2 description"}, 
												};
	}
}
