using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace dotnetCoreConsole
{
    using static SystemInformation;
    internal class Program
    {
		static void Main(string[] args)
		{
			SystemInformation mySystem = new SystemInformation(DefinePlatform());
            SystemData dataToJson = new SystemData(mySystem);

            bool isStop = false;
            while (!isStop)
            {
                Console.WriteLine("1.  Вывести всю информацию о системе.");
                Console.WriteLine("2.  Вывести текущее количество процессов.");
                Console.WriteLine("3.  Вывести информацию об операционной системе.");
                Console.WriteLine("4.  Вывести информацию о сетевых интерфейсах.");
                Console.WriteLine("5.  Вывести информацию о подключенных USB.");
                Console.WriteLine("6.  Вывести информацию о процессоре.");
                Console.WriteLine("7.  Вывести информацию об оперативной памяти.");
                Console.WriteLine("8.  Вывести информацию о дисках.");
                Console.WriteLine("9.  Сериализовать информацию о системе, полученную ранее");
                Console.WriteLine("10. Сериализовать всю информацию о системе.");
                Console.WriteLine("11. Очистить экран.");
                Console.WriteLine("12. Завершить программу.");
                Console.Write("Выберите действие (напишите номер): ");
                int choice = Int32.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
						Console.WriteLine("\n------ Количество процессов ------\n");
						Print(GetNumberOfProcesses());
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
						break;
                    case 2:
						Console.WriteLine("\n------ Количество процессов ------\n");
						Print(GetNumberOfProcesses());
						break;
                    case 3:
						Console.WriteLine("\n------ Информация об ОС ------\n");
						Print(GetOSInfo());
						break;
                    case 4:
						Console.WriteLine("\n------ Сетевые интерфейсы ------\n");
						Print(GetNetworkInterfacesInfo());
						break;
                    case 5:
						Console.WriteLine("\n------ Подключенные USB ------\n");
						Print(mySystem.GetUSBPortsInfo());
						break;
                    case 6:
						Console.WriteLine("\n------ Информация о процессоре ------\n");
						Print(mySystem.GetCPUInfo());
						break;
                    case 7:
						Console.WriteLine("\n------ Информация об оперативной памяти ------\n");
						Print(mySystem.GetSystemMemoryInfo());
						break;
                    case 8:
						Console.WriteLine("\n------ Информация о дисках ------\n");
						Print(mySystem.GetDiskInfo());
						break;
                    case 9:

                        break;
                    case 10:
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.Formatting = Formatting.Indented;
                        string InfoToJSON = JsonConvert.SerializeObject(dataToJson, settings);
                        Console.WriteLine(InfoToJSON);
                        break;
                    case 11:
                        Console.Clear();
                        break;
                    case 12:
                        isStop = true;
                        break;
                    default:
                        Console.WriteLine("Неверно введено значение.");
                        break;
                }
            }
			//SystemData test = new SystemData();
			//test = JsonConvert.DeserializeObject<SystemData>(InfoToJSON);
			//Print(test.USBs);
			//Print(test.RAM);
			//Print(test.CPU);
			//Print(test.Drives);
			//Print(test.OS);
			//Print(test.Procs);
			//Print(test.NetAdapters);
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