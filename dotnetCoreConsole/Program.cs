using System;
using System.Runtime.InteropServices;

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
                //mySystem.GetDrivesTemperature();
                mySystem.GetSystemMemoryInfo();
                Console.WriteLine();
                mySystem.GetUSBPortsInfo();
                Console.WriteLine();
                mySystem.GetCPUInfo();
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
