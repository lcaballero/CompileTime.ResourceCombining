using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimpleLineParser;
using System.IO;
using CompileTime.ResourceCombining;


namespace CompileTime.ResourceCombining.Tests
{
	[TestFixture]
	public class InfiniteRecursiveIncludesTests : AssertionHelper
	{
		public IPathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Infiinite-Recursive-Including/"));
			}
		}

		private PackageProcessing _Processing = null;
		public PackageProcessing Processing
		{
			get
			{
				return _Processing ?? new PackageProcessing(Resolver, Resolver.MapPackages());
			}
		}

		[Test]
		public void Find_Errors_In_Infinite_Recursive_Package()
		{
			var defs = Processing.ToCacheDefinitions();

			Expect(Processing.Reporting.Errors.Count, Is.GreaterThan(0));
		}

		[Test]
		public void Expected_Amount_Of_Expansion_In_Midst_Of_Infinite_Recursion()
		{
			var paths = Processing
				.ToCacheDefinitions()
				.Find(f => f.PackageFileName == "Include-Other-Packages-2.js.package")
				.Paths;

			Expect(paths.Count, Is.EqualTo(6));
		}
	}
}
