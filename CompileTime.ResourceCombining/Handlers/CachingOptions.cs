using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompileTime.Handlers
{
	public class CachingOptions
	{
		/// <summary>
		/// The leading path name for the cache handler routes so that script or style paths
		/// begin with this.  For instance, if the BasePath were AssetPath then the url to
		/// resources as found in the dom would look something like this:
		/// 
		/// http://localhost/base-path/generated-hash-key/actual-name.type.package
		/// 
		/// So that if the BasePath needed to have more directories it could, and routes to
		/// assets would be registered using that BasePath.
		/// </summary>
		public string BasePath { get; set; }

		/// <summary>
		/// File path relative to the application root that stores the generated asset 
		/// cache xml file.
		/// </summary>
		public string CacheFile { get; set; }

		public string Host { get; set; }

		public string Protocol { get; set; }

		/// <summary>
		/// Default part o the base path, where base path appears in the URL path.
		/// </summary>
		public const string DefaultBasePath = "Assets";

		/// <summary>
		/// The path to the file where the content cache lives and from which an 
		/// cache handler can load the generated file, process it for lookups, and
		/// then serve files to pages with the contents as generated and placed
		/// in that file.
		/// </summary>
		public const string DefaultCacheFile = "~/Content/Caches/combined-resources.cache.xml";

		public CachingOptions()
		{
			BasePath = DefaultBasePath;
			CacheFile = DefaultCacheFile;
		}
	}
}