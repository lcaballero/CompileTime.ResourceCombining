using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompileTime.Handlers
{
	public class CacheRequest
	{
		public string Hash { get; set; }
		public string Type { get; set; }
		public string Path { get; set; }
	}
}