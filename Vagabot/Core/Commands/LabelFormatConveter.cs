using System;
using System.Collections.Generic;
using System.Linq;
using Binboo.JiraIntegration;

namespace Binboo.Core.Commands
{
	internal class LabelFormatConveter
	{
		public static TargetConverter From(RemoteIssue issue)
		{
			var labelFieldValue = FromIssue(issue);
			return FromJira(labelFieldValue);
		}

		public static string FromIssue(RemoteIssue issue)
		{
			var customFieldValue = issue.CustomFieldValue(CustomFieldId.Labels);
			return customFieldValue != null && customFieldValue.Length == 1 ? customFieldValue[0] : string.Empty;
		}

		public static TargetConverter From(IEnumerable<string> labels)
		{
			return new TargetConverter(labels);
		}

		public static TargetConverter FromJira(string labels)
		{
			return new TargetConverter(labels.Split(' '));
		}
		
		public static TargetConverter FromUI(string labels)
		{
			return new TargetConverter(labels.Split(',').Select( label => label.Trim()));
		}

	}

	internal class TargetConverter
	{
		internal TargetConverter(IEnumerable<string> source)
		{
			_source = source;
		}
		
		internal T To<T>(Func<IEnumerable<string>, T> func)
		{
			return func(_source);
		}

		internal string ToJira()
		{
			return _source.Aggregate("", (acc, curr) => acc + " " + curr).Trim();
		}
		
		internal string ToUI()
		{
			return string.Join(", ", _source.ToArray());
		}
		
		private IEnumerable<string> _source;
	}
}
