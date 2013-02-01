using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleLineParser;
using CompileTime.ResourceCombining.Tests;


namespace CompileTime.ResourceCombining.Tests
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//ExampleOutput.ToConsole();

			new InfiniteRecursiveIncludesTests().Find_Errors_In_Infinite_Recursive_Package();
		}
	}
}
