using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace dotnetCoreConsole
{
    using static SystemInformation;
    internal class Program
    {
        static void Main(string[] args)
        {
            SystemInformation mySystem = new SystemInformation(DefinePlatform());
            try
            {
                //Console.WriteLine(GetNumberOfProcesses());
                //Print(GetNetworkInterfacesInfo());
                //Print(mySystem.GetUSBPortsInfo());
                //Print(mySystem.GetCPUInfo());
                //Print(mySystem.GetSystemMemoryInfo());
                Print(mySystem.GetDiskInfo());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static IOperatingSystemSpecial DefinePlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsSystemInfo();
            }
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return new LinuxSystemInfo();
			}
			else
            {
                throw new Exception("Программа не смогла определить операционную систему.");
            }
        }
    }
}
