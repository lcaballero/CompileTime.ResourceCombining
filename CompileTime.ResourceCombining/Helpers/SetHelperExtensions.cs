using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CompileTime.ResourceCombining.Helpers
{
	public static class SetHelperExtensions
	{
		private static IEnumerable<T> Empty<T>()
		{
			yield break;
		}

		public static HashSet<T> Difference<T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			a = a ?? Empty<T>();
			b = b ?? Empty<T>();

			var union = a.Union(b);
			var intersect = a.Intersect(b);

			var diff = union.Except(intersect);

			return new HashSet<T>(diff);
		}

		public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> a)
		{
			var set = new HashSet<T>();

			foreach (var item in a)
			{
				if (set.Contains(item))
				{
					yield return item;
				}
				else
				{
					set.Add(item);
				}
			}
		}
	}
}
