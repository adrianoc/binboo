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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Binboo.Jira.Commands;

namespace Binboo.Jira.Configuration
{
	[XmlRoot("link")]
	public class LinkConfiguration
	{
		[XmlArrayItem("alias", Type = typeof(LinkAlias))]
		[XmlArray("description-aliases")]
		public List<LinkAlias> Aliases
		{
			get { return _aliases; }
		}

		public LinkAlias AliasFor(string original)
		{
			var originalLowerCase = original.ToLowerInvariant();
			var found = _aliases.Find(candidate => candidate.Original.ToLowerInvariant() == originalLowerCase);
			
			return found ?? LinkAlias.NullAlias;
		}

		public override string ToString()
		{
			return string.Format("Aliases({0})", 
								_aliases.Aggregate("", (acc, current) => string.Format("{0}, [{1}]", acc, current), agg => agg.Remove(0, 2))
								);
		}

		private List<LinkAlias> _aliases = new List<LinkAlias>();
	}
}
