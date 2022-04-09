using System;
using System.Collections.Generic;

namespace dotnetCoreConsole
{
	public partial class SystemInformation
	{
		public static void Print(Dictionary<string, string> dataToPrint)
		{
			foreach (var item in dataToPrint)
			{
				Console.WriteLine(item.Key + ": " + item.Value);
			}
		}
		public static void Print(Dictionary<string, int> dataToPrint)
		{
			foreach (var item in dataToPrint)
			{
				Console.WriteLine(item.Key + ": " + item.Value);
			}
		}
	}
}
