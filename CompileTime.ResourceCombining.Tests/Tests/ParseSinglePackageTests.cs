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
	public class ParseSinglePackageTests : AssertionHelper
	{
		private const int AssetPathCount = 32;
		private const int UniqueAssetCount = 31;
		private const int DupeCount = 1;
		private const int PackageCount = 1;

		public IPathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Single-Package/"));
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
		public void Finds_One_Package()
		{
			var list = Resolver.MapPackages();

			Expect(list.Count, Is.EqualTo(PackageCount));
		}

		[Test]
		public void Package_Lines()
		{
			var list = Resolver.MapPackages().ToList();

			// non-comment and non-empty lines in the one .package file.
			Expect(list[0].Value.Count, Is.EqualTo(AssetPathCount));
		}

		[Test]
		public void File_Does_Not_Exist_Produces_Empty_List()
		{
			var resolver = new HomePathResolver(Guid.NewGuid().ToString());

			var list = resolver.MapPackages().ToList();

			Expect(list, Is.Not.Null);
			Expect(list.Select(s => s.Key).FirstOrDefault(), Is.Null);
			Expect(list.Select(s => s.Value).FirstOrDefault(), Is.Null);
		}

		[Test]
		public void Package_Source_When_Cannot_Find_Files()
		{
			var defs = Processing.ToCacheDefinitions().FirstOrDefault();

			Expect(defs, Is.Not.Null);
		}

		[Test]
		public void Package_Handles_Dupe_Assets()
		{
			var p = Processing;

			// Should get warning that dupe exists.
			Expect(p.Reporting.Warnings.Count, Is.GreaterThan(0));
		}

		[Test]
		public void Package_Source_Paths()
		{
			var defs = Processing.ToCacheDefinitions().FirstOrDefault();

			Expect(defs.Paths.Count, Is.EqualTo(UniqueAssetCount));
		}

		[Test]
		public void Combined_Source_Without_Files_Causes_Errors()
		{
			var p = Processing;
			var defs = p.ToCacheDefinitions().FirstOrDefault();
			var src = defs.FullRawSource;

			Expect(p.Reporting.Errors.Count, Is.GreaterThan(0));
			Expect(p.Reporting.Errors.Count, Is.EqualTo(UniqueAssetCount));
		}

		[Test]
		public void Warnings_For_Empty_Package()
		{
			var resolver =
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Empty-Package/"));

			var p = new PackageProcessing(resolver, resolver.MapPackages());

			Expect(p.Reporting.Warnings.Count, Is.GreaterThan(0));
		}
	}
}
