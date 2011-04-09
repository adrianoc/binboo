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
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace Binboo.Core.CustomFilters
{
	internal class CustomPredicateFactory<T>
	{
		public CustomPredicateFactory(string assemblyName, string paramName, string source)
		{
			_assemblyName = assemblyName;
			_paramName = paramName;
			_source = source;
		}

		public string EmitAssemly(string targetFolder)
		{
			Directory.CreateDirectory(targetFolder);
			
			var predicateFactorySource = PreparePredicateFactorySource();

			CompilerParameters parameters = CompilerParametersFor(targetFolder);

			var provider = CodeDomProvider.CreateProvider("c#");
			var result = provider.CompileAssemblyFromSource(parameters, predicateFactorySource);

			if (result.Errors.Count > 0)
			{
				var output = CollectErrors(result);
				throw new ArgumentException(output.ToString());
			}

			return parameters.OutputAssembly;
		}

		private static StringBuilder CollectErrors(CompilerResults result)
		{
			var output = new StringBuilder();
			foreach (var error in result.Errors)
			{
				output.AppendLine(error.ToString());
			}
			return output;
		}

		private CompilerParameters CompilerParametersFor(string targetFolder)
		{
			var parameters = new CompilerParameters
			                 	{
			                 		GenerateInMemory = false,
			                 		OutputAssembly = Path.Combine(targetFolder, _assemblyName + ".dll")
			                 	};

			AddReferences(parameters);
			return parameters;
		}

		private void AddReferences(CompilerParameters parameters)
		{
			parameters.ReferencedAssemblies.Add("System.dll");
			parameters.ReferencedAssemblies.Add(Path.GetFileName(typeof(CustomPredicateFactory<>).Assembly.Location));
			parameters.ReferencedAssemblies.Add(Path.GetFileName(Assembly.GetCallingAssembly().Location));
			parameters.ReferencedAssemblies.Add(Path.GetFileName(typeof(T).Assembly.Location));
		}

		private string PreparePredicateFactorySource()
		{
			var predicateSource = PredicateTemplate.Replace("%T%", typeof(T).FullName);
			predicateSource = predicateSource.Replace("%pn%", _paramName);
			predicateSource = predicateSource.Replace("%cod%", MakeCSharpCompatible(_paramName, _source));
			return predicateSource;
		}

		private static string MakeCSharpCompatible(string paramName, string original)
		{
			string converted = original.Replace("@" + paramName, paramName).Replace('\'', '"');
			return converted;
		}

		private readonly string _assemblyName;
		private readonly string _paramName;
		private readonly string _source;

		private const string  PredicateTemplate = @"using System;
                                           using Binboo.Core.CustomFilters;
										   sealed class CustomFilterPredicate : MarshalByRefObject, ICustomFilterPredicate<%T%>
										   {
												static CustomFilterPredicate()
												{
												}

												public CustomFilterPredicate()
												{
												}

												public bool Include(%T% %pn%)
												{
													return %cod%;
												}
											}";

	}
}
