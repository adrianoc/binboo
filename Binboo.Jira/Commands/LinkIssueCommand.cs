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
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Commands.Arguments;
using Binboo.Jira.Configuration;
using Binboo.Jira.Integration;
using TCL.Net.Extensions;

namespace Binboo.Jira.Commands
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

		protected override ICommandResult ProcessCommand(IContext context)
		{
			var arguments = CollectAndValidateArguments(context.Arguments,
                                                        sourceIssueKey => JiraParamValidator.IssueId,
														linkDescription => ParamValidator.AnythingStartingWithText,
                                                        targetIssueKey => JiraParamValidator.IssueId,
														verbose => ParamValidator.Custom("verbose", true));

			var sourceTitket = arguments["sourceIssueKey"];
			var targetTicket = arguments["targetIssueKey"];

			var ret = Run(	delegate
			{
				string aliasedLinkDescription = AliasedLinkDescription(arguments["linkDescription"].Value);

				string result = _jira.CreateLink(sourceTitket, aliasedLinkDescription, targetTicket, arguments["verbose"].IsPresent);
			    return ResultMessageFor(result, string.Format("Link created successfully: {0} {1} {2}",
			    																	sourceTitket.Value,
																					arguments["linkDescription"].Value,
			    																	targetTicket.Value));
			});

			return CommandResult.Success(ret, sourceTitket, targetTicket);
		}

		private static string AliasedLinkDescription(string linkDescription)
		{
			var aliasedLinkDescription = Configuration().AliasFor(linkDescription);
			return aliasedLinkDescription.IsNull ? linkDescription : aliasedLinkDescription.Replacement;
		}

		private static LinkConfiguration Configuration()
		{
			return JiraConfig.Instance.CommandConfigurationFor("link").Deserialize<LinkConfiguration>();
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
