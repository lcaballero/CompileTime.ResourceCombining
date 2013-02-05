using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using CompileTime.ResourceCombining;
using System.IO;
using SimpleLineParser;


namespace CompileTime.Handlers
{
	public class CacheManager
	{
		public static CacheManager Instance { get; private set; }

		/// <summary>
		/// The assets loaded from a generated cache file for this instance of CacheManager.
		/// </summary>
		private CombinedAssets Assets { get; set; }

		/// <summary>
		/// The options used when registering this Manager with global.asax routing.
		/// </summary>
		public CachingOptions Options { get; set; }

		/// <summary>
		/// Map of Hash keys to source code (either js or css).  For lookup off of the query
		/// string when served up, and as added to the DOM.
		/// </summary>
		public Dictionary<string, CacheDefinition> HashToSource { get; private set; }

		/// <summary>
		/// The fully resolved path as key, to the actual source.  Used when looking up paths
		/// to put in the DOM to later be looked up when the browser requests the asset/resource.
		/// </summary>
		public Dictionary<string, CacheDefinition> PackageFileToSource { get; private set; }

		public HomePathResolver Resolver { get; set; }

		public string AppHome { get; set; }

		public CacheManager(CombinedAssets ca, CachingOptions opts)
		{
			Assets = ca;
			Options = opts;
			AppHome = VirtualPathUtility.ToAbsolute("~/");

			HashToSource = ca.Caches.ToDictionary(c => c.Hash, c => c);
			PackageFileToSource = ca.Caches.ToDictionary(c => c.PackageFile, c => c);

			Resolver = new HomePathResolver(ca.ResolvedHomePath);
		}

		/// <summary>
		/// Used in Gloabal.asax to setup this CacheManager to handle paths where the RouteOptions.BasePath
		/// leading directory is used to distinguish the paths
		/// </summary>
		/// <param name="routes"></param>
		/// <param name="opts"></param>
		public static void RegisterDefault(RouteCollection routes, CachingOptions opts = null)
		{
			opts = opts ?? new CachingOptions();

			var path = string.Format("{0}/{{hash}}/{{type}}/{{*path}}", opts.BasePath);

			var mgr = new CacheManager(LoadAssets(opts.CacheFile), opts);

			routes.Insert(0, new Route(path, new CacheHandler(mgr)));

			Instance = mgr;
		}

		/// <summary>
		/// Loads the xml file containing the generated asset cache.
		/// </summary>
		/// <param name="path">
		/// The file location relative to the application root.
		/// </param>
		/// <returns>
		/// The strongly typed instance of Assets after deserialization.
		/// </returns>
		private static CombinedAssets LoadAssets(string path)
		{
			var vp = HttpContext.Current.Server.MapPath(path);
			var xml = File.ReadAllText(vp);

			var ca = xml.FromXml<CombinedAssets>();

			return ca;
		}

		//public IEnumerable<string> ToUncombinedPaths(string appRelativePath)
		//{
		//    var path = Resolver.ResolvePath(appRelativePath);
		//    CacheDefinition src = null;

		//    if (PackageFileToSource.TryGetValue(path, out src))
		//    {
		//        src.Paths.Select(

		//    }
		//    else
		//    {
		//        return new List<string> { "" };
		//    }
		//}

		public IEnumerable<string> ToScriptPaths(string appRelativePath)
		{
			var path = Resolver.ResolvePath(appRelativePath);
			CacheDefinition src = null;

			if (PackageFileToSource.TryGetValue(path, out src))
			{
				yield return
				string.Format(
					"{0}{1}{2}{3}/{4}/{5}/{6}",
					string.IsNullOrEmpty(Options.Protocol)
						? ""
						: Options.Protocol + "://",
					string.IsNullOrEmpty(Options.Host)
						? ""
						: Options.Host,
					AppHome,
					Options.BasePath,
					src.Hash,
					ToPackageType(src.PackageFileName),
					ToHomeRelative(Resolver.Home, path).NormalizePathSeparators('/'));
			}
			else
			{
				yield return "";
			}
		}

		private string ToHomeRelative(string home, string s)
		{
			return
			s.StartsWith(home)
				? s.Substring(home.Length, s.Length - home.Length)
				: s;
		}

		private string ToPath(string s)
		{
			return
			s.StartsWith("~/")
				? s.Substring(2)
				: s;
		}

		private string ToPackageType(string s)
		{
			return
			s.EndsWith(".js.package")
				? "js"
				:
			s.EndsWith(".css.package")
				? "css"
				: "-";
		}
	}
}