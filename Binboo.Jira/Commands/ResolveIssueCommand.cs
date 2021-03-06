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
using System.Linq;
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Jira.Integration;

namespace Binboo.Jira.Commands
{
	internal class ResolveIssueCommand : JiraCommandBase
	{
		public ResolveIssueCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "Resolve"; }
		}

		/**
		 * args: issue resolution (fixed, wont fix, etc) versions=[+]v1,[+]v2,...,vn "comment"
		 * 
		 * if version is informed but does not exist yet:
		 *		+ present    : force adding the version in case it doesn't exist
		 *		+ NOT present: error if version does not exist
		 * 
		 */ 
		protected override ICommandResult ProcessCommand(IContext context)
		{
			IDictionary<string, Argument> arguments = CollectAndValidateArguments(context);
			return ResolveIssue(
						arguments["issueId"],
						arguments["comment"].ValueOrDefault(string.Empty),
						arguments["resolution"],
						arguments["fixedInVersion"]);
		}

		private IDictionary<string, Argument> CollectAndValidateArguments(IContext context)
		{
			return CollectAndValidateArguments(context.Arguments,
                                               issueId => JiraParamValidator.IssueId,
                                               resolution => ParamValidator.From(IssueResolution.FriendlyNames()),
                                               fixedInVersion => JiraParamValidator.Version.AsOptional(),
			                                   comment => ParamValidator.Anything.AsOptional());
		}

		private ICommandResult ResolveIssue(string ticket, string comment, string resolution, Argument fixedInVersions)
		{
			return Run(
					() =>
					{
						var versions = EnumerableFor(fixedInVersions).ToList();
						return _jira.ResolveIssue(ticket, comment, IssueResolution.Parse(resolution), versions);
					},
				
					issue => CommandResult.Success(string.Format("Issue {0} ('{1}') resolved as '{2}'.", ticket, issue.summary, IssueResolution.Parse(resolution).Description), ticket));
		}

		private static IEnumerable<string> EnumerableFor(Argument fixedInVersions)
		{
			if (!fixedInVersions.IsPresent)
			{
				yield break;
			}

			foreach (var version in fixedInVersions.Values)
			{
				yield return version;
			}
		}
	}
}
