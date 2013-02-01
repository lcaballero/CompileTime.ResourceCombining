using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using SimpleLineParser;
using System.Security.Cryptography;


namespace CompileTime.ResourceCombining
{
	public static class XmlCacheIO
	{
		public static string ToXml<T>(this T c)
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			StringWriter sw = new StringWriter();

			using (XmlTextWriter xw = new XmlTextWriter(sw))
			{
				xw.Formatting = Formatting.Indented;
				xs.Serialize(xw, c);
			}

			return sw.ToString();
		}

		public static T FromXml<T>(this string xml)
			where T : class
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			StringReader sw = new StringReader(xml);
			T t = null;

			using (XmlTextReader xw = new XmlTextReader(sw))
			{
				t = (T)xs.Deserialize(xw);
			}

			return t ?? default(T);
		}

		public static byte[] ToUtf8Bytes(this string input)
		{
			byte[] bs = Encoding.UTF8.GetBytes(input);

			return bs;
		}

		public static string ToMD5Hash(this byte[] bytes)
		{
			MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();

			bytes = x.ComputeHash(bytes);

			StringBuilder s = new StringBuilder();

			foreach (byte b in bytes)
			{
				s.Append(b.ToString("x2").ToLower());
			}

			string password = s.ToString();

			return password;
		}
	}
}
