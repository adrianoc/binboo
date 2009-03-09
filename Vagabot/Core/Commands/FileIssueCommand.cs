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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class FileIssueCommand : JiraCommandBase
	{
		public FileIssueCommand(IJiraProxy proxy, string help) : base(proxy, help)
		{
		}

		public override string Id
		{
			get { return "File"; }
		}

		public override string Process(Context context)
		{
			ParamValidator[] validators = {
			                              	ParamValidator.Project,
			                              	ParamValidator.AnythingStartingWithText,
			                              	ParamValidator.AnythingStartingWithText.AsOptional(),
			                              	ParamValidator.Order,
											ParamValidator.Type
			                              };

			var args = context.Arguments;
			var ret = CheckParameters(args, validators);

			if (ret != null) return ret;
			
			return Run(
						() => _jira.FileIssue(
									ConfigServices.IMUserToIssueTrackerUser(context.UserName),
									args[0], 
									args[1],
									OptionalParameterOrDefault(args, NonOptionalCount(validators), ParamValidator.AnythingStartingWithText.ButNot(ParamValidator.Type), String.Empty),
									IssueTypeOrDefault(args, NonOptionalCount(validators), IssueType.Bug),
									OrderOrDefault(args, NonOptionalCount(validators), -1)),
						issue => string.Format("Jira tiket created successfuly ({2}).{0}{0}{1}", Environment.NewLine, IssueToResultString(issue), UrlFor(issue)));
		}

		private static string IssueTypeOrDefault(IEnumerable<string> args, int startIndex, string @default)
		{
			Match match = OptionalParameterOrNull(args, startIndex, ParamValidator.Type);
			return match == null ? @default : IssueType.Parse(match.Groups[1].Value).Id;
		}

		private static int OrderOrDefault(IEnumerable<string> args, int startIndex, int @default)
		{
			Match match = OptionalParameterOrNull(args, startIndex, ParamValidator.Order);
			return match == null ? @default : Int32.Parse(match.Value);
		}
	}
}