using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace dotnetCoreConsole
{
	public partial class SystemInformation
	{
		// Вывод информации об Операционной Системе.
		public static void Print(OperatingSystem dataToPrint)
		{
			Console.WriteLine("Архитектура ОС: {0}", dataToPrint.architecture);
			Console.WriteLine("Описание ОС: {0}", dataToPrint.description);
		}
		// Вывод информации о дисках.
		public static void Print(List<Disk> dataToPrint)
		{
			foreach (var item in dataToPrint)
			{
				Console.WriteLine("Модель: {0}", item.model);
				Console.WriteLine("Том: {0}", item.label);
				Console.WriteLine("Тип: {0}", item.type);
				Console.WriteLine("Формат: {0}", item.driveFormat);
				Console.WriteLine("Всего места: {0} Гб", item.totalSize);
				Console.WriteLine("Свободное место: {0} Гб", item.totalFreeSpace);
				Console.WriteLine("Занято места: {0} Гб", item.usedSpace);
				Console.WriteLine("Имя экземпляра: {0}", item.instanceName);
				Console.WriteLine("Температура: {0} °С", item.temperature);
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
				Console.WriteLine("Интерфейс: {0}, тип: {1}", item.name, item.type);
				Console.WriteLine("Описание: {0}", item.description);
				if (item.type == NetworkInterfaceType.Loopback)
					continue;
				if (item.dns != null)
				{
					Console.WriteLine("DNS-адреса:");
					foreach (var dnsInfo in item.dns)
					{
						Console.WriteLine("\t{0}, IPv4: {1}", dnsInfo.Item1, dnsInfo.Item2);
					}
				}
				if (item.gateway != null)
				{
					Console.WriteLine("Адреса шлюза:");
					foreach (var gtwInfo in item.gateway)
					{
						Console.WriteLine("\t{0}", gtwInfo);
					}
				}
				if (item.unicast != null)
				{
					Console.WriteLine("Маски подсетей:");
					foreach (var uniInfo in item.unicast)
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
				Console.WriteLine("Имя: {0}, ID: {1}", item.name, item.deviceID);
				if (item.description != null)
					Console.WriteLine("Описание: {0}", item.description);
			}

		}
		// Вывод информации о процессоре.
		public static void Print(CentralProcessorUnit dataToPrint)
		{
			Console.WriteLine("Модель процессора:\t{0}", dataToPrint.name);
			Console.WriteLine("Количество ядер:\t{0}", dataToPrint.numberOfCores);
			Console.WriteLine("Количество потоков:\t{0}", dataToPrint.numberOfLogicalProcessors);
			Console.WriteLine("Текущая частота:\t{0} МГц", dataToPrint.currentClockSpeed);
			Console.WriteLine("Загрузка процессора:\t{0}%", dataToPrint.loadPercentage);
			if (dataToPrint.temperature == -1.0)
			{
				Console.WriteLine("Не удалось получить температуру.");
			}
			else
			{
				Console.WriteLine("Температура процессора:\t{0} C", dataToPrint.temperature);
			}
		}
		// Вывод информации об оперативной памяти.
		public static void Print(Memory dataToPrint)
		{
			Console.WriteLine("Всего оперативной памяти:\t{0} МБ", dataToPrint.total);
			Console.WriteLine("Доступно:\t{0} МБ", dataToPrint.free);
			Console.WriteLine("Занято:\t{0} МБ", dataToPrint.used);
		}
	}
}
