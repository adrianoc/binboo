/**
 * Copyright (c) 2010 Adriano Carlos Verona
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
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Configuration;
using Binboo.JiraIntegration;
using TCL.Net.Extensions;

namespace Binboo.Core.Commands
{
	class LinkIssueCommand : JiraCommandBase
	{
		public LinkIssueCommand(IJiraProxy jira, string help) : base(jira, help)
		{
		}

		public override string Id
		{
			get { return "Link"; }
		}

		public override string Help
		{
			get
			{
				return AppendAliases(base.Help);
			}
		}

		protected override string ProcessCommand(IContext context)
		{
			var arguments = CollectAndValidateArguments(context.Arguments, 
														sourceIssueKey => ParamValidator.IssueId,
														linkDescription => ParamValidator.AnythingStartingWithText,
														targetIssueKey => ParamValidator.IssueId,
														verbose => ParamValidator.Custom("verbose", true));

			return Run(	delegate
			{
				string aliasedLinkDescription = AliasedLinkDescription(arguments["linkDescription"].Value);

				string result = _jira.CreateLink(arguments["sourceIssueKey"], aliasedLinkDescription, arguments["targetIssueKey"], arguments["verbose"].IsPresent);
			    return ResultMessageFor(result, string.Format("Link created successfully: {0} {1} {2}",
			    																	arguments["sourceIssueKey"].Value,
																					arguments["linkDescription"].Value,
			    																	arguments["targetIssueKey"].Value));
			});
		}

		private static string AliasedLinkDescription(string linkDescription)
		{
			var aliasedLinkDescription = Configuration().AliasFor(linkDescription);
			return aliasedLinkDescription.IsNull ? linkDescription : aliasedLinkDescription.Replacement;
		}

		private static LinkConfiguration Configuration()
		{
			return ConfigServices.CommandConfigurationFor("link").Deserialize<LinkConfiguration>();
		}

		private string ResultMessageFor(string failureMessage, string successMessage)
		{
			return string.Format("[{0}] {1}", Id, failureMessage ?? successMessage);
		}

		private static string AppendAliases(string message)
		{
			return Configuration().Aliases.Aggregate(message + "\r\nAvailable link types:",
													 (acc, curr) => acc + "\r\n\t" + curr.Original);
		}
	}
}
