using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace dotnetCoreConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SystemInformation mySystem = new SystemInformation(DefinePlatform());

            try
            {
                mySystem.GetDrivesTemperature();
                //mySystem.GetUSBPortsInfo();
                //mySystem.GetSystemMemoryInfo();
                //mySystem.GetCPUInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
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
