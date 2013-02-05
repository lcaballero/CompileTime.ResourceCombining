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
	public static class ParsePackages
	{
		public static Dictionary<string, string> ReadPackages(this Dictionary<string, string> packages)
		{
			return
			packages.ToDictionary(
				s => s.Key,
				s => File.ReadAllText(s.Key));
		}

		public static Dictionary<string, List<string>> MapCssJsList(this IPathResolver home)
		{
			var content_dir = home.ResolvePath("~/Content");

			var paths = DirectorySearching
				.Paths(content_dir, Accept.CssOrJs)
				.ToDictionary(
					s => s,
					// TODO: resolve any nested paths (packages pointing at packages).
					// TODO: handle any infinite recursion.
					s => LinesReader.Parse(s)
						.Select(p => home.ResolvePath(p))
						.Select(p => Path.GetFullPath(p))
						.ToList());

			return paths;
		}

		public static Dictionary<string, List<string>> MapPackages(this IPathResolver home)
		{
			var content_dir = home.ResolvePath("~/Content");

			var paths = DirectorySearching
				.Paths(content_dir, Accept.CssOrJsPacakges)
				.ToDictionary(
					s => Path.GetFullPath(s),
					// TODO: resolve any nested paths (packages pointing at packages).
					// TODO: handle any infinite recursion.
					s => LinesReader.Parse(s)
						.Select(p => home.ResolvePath(p))
						.Select(p => Path.GetFullPath(p))
						.ToList());

			return paths;
		}

		public static List<CacheDefinition> ToCacheDefinitions(this PackageProcessing processing)
		{
			return
			processing
				.Packages
				.Select(p => p.Key.ToCache(p.Value, processing))
				.ToList();
		}

		public static CacheDefinition ToCache(this string key, List<string> paths, PackageProcessing processing)
		{
			return key.ToCacheDefintion(paths, paths.RawSource(processing));
		}

		private static CacheDefinition ToCacheDefintion(this string filename, List<string> paths, string fullSource)
		{
			return
			new CacheDefinition(filename, paths, fullSource)
			{
				Hash = fullSource.Md5RawSource()
			};
		}

		/// <summary>
		/// Uses the PackageProcessing to lookup the filenames and map them to the source code,
		/// which if the file doesn't exist the PacakgeProcessing will log the error for later
		/// surfacing.
		/// </summary>
		/// <param name="files">
		/// The list of filenames where the path is already resolved.
		/// </param>
		/// <param name="processing">	
		/// A package processing instance that can log errors and resolve filenames to file
		/// source code.
		/// </param>
		/// <returns>
		/// The source code with all the code from all of the files provided (when a file can
		/// be found) else an empty string for those files.
		/// </returns>
		public static string RawSource(this List<string> files, PackageProcessing processing)
		{
			return
			files.Aggregate(
					new StringBuilder(),
					(acc, name) => acc.Append(processing.Content[name]))
				.ToString();
		}

		public static string Md5RawSource(this string source)
		{
			return source.ToUtf8Bytes().ToMD5Hash();
		}

		public static List<Resource> ToResources(this PackageProcessing p)
		{
			return
			p.Content.Content
				.Select(
					a =>
					new Resource
					{
						FullRawSource = p.Content[a.Key],
						Hash = p.Content[a.Key].Md5RawSource(),
						Path = a.Key
					})
				.ToList();
		}
	}
}
