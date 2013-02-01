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
	public class CombiningFilesTests : AssertionHelper
	{
		public IPathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Near-Empty-Source-Package/"));
			}
		}

		public PackageProcessing Processing
		{
			get
			{
				return
				new PackageProcessing(Resolver, Resolver.MapPackages());
			}
		}

		[Test]
		public void Combined_Source_With_Files_Causes_No_Errors()
		{
			var p = Processing;
			var defs = p.ToCacheDefinitions().FirstOrDefault();
			var src = defs.FullRawSource;

			Expect(p.Reporting.Errors.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_Package_In_Directory()
		{
			var p = Processing;
			var defs = p.ToCacheDefinitions().FirstOrDefault();
			var src = defs.FullRawSource;

			Expect(p.Packages.Count, Is.EqualTo(1));
		}

		[Test]
		public void All_Files_In_Package_Are_In_Combined_Source()
		{
			var p = Processing;
			var defs = p.ToCacheDefinitions().FirstOrDefault();
			var src = defs.FullRawSource;

			Expect(defs.Paths.All(
				c =>
				src.Code.Contains(Path.GetFileName(c))));
		}


		[Test]
		public void Creates_MD5_For_Combined_Source()
		{
			var p = Processing;
			var defs = p.ToCacheDefinitions().FirstOrDefault();
			var src = defs.FullRawSource;

			Expect(defs.Hash, Is.Not.Empty);
			Expect(defs.Hash.Length, Is.EqualTo(32));
		}
	}
}
