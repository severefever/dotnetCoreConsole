using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace dotnetCoreConsole
{
    using static SystemInformation;
    internal class Program
    {
		static void Main(string[] args)
		{
			SystemInformation mySystem = new SystemInformation(DefinePlatform());

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
                Console.WriteLine("9.  Сериализовать информацию о системе, полученную ранее.");
                Console.WriteLine("10. Сериализовать всю информацию о системе.");
                Console.WriteLine("11. Очистить экран.");
                Console.WriteLine("12. Завершить программу.");
                Console.Write("Выберите действие (напишите номер): ");
                int choice = Int32.Parse(Console.ReadLine());
                string yesOrNo;
                switch (choice)
                {
                    case 1:
						{
                            Parallel.Invoke(
                                () =>
                                {
                                    Print(mySystem.GetNumberOfProcesses());
                                },
                                () =>
                                {
                                    Print(mySystem.GetOSInfo());
                                },
                                () =>
                                {
                                    Print(mySystem.GetNetworkInterfacesInfo());
                                },
                                () =>
                                {
                                    Print(mySystem.GetUSBPortsInfo());
                                },
                                () =>
                                {
                                    Print(mySystem.GetCPUInfo());
                                },
                                () =>
                                {
                                    Print(mySystem.GetSystemMemoryInfo());
                                },
                                () =>
                                {
                                    Print(mySystem.GetDiskInfo());
                                }
                                );
                            break;
                        }
                    case 2:
						{
                            Print(mySystem.GetNumberOfProcesses());
                            break;
                        }
                    case 3:
						{
                            Print(mySystem.GetOSInfo());
                            break;
                        }
                    case 4:
						{
                            Print(mySystem.GetNetworkInterfacesInfo());
                            break;
                        }
                    case 5:
						{
                            Print(mySystem.GetUSBPortsInfo());
                            break;
                        }
                    case 6:
						{
                            Print(mySystem.GetCPUInfo());
                            break;
                        }
                    case 7:
						{
                            Print(mySystem.GetSystemMemoryInfo());
                            break;
                        }
                    case 8:
						{
                            Print(mySystem.GetDiskInfo());
                            break;
                        }
                    case 9:
						{
                            while (true)
							{
                                Console.Write("Вывести в удобном для чтения формате? [y/n]: ");
                                yesOrNo = Console.ReadLine();
                                if (yesOrNo.ToLower().Equals("y") || yesOrNo.ToLower().Equals("yes"))
                                {
                                    JsonSerializerSettings settings = new JsonSerializerSettings();
                                    settings.Formatting = Formatting.Indented;
                                    Print(mySystem.SerializeData(settings));
                                }
                                else if (yesOrNo.ToLower().Equals("n") || yesOrNo.ToLower().Equals("no"))
                                {
                                    Print(mySystem.SerializeData());
                                }
                                else
                                {
                                    Console.WriteLine("Неверно введен ответ. Попробуйте еще раз.");
                                    continue;
                                }
                                break;
                            }
                            Console.WriteLine();
                            break;
                        }
                    case 10:
						{
                            while (true)
							{
                                Console.Write("Вывести в удобном для чтения формате? [y/n]: ");
                                yesOrNo = Console.ReadLine();
                                if (yesOrNo.ToLower().Equals("y") || yesOrNo.ToLower().Equals("yes"))
                                {
                                    JsonSerializerSettings settings = new JsonSerializerSettings();
                                    settings.Formatting = Formatting.Indented;
                                    Print(mySystem.SerializeAllData(settings));
                                }
                                else if (yesOrNo.ToLower().Equals("n") || yesOrNo.ToLower().Equals("no"))
                                {
                                    Print(mySystem.SerializeAllData());
                                }
                                else
                                {
                                    Console.WriteLine("Неверно введен ответ. Попробуйте еще раз.");
                                    continue;
                                }
                                break;
                            }
                            Console.WriteLine();
                            break;
                        }
                    case 11:
						{
                            Console.Clear();
                            break;
                        }
                    case 12:
						{
                            isStop = true;
                            break;
                        }
                    default:
                        Console.WriteLine("Неверно введено значение.");
                        break;
                }
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