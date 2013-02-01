using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using SimpleLineParser;


namespace CompileTime.ResourceCombining
{
	public static class DirectorySearching
	{
		public static IEnumerable<string> Paths(string root, Func<string, bool> acceptFn)
		{
			return
			Directory.Exists(root)
				? Directory
					.GetFiles(root)
					.Where(acceptFn)
					.Concat(
						Directory.Exists(root)
							? Directory.GetDirectories(root)
								.SelectMany(d => Paths(d, acceptFn))
							: new List<string>())
				: new List<string>();
		}
	}
}
