using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace dotnetCoreConsole
{
	public partial class SystemInformation
	{
		public static List<string> ProcessExecution(string fileName, string arguments = null)
		{
			ProcessStartInfo procStartInfo;
			if (arguments == null)
				procStartInfo = new ProcessStartInfo(fileName);
			else
				procStartInfo = new ProcessStartInfo(fileName, arguments);

			procStartInfo.RedirectStandardOutput = true;
			procStartInfo.UseShellExecute = false;
			procStartInfo.CreateNoWindow = true;

			Process proc = new Process();
			proc.StartInfo = procStartInfo;
			proc.Start();
			proc.WaitForExit();

			var dataList = new List<string>();
			while (!proc.StandardOutput.EndOfStream)
			{
				dataList.Add(proc.StandardOutput.ReadLine());
			}
			return dataList;
		}
	}
}
