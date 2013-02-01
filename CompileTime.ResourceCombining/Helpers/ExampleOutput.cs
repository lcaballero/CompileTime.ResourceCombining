using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompileTime.ResourceCombining.Helpers
{
	public static class ExampleOutput
	{
		public static void ToConsole()
		{
			var s = Assets().ToXml();

			Console.WriteLine(s);
		}

		private static CombinedAssets Assets()
		{
			var assets = new CombinedAssets
			{
				Caches = new List<CacheDefinition>
				{
					new CacheDefinition
					{
						ActionName = "Action",
						ActionPath = "~/Controller/Action",
						ControllerName = "Controller",
						Hash = "aslkdbjaslkjuo0-2374123l12o4",
						Paths = new List<string>
						{
							"~/Content/Scripts/Path-1.js",
							"~/Content/Scripts/Path-2.js",
						},
						FullRawSource = @"

// Path-1.js

... // code for path 1

// Path-2.js

... // code for path 2

"
					}
				}
			};

			return assets;
		}
	}
}
