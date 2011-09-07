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
using System.IO;
using Binboo.Core.CustomFilters;
using NUnit.Framework;

namespace Binboo.Core.Tests.Tests.CustomFilters
{
	[TestFixture]
	public class CustomFilterPredicateTestCase
	{
		[Test]
		public void TestSimple()
		{
			var textPredicate = "@item.Name.Contains('no ca')";
			using (var pred = PredicateServices.Compile<Subject>("item", textPredicate))
			{
				Assert.IsTrue(pred.Include(new Subject("adriano carlos verona")));
				Assert.IsFalse(pred.Include(new Subject("John")));
			}
		}
		
		[Test]
		[ExpectedException(typeof(TypeInitializationException))]
		public void TestInnerClassInjection()
		{
			const string textPredicate = @" new Evil();
												}

												private static Evil evil = new Evil();
											}
											
											class Evil
											{
												public Evil()
												{
													%FILE-WRITE%;
												}

												public static implicit operator bool(Evil e)
												{
													return true";

			AssertIOIsNotAllowedForPredicates("InjectingStaticConstructorOnBaseClass", textPredicate);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void TestPredicateClassConstructors()
		{
			const string textPredicate = @" new Evil();
												}

												static CustomFilterPredicate()
												{
													%FILE-WRITE%;
												}

												public CustomFilterPredicate()
												{
													%FILE-WRITE%;
												}
											}
											
											class Evil
											{
												public static implicit operator bool(Evil e)
												{
													return true";

			AssertIOIsNotAllowedForPredicates("InjectingStaticConstructorOnBaseClass", textPredicate);
		}

		[Test]
		public void TestImplicitBoolOperator()
		{
			const string textPredicate = @" new Evil();
												}												
											}
											
											class Evil
											{
												public static implicit operator bool(Evil e)
												{
													%FILE-WRITE%;
													return true";

			AssertIOIsNotAllowedForPredicates("ImplicitBoolOperator", textPredicate);
		}

		[Test]
		public void TestStaticConstructors()
		{
			AssertIOIsNotAllowedForPredicates("StaticConstructors", 
													@" new Evil();
												}												
											}
											
											class Evil
											{
												static Evil()
												{
													%FILE-WRITE%;
												}

												public static implicit operator bool(Evil e)
												{
													return true");
		}

		private void AssertIOIsNotAllowedForPredicates(string trickName, string trickCode)
		{
			var testFilePath = Path.Combine(Path.GetTempPath(), trickName + DateTime.Now.Ticks);

			bool ioSucceeded = false;
			var watcher = new FileSystemWatcher(Path.GetDirectoryName(testFilePath));
			watcher.Created += (send, args) => ioSucceeded = !ioSucceeded && args.FullPath == testFilePath;

			watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.LastAccess;
	
			trickCode = trickCode.Replace("%FILE-WRITE%",
			                               @"System.IO.File.WriteAllText(""" + testFilePath.Replace("\\", "\\\\") + @""", ""content"")");

			bool included = CompileAndRunCustomFilter(watcher, trickCode);
			if (File.Exists(testFilePath))
			{
				File.Delete(testFilePath);
				Assert.Fail("Custom filters should not be able to create files.");
			}

			Assert.False(ioSucceeded, "Custom filter code should not be able to write to files");
			Assert.False(included);
		}

		private bool CompileAndRunCustomFilter(FileSystemWatcher watcher, string trickCode)
		{
			watcher.EnableRaisingEvents = true;
			bool included;
			using (var pred = PredicateServices.Compile<Subject>("item", trickCode))
			{
				included = pred.Include(new Subject("adriano carlos verona"));
				watcher.EnableRaisingEvents = false;
			}
			return included;
		}
	}

	[Serializable]
	public class Subject
	{
		private readonly string _name;

		public Subject(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}
}
