using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace dotnetCoreConsole
{
    interface IOperatingSystemSpecial
    {
        void USBPortsInfo();
        void MemoryInfo();
        void CPUInfo();
        void DrivesTemperatureProbe();
        void CPUTemperatureProbe();
    }
    internal class SystemInformation
    {
        IOperatingSystemSpecial _specialOS;
        public SystemInformation() { }
        public SystemInformation(IOperatingSystemSpecial specialOS)
        {
            _specialOS = specialOS;
        }
        public void GetUSBPortsInfo()
        {
            _specialOS.USBPortsInfo();
        }
        public void GetSystemMemoryInfo()
        {
            _specialOS.MemoryInfo();
        }
        public void GetCPUInfo()
        {
            _specialOS.CPUInfo();
        }
        public void GetDrivesTemperature()
        {
            _specialOS.DrivesTemperatureProbe();
        }
        public void GetCPUTemperature()
        {
            _specialOS.CPUTemperatureProbe();
        }
        //
        // Cross platform methods
        //
        public static void GetDiskInfo()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                Console.WriteLine("Диск:      {0}", drive.Name);
                Console.WriteLine("Тип диска: {0}", drive.DriveType);
                if (drive.IsReady == true)
                {
                    Console.WriteLine("  Название диска:   {0}", drive.VolumeLabel);
                    Console.WriteLine("  Файловая система: {0}", drive.DriveFormat);
                    Console.WriteLine(
                        "  Общее свободное пространство\n" +
                        "     для текущего пользователя:   {0} гигабайт",
                        Math.Round(drive.AvailableFreeSpace / 1073741824.0, 2));

                    Console.WriteLine(
                        "  Общее свободное пространство:   {0} гигабайт",
                        Math.Round(drive.TotalFreeSpace / 1073741824.0, 2));

                    Console.WriteLine(
                        "  Общий объем диска:              {0} гигабайт",
                        Math.Round(drive.TotalSize / 1073741824.0, 2));
                    Console.WriteLine(
                        "  Занято пространства:            {0} гигабайт",
                        Math.Round(Convert.ToDouble(drive.TotalSize - drive.TotalFreeSpace) / 1073741824.0, 2));
                }
            }
        }
        public static void GetOSInfo()
        {
            Console.WriteLine("Описание ОС: {0}", RuntimeInformation.OSDescription);
            Console.WriteLine("Архитектура ОС: {0}", RuntimeInformation.OSArchitecture);
        }
        public static void GetNumberOfProcesses()
        {
            Console.WriteLine($"Количество процессов: {Process.GetProcesses().Length}");
        }
        public static void GetNetworkInterfacesInfo()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if (adapters == null || adapters.Length < 1)
            {
                Console.WriteLine("Сетевых интерфейсов не найдено.");
                return;
            }
            Console.WriteLine("Количество сетевых интерфейсов: {0}", adapters.Length);
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine("Интерфейс: {0}, тип: {1}", adapter.Name, adapter.NetworkInterfaceType);
                Console.WriteLine("Описание: {0}", adapter.Description);
                //Console.WriteLine("Физический адрес: {0}", adapter.GetPhysicalAddress());
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;
                if (properties.DnsAddresses.Count > 0)
                {
                    Console.WriteLine("DNS-адреса:");
                    foreach (var dnsAdd in properties.DnsAddresses)
                    {
                        Console.WriteLine("\t{0}, IPv4: {1}", dnsAdd.ToString(), dnsAdd.MapToIPv4().ToString());
                    }
                }
                if (properties.GatewayAddresses.Count > 0)
                {
                    Console.WriteLine("Адреса шлюза:");
                    foreach (var gtwAdd in properties.GatewayAddresses)
                    {
                        Console.WriteLine("\t{0}", gtwAdd.Address);
                    }
                }
                if (properties.UnicastAddresses.Count > 0)
                {
                    Console.WriteLine("Маски подсетей:");
                    foreach (var uni in properties.UnicastAddresses)
                    {
                        Console.WriteLine("\t{0}, адрес: {1}", uni.IPv4Mask, uni.Address.MapToIPv4());
                    }
                }
                Console.WriteLine();
            }
        }
    }
    class WindowsSystemInfo : IOperatingSystemSpecial
    {
        public void USBPortsInfo()
        {
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            foreach (ManagementObject mo in objOSDetails.Get())
            {
                if (Convert.ToString(mo["DeviceID"]).StartsWith("USB"))
                {
                    Console.WriteLine("Имя: {0}, ID: {1}", mo["Name"], mo["DeviceID"]);
                    if (Convert.ToString(mo["Name"]) != Convert.ToString(mo["Description"]))
                        Console.WriteLine("Описание: {0}", mo["Description"]);
                }
            }
            objOSDetails.Dispose();
        }
        public void MemoryInfo()
        {
            PerformanceCounter memAvailable = new PerformanceCounter("Memory", "Available MBytes");
            double availableMemory = memAvailable.RawValue;
            double totalMemory = 0.0;
            ManagementObjectSearcher objDetails = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject mo in objDetails.Get())
            {
                totalMemory += Convert.ToDouble(mo["Capacity"]) / 1048576.0;
            }
            Console.WriteLine("Всего оперативной памяти:\t{0} МБ", totalMemory);
            Console.WriteLine("Доступно:\t{0} МБ", availableMemory);
            Console.WriteLine("Занято:\t{0} МБ", totalMemory - availableMemory);
            objDetails.Dispose();
        }
        public void CPUInfo()
        {
            SelectQuery sq = new SelectQuery("Win32_Processor");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(sq);
            foreach (ManagementObject mo in objOSDetails.Get())
            {
                Console.WriteLine("Модель процессора:\t{0}", mo["Name"]);
                Console.WriteLine("Количество ядер:\t{0}", mo["NumberOfCores"]);
                Console.WriteLine("Количество потоков:\t{0}", mo["NumberOfLogicalProcessors"]);
                Console.WriteLine("Максимальная частота:\t{0} МГц", mo["MaxClockSpeed"]);
                Console.WriteLine("Текущая частота:\t{0} МГц", mo["CurrentClockSpeed"]);
                Console.WriteLine("Загрузка процессора:\t{0}%", mo["LoadPercentage"]);
            }
            /*
            objOSDetails = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name=\"_Total\"");
            foreach (ManagementObject mo in objOSDetails.Get())
            {
                Console.WriteLine("Загрузка процессора:\t{0}%", mo["PercentProcessorTime"]);
            }
            */
            objOSDetails.Dispose();
        }
        public void DrivesTemperatureProbe()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSStorageDriver_ATAPISmartData");
            string friendlyName = null;
            string instanceName = null;
            RegistryKey hLocal = Registry.LocalMachine;
            foreach (ManagementObject obj in searcher.Get())
            {
                // Получение имени диска.
                instanceName = obj["InstanceName"].ToString();
                if (instanceName.EndsWith("_0"))
                {
                    instanceName = instanceName.Substring(0, instanceName.Length - 2);
                }
                friendlyName = Convert.ToString(hLocal.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\" + instanceName).GetValue("FriendlyName"));
                Console.WriteLine("Имя диска:\t{0}", friendlyName);

                byte[] vendorSpec = obj["VendorSpecific"] as byte[];

                const int tableID = 2;
                const int temperatureID = 194;
                const int tableRawValue = 7;
                const int numOfSMARTParams = 30;
                const int tableMultiplier = 12;
                for (int i = 0; i < numOfSMARTParams; i++)
                {
                    /*
                    Вся информация лежит в vendorSpec (информация, которую предоставляет S.M.A.R.T).
                    Этот массив по сути является таблицей (матрицей), в которой всего 30 строк.
                    В каждой строке 12 чисел (уже переведенных в 10-тичную систему счисления).
                    Каждая строка содержит какой-то параметр диска, по типу температуры, частоты ошибок и т.д.
                     
                    Идентификатор атрибута находится в 3-ей ячейке таблицы (если считать с нуля, то во 2-ой (tableID)).
                    Этот идентификатор позволяет понять, какая информация хранится в этой строке.
                    Если ID = 194 (в 16-тиричной системе C2), то это означает, что в этой строке хранится значение Температуры (temperatureID).
                    
                    Значение атрибута хранится в 8-ой ячейке таблицы (если считать с нуля, то в 7-ой (tableRawValue)).
                    Хранится сразу же в 10-тичном формате, в градусах ЦЕЛЬСИЯ.

                    Такие дела :\
                    */
                    if (vendorSpec[i * tableMultiplier + tableID] == temperatureID)
                    {
                        Console.WriteLine("Температура:\t{0} °C", vendorSpec[i * 12 + tableRawValue]);
                    }
                }
            }
        }
        public void CPUTemperatureProbe()
        {
            
        }
    }
}
