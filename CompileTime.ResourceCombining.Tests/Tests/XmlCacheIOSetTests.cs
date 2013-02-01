using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CompileTime.ResourceCombining.Helpers;


namespace CompileTime.ResourceCombining.Tests
{
	[TestFixture]
	public class XmlCacheIOSetTests : AssertionHelper
	{
		[Test]
		public void Null_Sets_Does_Not_Fail()
		{
			List<string> a = null;

			a.Difference(a);

			Assert.Pass();
		}

		[Test]
		public void One_Null_Sets_Does_Not_Fail()
		{
			List<int> a = null;
			List<int> b = new List<int>();

			a.Difference(b);

			Assert.Pass();
		}

		[Test]
		public void No_Null_Sets_Does_Not_Fail()
		{
			List<int> a = new List<int>();
			List<int> b = new List<int>();

			a.Difference(b);

			Assert.Pass();
		}

		[Test]
		public void Same_Sets_Produce_Empty_Set_On_Difference()
		{
			var a = new [] { 1 };
			var b = new [] { 1 };

			var c = a.Difference(b);

			Expect(c, Is.Not.Null);
			Expect(c.Count, Is.EqualTo(0));
		}

		[Test]
		public void One_Sided_Difference()
		{
			var a = new[] { 1, 2 };
			var b = new[] { 1 };

			var c = a.Difference(b);

			Expect(c, Is.Not.Null);
			Expect(c.Count, Is.EqualTo(1));
			Expect(c.First(), Is.EqualTo(2));
		}

		[Test]
		public void Two_Sided_Difference()
		{
			var a = new[] { 1, 2 };
			var b = new[] { 1, 3 };

			var c = a.Difference(b);

			Expect(c, Is.Not.Null);
			Expect(c.Count, Is.EqualTo(2));
			Expect(c.SetEquals(new [] { 2, 3 }));
		}

		[Test]
		public void Null_Dupes_Does_Not_Fail()
		{
			int[] a = null;

			a.Duplicates();

			Assert.Pass();
		}

		[Test]
		public void Empty_List_Dupes_Produces_Empty_List()
		{
			int[] a = new int[] {};

			var c = a.Duplicates();

			Expect(c.Count(), Is.EqualTo(0));
		}

		[Test]
		public void Non_Empty_Without_Dupes_Produces_Empty_List()
		{
			int[] a = new int[] { 1, 2, 3, 4, 5 };

			var c = a.Duplicates();

			Expect(c.Count(), Is.EqualTo(0));
		}

		[Test]
		public void Non_Empty_With_Dupes_Produces_Dupes()
		{
			int[] a = new int[] { 1, 2, 3, 4, 2, 3 };

			var c = new HashSet<int>(a.Duplicates());

			Expect(c.SetEquals(new [] { 2, 3 }));
		}
	}
}
