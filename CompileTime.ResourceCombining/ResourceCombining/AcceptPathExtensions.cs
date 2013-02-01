using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompileTime.ResourceCombining
{
	public static class Accept
	{
		public static readonly Func<string, bool> CssOrJs = Any(Css, Js);
		public static readonly Func<string, bool> CssOrJsPacakges = Any(CssPackage, JsPackage);

		public static Func<string, bool> Any(params Func<string, bool>[] args)
		{
			return (s) => args.Any(a => a(s));
		}

		public static bool NonEmpty(this string path)
		{
			return !string.IsNullOrEmpty(path.Trim());
		}

		public static bool Css(this string path)
		{
			return path.EndsWith(".css");
		}

		public static bool Js(this string path)
		{
			return path.EndsWith(".js");
		}

		public static bool JsPackage(this string path)
		{
			return path.EndsWith(".css.package");
		}

		public static bool CssPackage(this string path)
		{
			return path.EndsWith(".js.package");
		}
	}
}
