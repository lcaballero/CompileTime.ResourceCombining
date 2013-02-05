using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLineParser;
using System.IO;


namespace CompileTime.ResourceCombining
{
	public class ContentProvider
	{
		/// <summary>
		/// Content is map of Paths to resource file content.  The typical case is where
		/// a package only includes resources like .js and .css files, but other resource
		/// files might also include other .package files.
		/// </summary>
		public Dictionary<string, string> Content { get; set; }

		/// <summary>
		/// An instance of ErrorReporting that can collect processing events
		/// (info, warnings, and errors).
		/// </summary>
		public ErrorReporting Reporting { get; set; }

		public IPathResolver Resolver { get; private set; }

		public ContentProvider(IPathResolver resolver, ErrorReporting reporting)
		{
			Content = new Dictionary<string, string>();
			Resolver = resolver;
			Reporting = reporting;
		}

		/// <summary>
		/// Provides the content of the given file name or an empty string if the file
		/// doesn't exist, but if the file doesn't exist it logs an error to the 
		/// error reporting instance.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to lookup.
		/// </param>
		/// <returns>
		/// The contents of the file or an empty string.
		/// </returns>
		public string this[string filename]
		{
			get
			{
				string src = null;

				if (!Content.TryGetValue(filename, out src))
				{
					src = Content[filename] = ReadContent(filename, Reporting);
				}

				return src;
			}
		}

		/// <summary>
		/// Reads the given file from disk if it exists, otherwise it logs an error,
		/// and returns an empty string.
		/// </summary>
		/// <param name="filename">
		/// Path to a text file containing an asset's text.
		/// </param>
		/// <param name="reporting">
		/// Error collecting class.
		/// </param>
		/// <returns>
		/// The contents of the file, or empty string.
		/// </returns>
		private string ReadContent(string filename, ErrorReporting reporting)
		{
			if (File.Exists(filename))
			{
				return File.ReadAllText(filename);
			}

			var fn = Resolver.ResolvePath(filename);

			if (File.Exists(fn))
			{
				return File.ReadAllText(fn);
			}

			// Have a filename that doesn't exist, so we log it as an error.
			reporting.AddError(
				string.Format("Could not find file: {0}", filename));

			return "";
		}
	}
}
