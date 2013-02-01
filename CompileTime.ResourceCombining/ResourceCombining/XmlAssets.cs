using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;


namespace CompileTime.ResourceCombining
{
	[XmlRoot("combined-assets")]
	public class CombinedAssets
	{
		[XmlArray("caches")]
		[XmlArrayItem("cache-definition")]
		public List<CacheDefinition> Caches { get; set; }

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return
			obj is CombinedAssets
				&& new HashSet<CacheDefinition>(Caches).IsSubsetOf(
					new HashSet<CacheDefinition>(((CombinedAssets)obj).Caches));
		}
	}

	public class Source
	{
		[XmlIgnore]
		public string Code { get; set; }

		[XmlText]
		public XmlNode[] CombinedSource
		{
			get
			{
				var dummy = new XmlDocument();
				return new XmlNode[] { dummy.CreateCDataSection(Code) };
			}
			set
			{
				if (value == null)
				{
					Code = null;
					return;
				}

				if (value.Length != 1)
				{
					throw new InvalidOperationException(
						string.Format(
							"Invalid array length {0}", value.Length));
				}

				Code = value[0].Value;
			}
		}

		public static implicit operator Source(string cdata)
		{
			var dummy = new XmlDocument();
			var nodes = new XmlNode[] { dummy.CreateCDataSection(cdata) };

			return
			new Source
			{
				CombinedSource = nodes
			};
		}
	}

	public class CacheDefinition
	{
		public CacheDefinition() { }

		public CacheDefinition(string filename, List<string> paths, string fullSource)
		{
			PackageFile = filename;
			Paths = paths;
			FullRawSource = fullSource;
		}

		[XmlArray("assets")]
		[XmlArrayItem("path")]
		public List<string> Paths { get; set; }

		[XmlIgnore]
		public string PackageFileName
		{
			get { return Path.GetFileName(PackageFile); }
		}

		[XmlElement("package-file")]
		public string PackageFile { get; set; }

		[XmlElement("action-path")]
		public string ActionPath { get; set; }

		[XmlElement("controller-name")]
		public string ControllerName { get; set; }

		[XmlElement("action-name")]
		public string ActionName { get; set; }

		[XmlElement("hash")]
		public string Hash { get; set; }

		[XmlElement("raw-source")]
		public Source FullRawSource { get; set; }

		public override int GetHashCode()
		{
			return 
			string.Format(
				"{0}, {1}, {2}, {3}, {4}, {5}",
				string.Join(",", (Paths ?? new List<string>()).ToArray()),
				ActionPath,
				ControllerName,
				ActionName,
				Hash,
				FullRawSource == null ? "" : FullRawSource.Code.ToString()).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CacheDefinition))
			{
				return false;
			}

			var rhs = (obj as CacheDefinition);

			return
			Paths.Count == rhs.Paths.Count
			&& Paths.SequenceEqual(rhs.Paths)
			&& ActionPath == rhs.ActionPath
			&& Hash == rhs.Hash
			&& ControllerName == rhs.ControllerName
			&& ActionName == rhs.ActionName
			&& FullRawSource.Code == rhs.FullRawSource.Code;
		}
	}
}