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
				Console.WriteLine("Количество процессов: {0}", GetNumberOfProcesses());

				Console.WriteLine("\n------ Информация об ОС ------\n");
				Print(GetOSInfo());

				Console.WriteLine("\n------ Сетевые интерфейсы ------\n");
				Print(GetNetworkInterfacesInfo());

				Console.WriteLine("\n------ Подключенные USB ------\n");
				Print(mySystem.GetUSBPortsInfo());

				Console.WriteLine("\n------ Информация о процессоре ------\n");
				Print(mySystem.GetCPUInfo());

				Console.WriteLine("\n------ Информация об оперативной памяти ------\n");
				Print(mySystem.GetSystemMemoryInfo());

				Console.WriteLine("\n------ Информация о дисках ------\n");
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
