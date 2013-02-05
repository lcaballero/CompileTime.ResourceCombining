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
		public HomePathResolver Resolver
		{
			get
			{
				return
				new HomePathResolver(
					Path.Combine(
						Environment.CurrentDirectory,
						@"C:\Projects\Playground\LucasCaballero\CacheHandler\CacheHandler\"));
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
					ResolvedHomePath = resolver.Home,
					Caches = defs
				};

			File.WriteAllText(
				@"C:\Projects\Playground\LucasCaballero\CacheHandler\CacheHandler\Content\Caches\combined-resources.cache.xml",
				cache.ToXml());

			var rc =
				new ResourceList
				{
					ResolvedHomePath = resolver.Home,
					Resources = p.ToResources()
				};


			File.WriteAllText(
				@"C:\Projects\Playground\LucasCaballero\CacheHandler\CacheHandler\Content\Caches\resource-list.cache.xml",
				rc.ToXml());
		}
	}
}
