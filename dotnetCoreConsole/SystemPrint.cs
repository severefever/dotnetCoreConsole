using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace dotnetCoreConsole
{
	public partial class SystemInformation
	{
		// Вывод количества процессов системы.
		public static void Print(Processes dataToPrint)
		{
			Console.WriteLine("Количество процессов: {0}", dataToPrint.NumberOfProcesses);
		}
		// Вывод информации об Операционной Системе.
		public static void Print(OperatingSystem dataToPrint)
		{
			Console.WriteLine("Архитектура ОС: {0}", dataToPrint.Architecture);
			Console.WriteLine("Описание ОС: {0}", dataToPrint.Description);
		}
		// Вывод информации о дисках.
		public static void Print(List<Disk> dataToPrint)
		{
			foreach (var item in dataToPrint)
			{
				Console.WriteLine("Модель: {0}", item.Model);
				Console.WriteLine("Том: {0}", item.Label);
				Console.WriteLine("Тип: {0}", item.Type);
				Console.WriteLine("Формат: {0}", item.DriveFormat);
				Console.WriteLine("Всего места: {0} Гб", item.TotalSize);
				Console.WriteLine("Свободное место: {0} Гб", item.TotalFreeSpace);
				Console.WriteLine("Занято места: {0} Гб", item.UsedSpace);
				Console.WriteLine("Имя экземпляра: {0}", item.InstanceName);
				Console.WriteLine("Температура: {0} °С", item.Temperature);
				Console.WriteLine();
			}
		}
		// Вывод информации о сетевых интерфейсах.
		public static void Print(List<NetInts> dataToPrint)
		{
			if (dataToPrint == null)
			{
				Console.WriteLine("Сетевых интерфейсов не найдено");
				return;
			}
			Console.WriteLine("Количество сетевых интерфейсов: {0}", dataToPrint.Count);
			foreach (var item in dataToPrint)
			{
				Console.WriteLine("Интерфейс: {0}, тип: {1}", item.Name, item.Type);
				Console.WriteLine("Описание: {0}", item.Description);
				if (item.Type == NetworkInterfaceType.Loopback)
					continue;
				if (item.DNS != null)
				{
					Console.WriteLine("DNS-адреса:");
					foreach (var dnsInfo in item.DNS)
					{
						Console.WriteLine("\t{0}, IPv4: {1}", dnsInfo.Item1, dnsInfo.Item2);
					}
				}
				if (item.Gateway != null)
				{
					Console.WriteLine("Адреса шлюза:");
					foreach (var gtwInfo in item.Gateway)
					{
						Console.WriteLine("\t{0}", gtwInfo);
					}
				}
				if (item.Unicast != null)
				{
					Console.WriteLine("Маски подсетей:");
					foreach (var uniInfo in item.Unicast)
					{
						Console.WriteLine("\t{0}, адрес: {1}", uniInfo.Item1, uniInfo.Item2);
					}
				}
				Console.WriteLine();
			}
		}
		// Вывод информации о подключенных USB.
		public static void Print(List<USB> dataToPrint)
		{
			foreach (var item in dataToPrint)
			{
				Console.WriteLine("Имя: {0},\nID: {1}", item.Name, item.DeviceID);
				Console.WriteLine();
			}

		}
		// Вывод информации о процессоре.
		public static void Print(CentralProcessorUnit dataToPrint)
		{
			Console.WriteLine("Модель процессора:\t{0}", dataToPrint.Name);
			Console.WriteLine("Количество ядер:\t{0}", dataToPrint.NumberOfCores);
			Console.WriteLine("Количество потоков:\t{0}", dataToPrint.NumberOfLogicalProcessors);
			Console.WriteLine("Текущая частота:\t{0} МГц", dataToPrint.CurrentClockSpeed);
			Console.WriteLine("Загрузка процессора:\t{0}%", dataToPrint.LoadPercentage);
			Console.WriteLine("Температура процессора:\t{0} °С", dataToPrint.Temperature);
		}
		// Вывод информации об оперативной памяти.
		public static void Print(Memory dataToPrint)
		{
			Console.WriteLine("Всего оперативной памяти: {0} МБ", dataToPrint.Total);
			Console.WriteLine("Доступно: {0} МБ", dataToPrint.Free);
			Console.WriteLine("Занято: {0} МБ", dataToPrint.Used);
		}
	}
}
