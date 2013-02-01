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
	public class PackageInsidePackagesTests : AssertionHelper
	{
		public IPathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Package-Including-Package/"));
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
		public void Finds_Packages()
		{
			var defs = Processing.ToCacheDefinitions();

			Expect(defs.Count, Is.EqualTo(4));
		}

		[Test]
		public void Includes_Other_Package_Files()
		{
			var defs = Processing.ToCacheDefinitions();
			
			var package = defs.Find(
				cd =>
				cd.PackageFileName == "Include-Other-Packages-1.js.package");

			// Paths should include the 7 inside a referenced package
			// and 2 in the original package file.
			Expect(package.Paths.Count, Is.EqualTo(9));
		}
	}
}
