using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.IO;

namespace dotnetCoreConsole
{
	public partial class SystemInformation
	{
		// Если TextWriter = null, то вывод будет оформляться в консоль.
		// Вывод количества процессов системы.
		public static void Print(Processes dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			using (textWriter)
			{
				textWriter.WriteLine("Количество процессов: {0}", dataToPrint.NumberOfProcesses);
				textWriter.WriteLine();
			}
		}
		// Вывод информации об Операционной Системе.
		public static void Print(OperatingSystem dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			using (textWriter)
			{
				textWriter.WriteLine("Архитектура ОС: {0}", dataToPrint.Architecture);
				textWriter.WriteLine("Описание ОС: {0}", dataToPrint.Description);
				textWriter.WriteLine();
			}
		}
		// Вывод информации о дисках.
		public static void Print(List<Disk> dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			foreach (var item in dataToPrint)
			{
				using (textWriter)
				{
					textWriter.WriteLine("Модель: {0}", item.Model);
					textWriter.WriteLine("Том: {0}", item.Label);
					textWriter.WriteLine("Тип: {0}", item.Type);
					textWriter.WriteLine("Формат: {0}", item.DriveFormat);
					textWriter.WriteLine("Всего места: {0} Гб", item.TotalSize);
					textWriter.WriteLine("Свободное место: {0} Гб", item.TotalFreeSpace);
					textWriter.WriteLine("Занято места: {0} Гб", item.UsedSpace);
					textWriter.WriteLine("Имя экземпляра: {0}", item.InstanceName);
					textWriter.WriteLine("Температура: {0} °С", item.Temperature);
					textWriter.WriteLine();
				}
			}
		}
		// Вывод информации о сетевых интерфейсах.
		public static void Print(List<NetInts> dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			if (dataToPrint == null)
			{
				textWriter.WriteLine("Сетевых интерфейсов не найдено");
				return;
			}
			textWriter.WriteLine("Количество сетевых интерфейсов: {0}", dataToPrint.Count);
			foreach (var item in dataToPrint)
			{
				textWriter.WriteLine("Интерфейс: {0}, тип: {1}", item.Name, item.Type);
				textWriter.WriteLine("Описание: {0}", item.Description);
				if (item.Type == NetworkInterfaceType.Loopback)
					continue;
				if (item.DNS != null)
				{
					textWriter.WriteLine("DNS-адреса:");
					foreach (var dnsInfo in item.DNS)
					{
						textWriter.WriteLine("\t{0}, IPv4: {1}", dnsInfo.Item1, dnsInfo.Item2);
					}
				}
				if (item.Gateway != null)
				{
					textWriter.WriteLine("Адреса шлюза:");
					foreach (var gtwInfo in item.Gateway)
					{
						textWriter.WriteLine("\t{0}", gtwInfo);
					}
				}
				if (item.Unicast != null)
				{
					textWriter.WriteLine("Маски подсетей:");
					foreach (var uniInfo in item.Unicast)
					{
						textWriter.WriteLine("\t{0}, адрес: {1}", uniInfo.Item1, uniInfo.Item2);
					}
				}
				textWriter.WriteLine();
			}
			try
			{
				textWriter.Dispose();
				textWriter.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		// Вывод информации о подключенных USB.
		public static void Print(List<USB> dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			foreach (var item in dataToPrint)
			{
				using (textWriter)
				{
					textWriter.WriteLine("Имя: {0},\nID: {1}", item.Name, item.DeviceID);
					textWriter.WriteLine();
				}
			}

		}
		// Вывод информации о процессоре.
		public static void Print(CentralProcessorUnit dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			using (textWriter)
			{
				textWriter.WriteLine("Модель процессора:\t{0}", dataToPrint.Name);
				textWriter.WriteLine("Количество ядер:\t{0}", dataToPrint.NumberOfCores);
				textWriter.WriteLine("Количество потоков:\t{0}", dataToPrint.NumberOfLogicalProcessors);
				textWriter.WriteLine("Текущая частота:\t{0} МГц", dataToPrint.CurrentClockSpeed);
				textWriter.WriteLine("Загрузка процессора:\t{0}%", dataToPrint.LoadPercentage);
				textWriter.WriteLine("Температура процессора:\t{0} °С", dataToPrint.Temperature);
				textWriter.WriteLine();
			}
		}
		// Вывод информации об оперативной памяти.
		public static void Print(Memory dataToPrint, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			using (textWriter)
			{
				textWriter.WriteLine("Всего оперативной памяти: {0} МБ", dataToPrint.Total);
				textWriter.WriteLine("Доступно: {0} МБ", dataToPrint.Free);
				textWriter.WriteLine("Занято: {0} МБ", dataToPrint.Used);
				textWriter.WriteLine();
			}
		}
		//
		// Вывод сериализованной строки.
		//
		public static void Print (string json, TextWriter textWriter = null)
		{
			if (textWriter == null)
			{
				textWriter = Console.Out;
			}
			using (textWriter)
			{
				textWriter.WriteLine(json);
				textWriter.WriteLine();
			}
		}
	}
}
