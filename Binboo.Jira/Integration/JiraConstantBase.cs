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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Binboo.Jira.Integration
{
	/// <summary>
	/// Base type for Jira constants
	/// </summary>
	/// <typeparam name="JT">Jira type for the constant.</typeparam>
	/// <typeparam name="D">Constant type (class deriving from JiraConstantBase<JT,D>)</typeparam>
	public abstract class JiraConstantBase<JT, D> where D : JiraConstantBase<JT, D>
	{
		public string Id
		{
			get { return _id; }
		}

		public string Description
		{
			get { return _description; }
		}

		public static IEnumerable<string> FriendlyNames()
		{
			foreach (var item in _idToConstantMap.Values)
			{
				yield return item.Description.ToLower();
			}
		}

		public static D Parse(string friendlyName)
		{
			foreach(D item in _idToConstantMap.Values)
			{
				if (string.Compare(item.Description, friendlyName, true) == 0) return item;
			}

			throw new ArgumentException(string.Format("No constant found for name '{0}'",  friendlyName));
		}

		public static string FriendlyNameFor(string id)
		{
			if (!_idToConstantMap.ContainsKey(id))
			{
				throw new ArgumentException(string.Format("No {0} constant for id {1}", typeof(JT).Name, id), "id");
			}

			return _idToConstantMap[id].Description;
		}

		public static implicit operator string(JiraConstantBase<JT, D> status)
		{
			return status.Id;
		}

		public override string ToString()
		{
			return GetType().Name + "(" + _description + ":" + _id + ")";
		}

		internal JiraConstantBase(string id, string description)
		{
			_description = FormatDescription(description);
			_id = id;

			_idToConstantMap.Add(id, This());
		}

		protected abstract D This();

		private static string FormatDescription(string description)
		{
			StringBuilder sb = new StringBuilder(description);
			foreach (Match lowerCaseFirstLetter in Regex.Matches(description, @"\b[a-z]"))
			{
				sb[lowerCaseFirstLetter.Index] = char.ToUpper(sb[lowerCaseFirstLetter.Index]);
			}
			return sb.ToString();
		}

		protected static void InitializeBase(IEnumerable<JT> constants)
		{
			foreach (var constantPair in _initializationList)
			{
				constantPair.Value.PopulateFrom(constants);
			}

			_initializationList.Clear();
			_initializationList = null;
		}

		protected static void ToBeInitialized<R>(Expression<Func<R, R>> tbi, Func<string, IEnumerable, object> instantiator) where R : JiraConstantBase<JT, D>
		{
			ToBeInitialized(tbi, MemberInfoFor(tbi).Name, instantiator);
		}

		protected static void ToBeInitialized<R>(Expression<Func<R, R>> tbi, string name, Func<string, IEnumerable, object> instantiator) where R : JiraConstantBase<JT, D>
		{
			InitializationList().Add(name, ConstantInfoFor(name, tbi, instantiator));
		}

		private static IDictionary<string, ConstantInfo> InitializationList()
		{
			if (_initializationList == null)
			{
				_initializationList = new Dictionary<string, ConstantInfo>();
			}

			return _initializationList;
		}

		private static ConstantInfo ConstantInfoFor<R>(string friendlyName, Expression<Func<R, R>> tbi, Func<string, IEnumerable, object> instantiator)
		{
			return new ConstantInfo(friendlyName, FieldInfoFor(MemberInfoFor(tbi)), instantiator);
		}

		private static FieldInfo FieldInfoFor(MemberInfo member)
		{
			return member.ReflectedType.GetField(member.Name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
		}

		private static MemberInfo MemberInfoFor<R>(Expression<Func<R, R>> tbi)
		{
			MemberExpression me = (MemberExpression)tbi.Body;
			return me.Member;
		}

		private readonly string _id;
		private readonly string _description;

		private static readonly IDictionary<string, D> _idToConstantMap = new Dictionary<string,  D>();
		private static IDictionary<string, ConstantInfo> _initializationList;

		private class ConstantInfo
		{
			public ConstantInfo(string friendlyName, FieldInfo field, Func<string, IEnumerable, object> instantiator)
			{
				_field = field;
				_instantiator = instantiator;
				_friendlyName = friendlyName;
			}

			public void PopulateFrom(IEnumerable constants)
			{
				_field.SetValue(null, _instantiator(_friendlyName, constants));
			}

			private readonly FieldInfo _field;
			private readonly Func<string, IEnumerable, object> _instantiator;
			private readonly string _friendlyName;
		}
	}
}

