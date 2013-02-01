using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLineParser;
using System.IO;
using CompileTime.ResourceCombining;

namespace CompileTimeCombining
{
	public class Program
	{
		public IPathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(Environment.CurrentDirectory, "../../../CompileTimeResourceCombining.Tests/Test-Files/Client/"));
			}
		}

		public static void Main(string[] args)
		{
			new Program().Run();
		}

		public void Run()
		{
			var resolver = Resolver;
			var p = new PackageProcessing(resolver, resolver.MapPackages());
			var defs = p.ToCacheDefinitions();

			var cache =
				new CombinedAssets
				{
					Caches = defs
				};

			File.WriteAllText("combined-resource.cache.xml", cache.ToXml());
		}
	}
}
