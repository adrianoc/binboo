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
using System.Linq;
using System.Text;
using Binboo.Core;
using Binboo.Core.Commands;
using Binboo.Core.Configuration;
using Binboo.Jira.Integration;

namespace Binboo.Jira.Commands
{
	class PairsCommand : JiraCommandBase
	{
		public PairsCommand(IJiraProxy jira, string help) : base(jira, help)
		{
		}

		public override string Id
		{
			get { return "pairs"; }
		}

		protected override ICommandResult ProcessCommand(IContext context)
		{
			IEnumerator<string> randomUsers = Random(ConfigServices.PairingUsers);
			var msg = new StringBuilder("Pairs: ");

			string pair = NextPair(randomUsers);
			while (null != pair)
			{
				msg.AppendFormat("{0}, ", pair);
				pair = NextPair(randomUsers);
			}

			return CommandResult.Success(msg.Remove(msg.Length - 2, 2).ToString());
		}

		private static string NextPair(IEnumerator<string> randomUsers)
		{
			if (!randomUsers.MoveNext()) return null;

			string user1 = randomUsers.Current;
			if (!randomUsers.MoveNext()) return "[" + user1 + "]";

			return "[" + user1 + ", " + randomUsers.Current + "]";
		}

		private IEnumerator<string> Random(IEnumerable<string> enumerable)
		{
			List<string> items = enumerable.ToList();
			Random rand = new Random(Environment.TickCount + GC.CollectionCount(1));

			IList<int> alreadyReturned = new List<int>();
			int i = 0;
			while(i < items.Count)
			{
				int next = rand.Next(0, items.Count);
				if (!alreadyReturned.Contains(next))
				{
					i++;
					alreadyReturned.Add(next);
					yield return DevNameFor(items[next]);
				}
			}
		}

		private static string DevNameFor(string jiraName)
		{
			int separatorIndex = jiraName.IndexOfAny(new[] {' ', '@'});
			return separatorIndex > 0 ? jiraName.Substring(0, separatorIndex) : jiraName;
		}
	}
}