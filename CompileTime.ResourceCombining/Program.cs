using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using SimpleLineParser;
using CompileTime.ResourceCombining;


namespace CompileTimeResourceCombining
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var home = new HomePathResolver(
				Path.Combine(Environment.CurrentDirectory, "../../Test-Files/Client"));

			var jsAndCss = ParsePackages.MapCssJsList(home);
			var packages = ParsePackages.MapPackages(home);

			packages
				.ToList()
				.ForEach(
					s =>
					Console.WriteLine(
						"Name: {0}, Size: {1}",
						Path.GetFileName(s.Key),
						s.Value.Count));
		}
	}
}
