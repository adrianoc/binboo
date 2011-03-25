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
using System.Text;
using Binboo.Core.Commands.Arguments;
using Binboo.Core.Commands.Support;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class LabelCommand : JiraCommandBase
	{
		public LabelCommand(IJiraProxy jira, string help) : base(jira, help)
		{
		}

		public override string Id
		{
			get { return "Label"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			var arguments = CollectAndValidateArguments(context.Arguments, 
			                                                              issueList => ParamValidator.MultipleIssueId,
																		  labelOperations => ParamValidator.LabelOperations
																		  );

			var operations = arguments["labelOperations"];
			var issueKeys = arguments["issueList"].Values;

			if (!operations.IsPresent)
			{
				return RetrieveLabelList(issueKeys);
			}

			return ProcessLabelUpdateOperations(issueKeys, ParseLabelOperations(operations));
		}

		private ICommandResult RetrieveLabelList(IEnumerable<string> issueKeys)
		{
			var labels = new HashSet<string>();
			return ExecuteCommand(
						issueKeys,
						
						issueKey =>
						{
							var issue = _jira.GetIssue(issueKey);
							Array.ForEach(LabelFormatConveter.From(issue).To(a => a.ToArray()), label => labels.Add(label));

							return FormatOutputMessage(issueKey, LabelFormatConveter.FromIssue(issue));
						},
						
						() => labels);
		}

		private static IEnumerable<Action<IList<string>>> ParseLabelOperations(Argument labelOperations)
		{
			return labelOperations.Values.Select(labelOperation => ParseLabelOperation(labelOperation)).ToList();
		}

		private static Action<IList<string>> ParseLabelOperation(string labelOperation)
		{
			switch(labelOperation[0])
			{
				case LabelOperations.Add:	 return labels => labels.Add(LabelFrom(labelOperation));
				case LabelOperations.Remove: return labels => labels.Remove(LabelFrom(labelOperation));
			}

			throw new ArgumentException("Invalid label operation for label: " + labelOperation, "labelOperations");
		}

		private ICommandResult ProcessLabelUpdateOperations(IEnumerable<string> issueKeys, IEnumerable<Action<IList<string>>> labelOperations)
		{
			return ExecuteCommand(
							issueKeys,

							issueKey =>
			                {
								var issue = _jira.GetIssue(issueKey);
								var updatedLabels = UpdateLabels(issue, labelOperations);
								
								_jira.UpdateIssue(issueKey, String.Empty, updatedLabels);
								return FormatOutputMessage(issue.key, updatedLabels.Values[0]);
							}, 
							
							() => issueKeys);
		}

		private static ICommandResult ExecuteCommand(IEnumerable<string> issueKeys, Func<string, string> commandBody, Func<IEnumerable<string>> pipeProvider)
		{
			var sb = new StringBuilder();
			foreach (var issueKey in issueKeys)
			{
				var currentIssue = issueKey;
				sb.AppendLine(Run( () => commandBody(currentIssue)));
			}

			return CommandResult.Success(sb.ToString(), pipeProvider());
		}

		internal static string FormatOutputMessage(string issueKey, string labels)
		{
			var labelsArray = LabelFormatConveter.FromJira(labels).To(array => array);
			return string.Format("Issue '{0}' labels ({1}): '{2}'", issueKey, labelsArray.Count(), LabelFormatConveter.From(labelsArray).ToUI());
		}

		private static IssueField UpdateLabels(RemoteIssue issue, IEnumerable<Action<IList<string>>> labelOperations)
		{
			var newLabels = LabelFormatConveter.From(issue).To(labelArray => new List<string>(labelArray));
			foreach (var operation in labelOperations)
			{
				operation(newLabels);
			}

			return IssueField.CustomField(CustomFieldId.Labels) <= LabelFormatConveter.From(newLabels).ToJira();
		}
		
		private static string LabelFrom(string labelOperation)
		{
			return labelOperation.Substring(1);
		}
	}

	internal class LabelOperations
	{
		public const char Add = '+';
		public const char Remove = '-';
	}
}
