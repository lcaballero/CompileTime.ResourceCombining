using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimpleLineParser;
using System.IO;
using CompileTime.ResourceCombining;
using CompileTime.ResourceCombining.Helpers;


namespace CompileTime.ResourceCombining.Tests
{
	[TestFixture]
	public class CombinedFileHashingTests : AssertionHelper
	{
		public IPathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Multi-Source-Package/"));
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
		public void Multiple_Package_Have_Unique_Hashes()
		{
			var hashes = Processing.ToCacheDefinitions().Select(d => d.Hash).ToList();
			var dupes = hashes.Duplicates().ToList();

			Expect(hashes.Count, Is.GreaterThan(2));
			Expect(dupes.Count, Is.EqualTo(0));
		}
	}
}
