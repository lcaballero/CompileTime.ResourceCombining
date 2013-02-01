using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLineParser;
using System.IO;
using Packages = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;
using CompileTime.ResourceCombining.Helpers;


namespace CompileTime.ResourceCombining
{
	public class PackageProcessing
	{
		/// <summary>
		/// Packages maps the Full Path of a .package file to paths of included resource
		/// files as found in that file.  These paths are typically .js or .css files,
		/// but somethings these files are also other .package files which can then be
		/// included directly into the list of that package.
		/// </summary>
		public Packages Packages { get; private set; }

		/// <summary>
		/// An instance of ErrorReporting that can collect processing events
		/// (info, warnings, and errors).
		/// </summary>
		public ErrorReporting Reporting { get; set; }

		/// <summary>
		/// An instance of ContentProvider that is a Facad over reading asset files from
		/// disk or off the network (and possibly provides caching).
		/// </summary>
		public ContentProvider Content { get; set; }

		public PackageProcessing(IPathResolver resolver, Packages packages)
		{
			Reporting = new ErrorReporting();

			// Process after setting up Info, Warn, and Errs
			ProcessPackage(packages, Reporting);

			Content = new ContentProvider(resolver, Reporting);

			Packages = ProcessPackageReferences(packages, Reporting, new HashSet<string>());
		}

		private static void ProcessPackage(Packages value, ErrorReporting reporting)
		{
			// Check for an empty file, where no resources/assets are listed.
			if (value.Count <= 0)
			{
				reporting.AddWarning("Package contains 0 assets");
			}

			// Check that duplicate paths are reported as warnings.
			value
				.Select(
					s =>
					new
					{
						Package = s.Key,
						Dupes = value[s.Key].Duplicates().ToList()
					})
				.Where(s => s.Dupes.Count > 0)
				.ToList()
				.ForEach(
					s =>
					reporting.AddWarning(
						string.Format("Package: {0} contains a duplicate asset paths:\n {1}",
							s.Package,
							string.Join("\n\t", s.Dupes.ToArray()))));

			// Check for infinite recursive includes of packages.
		}

		private static Packages ProcessPackageReferences(
			Packages packages,
			ErrorReporting reporting,
			HashSet<string> packagesProcessedAlready)
		{
			return
			packages.ToDictionary(
				s => s.Key,
				s =>
				ExpandAssets(s.Key, packages, null, reporting)
					.Distinct()
					.ToList());
		}

		private static IEnumerable<string> ExpandAssets(
			string referencingFile,
			Packages packages,
			HashSet<string> alreadyExpanded,
			ErrorReporting reporting)
		{
			return
			packages.ContainsKey(referencingFile)
				? packages[referencingFile]
					.SelectMany(
						packageFile =>
						packageFile.EndsWith(".package")
							? ExpandPackage(
								referencingFile,
								packageFile,
								packages,
								alreadyExpanded ?? new HashSet<string> { referencingFile },
								reporting)
							: IncludeAsset(packageFile))
				: new List<string>();
		}

		/// <summary>
		/// This function is a convienence to provided the same return type for a single value
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private static IEnumerable<string> IncludeAsset(string s)
		{
			yield return s;
		}

		/// <summary>
		/// Given that s is a .package path this will return the list of files
		/// for that package after recursively expanding that .package file.
		/// </summary>
		/// <param name="referencingFile">
		/// The package where this the assetFile is referenced.
		/// </param>
		/// <param name="packageFile">
		/// A .package file.
		/// </param>
		/// <param name="packages">
		/// The set of packages being processed.
		/// </param>
		/// <returns>
		/// List of assets for the package s, where every .package has been expanded.
		/// </returns>
		private static IEnumerable<string> ExpandPackage(
			string referencingFile,
			string packageFile,
			Packages packages,
			HashSet<string> alreadyExpanded,
			ErrorReporting reporting)
		{
			// Have 2 cases:
			//	case 1: packages contains reference.
			//	case 2: packages DOES NOT contain reference.
			if (packages.ContainsKey(packageFile))
			{
				if (alreadyExpanded.Contains(packageFile))
				{
					reporting.AddError(
						string.Format(
						"Already included package:"
						+ "\n\t{0} and including it again would cause infinite recursion."
						+ "\n\tNo further expansion done.",
						packageFile));

					return new List<string>();
				}
				else
				{
					alreadyExpanded.Add(packageFile);

					return ExpandAssets(packageFile, packages, alreadyExpanded, reporting);
				}
			}
			else
			{
				reporting.AddWarning(
					string.Format(
					"Could not find package: {0}, as referenced in {1}",
					packageFile,
					referencingFile));
					
				return new List<string>();
			}
		}

		
	}
}
