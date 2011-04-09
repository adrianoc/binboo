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
using System.Security;
using System.Security.Permissions;

namespace Binboo.Core.CustomFilters
{
	public class PredicateServices
	{
		public static DisposableCustomFilterPredicate<T> Compile<T>(string paramName, string source)
		{
			var domain = NewSandboxedAppDomain();

			var assemblyPath = EmitCustomFilterAssembly<T>(paramName, source, domain);

			var predicate = NewPredicateInstance<T>(domain);

			return DisposablePredicateFor(predicate, domain, assemblyPath);
		}

		private static ICustomFilterPredicate<T> NewPredicateInstance<T>(AppDomain domain)
		{
			return (ICustomFilterPredicate<T>) domain.CreateInstanceAndUnwrap(CustomPredicateAssembly, CustomFilterTypeName);
		}

		private static string EmitCustomFilterAssembly<T>(string paramName, string source, AppDomain domain)
		{
			return new CustomPredicateFactory<T>(CustomPredicateAssembly, paramName, source).EmitAssemly(domain.DynamicDirectory);
		}

		private static AppDomain NewSandboxedAppDomain()
		{
			var domainSetup = new AppDomainSetup
			                  	{
			                  		ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
			                  		ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
			                  		ApplicationName = "Binboo.CustomFilter",
			                  		DynamicBase = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks.ToString())
			                  	};

			return AppDomain.CreateDomain("BinbooPredicate", null, domainSetup, SandboxedPermissionSet());
		}

		private static PermissionSet SandboxedPermissionSet()
		{
			var permissionSet = new PermissionSet(null);

			permissionSet.SetPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
			permissionSet.SetPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			return permissionSet;
		}

		private static DisposableCustomFilterPredicate<T> DisposablePredicateFor<T>(ICustomFilterPredicate<T> predicate, AppDomain domain, string assemblyPath)
		{
			return new DisposableCustomFilterPredicate<T>(predicate, domain, assemblyPath);
		}

		const string CustomPredicateAssembly = "CustomPredicateAssembly";
		const string CustomFilterTypeName = "CustomFilterPredicate";
	}
}
