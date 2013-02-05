using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using CompileTime.ResourceCombining;
using System.IO;


namespace CompileTime.Handlers
{
	public class CacheHandler : IHttpHandler, IRouteHandler
	{
		public static readonly HashSet<string> TypeSet = new HashSet<string> { "css", "js" };

		public bool IsReusable { get { return false; } }

		public CacheManager Cache { get; private set; }

		public CacheHandler(CacheManager combined)
		{
			Cache = combined;
		}

		public void ProcessRequest(HttpContext context)
		{
			var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

			var req = new CacheRequest
			{
				Hash = (routeData.Values["hash"] ?? "").ToString(),
				Type = (routeData.Values["type"] ?? "").ToString(),
				Path = (routeData.Values["path"] ?? "").ToString()
			};

			if (TypeSet.Contains(req.Type))
			{
				ProcessFile(context, routeData, req);
			}
			else
			{
				context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
				context.Response.StatusCode = 403;
				context.Response.Write("Invalid Request");
			}
		}

		public void ProcessFile(HttpContext context, RouteData routeData, CacheRequest req)
		{
			CacheDefinition src = null;

			if (Cache.HashToSource.TryGetValue(req.Hash, out src))
			{
				// Set Headers
				var ext =
					req.Path.EndsWith(".js.package")
						? ".js"
						:
					req.Path.EndsWith(".css.pacakge")
						? ".css"
						: Path.GetExtension(req.Path);

				switch (ext)
				{
					case ".png":
					case ".jpg":
					case ".jpeg":
					case ".gif":
						// Determine we need additional headers
						// Transmit file here.
						break;
					case ".js":
					case ".css":
						context.Response.Write(src.FullRawSource.CombinedSource[0].InnerText);
						return;
					default:
						return;
				}
			}
			else
			{
				context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
				context.Response.StatusCode = 403;
				context.Response.Write("Invalid Request");
			}
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return this;
		}
	}
}