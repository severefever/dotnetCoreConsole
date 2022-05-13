using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Collections.Generic;

namespace dotnetCoreConsole
{
    public struct Disk
    {
        public string Model { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string DriveFormat { get; set; }
        public double TotalFreeSpace { get; set; }
        public double TotalSize { get; set; }
        public double UsedSpace { get; set; }
        public string InstanceName { get; set; }
        public double Temperature { get; set; }
    }
    public struct DiskTemp
	{
        public string Model { get; set; }
        public string InstanceName { get; set; }
        public double Temperature { get; set; }
    }
    public struct OperatingSystem
    {
        public string Description { get; set; }
        public string Architecture { get; set; }
    }
    public struct NetInts
    {
        public string Name { get; set; }
        public NetworkInterfaceType Type { get; set; }
        public string Description { get; set; }
        public List<(string, string)> DNS { get; set; }
        public List<string> Gateway { get; set; }
        public List<(string, string)> Unicast { get; set; }
    }
    public struct USB
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
    }
    public struct Memory
	{
        public double Total { get; set; }
        public double Used { get; set; }
        public double Free { get; set; }
    }
    public struct CentralProcessorUnit
	{
        public string Name { get; set; }
        public int NumberOfCores { get; set; }
        public int NumberOfLogicalProcessors { get; set; }
        public double CurrentClockSpeed { get; set; }
        public double LoadPercentage { get; set; }
        public double Temperature { get; set; }
    }
    public interface IOperatingSystemSpecial
    {
        List<USB> USBPortsInfo();
        Memory MemoryInfo();
        CentralProcessorUnit CPUInfo();
        List<Disk> DiskInfo();
    }
    public partial class SystemInformation
    {
        IOperatingSystemSpecial _specialOS;
        public SystemInformation() { }
        public SystemInformation(IOperatingSystemSpecial specialOS)
        {
            _specialOS = specialOS;
        }
        public List<USB> GetUSBPortsInfo()
        {
            return _specialOS.USBPortsInfo();
        }
        public Memory GetSystemMemoryInfo()
        {
            return _specialOS.MemoryInfo();
        }
        public CentralProcessorUnit GetCPUInfo()
        {
            return _specialOS.CPUInfo();
        }
        public List<Disk> GetDiskInfo()
        {
            return _specialOS.DiskInfo();
        }

        //
        // Кроссплатформенные методы
        //

        public static OperatingSystem GetOSInfo()
        {
            OperatingSystem operatingSystem = new OperatingSystem();
            operatingSystem.Description = RuntimeInformation.OSDescription;
            operatingSystem.Architecture = RuntimeInformation.OSArchitecture.ToString();
            return operatingSystem;
        }
        public static int GetNumberOfProcesses()
        {
            int NumberOfProcesses = Process.GetProcesses().Length;
            return NumberOfProcesses;
        }
        public static List<NetInts> GetNetworkInterfacesInfo()
        {
            var adaptersList = new List<NetInts>();
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if (adapters == null || adapters.Length < 1)
            {
                return adaptersList;
            }
            foreach (NetworkInterface adapter in adapters)
            {
                NetInts adapterInfo = new NetInts();
                IPInterfaceProperties properties = adapter.GetIPProperties();
                adapterInfo.Name = adapter.Name;
                adapterInfo.Type = adapter.NetworkInterfaceType;
                adapterInfo.Description = adapter.Description;
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;
                // DNS-адреса.
                if (properties.DnsAddresses.Count > 0)
                {
                    adapterInfo.DNS = new List<(string, string)>();
                    foreach (var dnsAdd in properties.DnsAddresses)
                    {
                        adapterInfo.DNS.Add((dnsAdd.ToString(), dnsAdd.MapToIPv4().ToString()));
                    }
                }
                // Адреса шлюзов.
                if (properties.GatewayAddresses.Count > 0)
                {
                    adapterInfo.Gateway = new List<string>();
                    foreach (var gtwAdd in properties.GatewayAddresses)
                    {
                        adapterInfo.Gateway.Add(gtwAdd.Address.ToString());
                    }
                }
                // Маски подсетей.
                if (properties.UnicastAddresses.Count > 0)
                {
                    adapterInfo.Unicast = new List<(string, string)>();
                    foreach (var uni in properties.UnicastAddresses)
                    {
                        adapterInfo.Unicast.Add((uni.IPv4Mask.ToString(), uni.Address.MapToIPv4().ToString()));
                    }
                }
                adaptersList.Add(adapterInfo);
            }
            return adaptersList;
        }
    }
    class WindowsSystemInfo : IOperatingSystemSpecial
    {
        public List<USB> USBPortsInfo()
        {
            var usbs = new List<USB>();
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            foreach (ManagementObject mo in objOSDetails.Get())
            {
                USB usb = new USB();
                if (Convert.ToString(mo["DeviceID"]).StartsWith("USB"))
                {
                    usb.Name = mo["Name"].ToString();
                    usb.DeviceID = mo["DeviceID"].ToString();
                    usbs.Add(usb);
                }
            }
            objOSDetails.Dispose();
            return usbs;
        }
        public Memory MemoryInfo()
        {
            Memory memory = new Memory();
            PerformanceCounter memAvailable = new PerformanceCounter("Memory", "Available MBytes");
            double availableMemory = memAvailable.RawValue;
            double totalMemory = 0.0;
            ManagementObjectSearcher objDetails = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject mo in objDetails.Get())
            {
                totalMemory += Double.Parse(mo["Capacity"].ToString()) / 1048576.0;
            }
            memory.Total = Math.Round(totalMemory);
            memory.Free = Math.Round(availableMemory);
            memory.Used = Math.Round(totalMemory - availableMemory);
            objDetails.Dispose();
            return memory;
        }
        public CentralProcessorUnit CPUInfo()
        {
            CentralProcessorUnit cpu = new CentralProcessorUnit();
            SelectQuery sq = new SelectQuery("Win32_Processor");
            ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(sq);
            foreach (ManagementObject mo in objOSDetails.Get())
            {
                cpu.Name = mo["Name"].ToString();
                cpu.NumberOfCores = Int32.Parse(mo["NumberOfCores"].ToString());
                cpu.NumberOfLogicalProcessors = Int32.Parse(mo["NumberOfLogicalProcessors"].ToString());
                cpu.CurrentClockSpeed = Double.Parse(mo["CurrentClockSpeed"].ToString());
                cpu.LoadPercentage = Double.Parse(mo["LoadPercentage"].ToString());
            }
            /*
            objOSDetails = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name=\"_Total\"");
            foreach (ManagementObject mo in objOSDetails.Get())
            {
                Console.WriteLine("Загрузка процессора:\t{0}%", mo["PercentProcessorTime"]);
            }
            */
            try
			{
                double temp = 0.0;
                objOSDetails = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");

                foreach (ManagementObject mo in objOSDetails.Get())
                {
                    temp = Double.Parse(mo["CurrentTemperature"].ToString());
                    // Перевод температуры в градусы Цельсия.
                    temp = (temp - 2732) / 10.0;
                }
                cpu.Temperature = temp;
            }
            catch (Exception ex)
			{
                Console.WriteLine("Ошибка получения температуры процессора: {0}", ex.Message);
			}
            objOSDetails.Dispose();
            return cpu;
        }
        public List<Disk> DiskInfo()
		{
            var disks = new List<Disk>();
            List<DiskTemp> tempList = GetDiskTemperature();

            var query = new SelectQuery("SELECT * FROM Win32_DiskDrive");
            var searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject moDisk in searcher.Get())
            {
                Disk disk = new Disk();
                // Запросить связанные разделы для текущего DeviceID.
                string assocQuery = "Associators of {Win32_DiskDrive.DeviceID='" +
                                                     moDisk.Properties["DeviceID"].Value.ToString() + "'}" +
                                                     "where AssocClass=Win32_DiskDriveToDiskPartition";
                var assocPart = new ManagementObjectSearcher(assocQuery);
                // Запросить связанные разделы для каждого диска.
                foreach (ManagementObject moPart in assocPart.Get())
                {
                    // Запросить связанные логические диски для текущего PartitionID.
                    string logDiskQuery = "Associators of {Win32_DiskPartition.DeviceID='" +
                                           moPart.Properties["DeviceID"].Value.ToString() + "'} " +
                                           "where AssocClass=Win32_LogicalDiskToPartition";
                    var logDisk = new ManagementObjectSearcher(logDiskQuery);
                    // Запросить логические диски для каждого раздела.
                    foreach (var logDiskEnu in logDisk.Get())
                    {
                        disk.Type = Convert.ToString((DriveType)Int32.Parse(logDiskEnu["DriveType"].ToString()));
                        disk.Model = moDisk["Model"].ToString();
                        disk.DriveFormat = logDiskEnu["FileSystem"].ToString();
                        disk.TotalFreeSpace = Math.Round(Double.Parse(logDiskEnu["FreeSpace"].ToString()) / 1073741824.0, 2);
                        disk.TotalSize = Math.Round(Double.Parse(logDiskEnu["Size"].ToString()) / 1073741824.0, 2);
                        disk.UsedSpace = Math.Round(disk.TotalSize - disk.TotalFreeSpace, 2);
                        disk.Label = logDiskEnu["Name"].ToString();

                        if (tempList.Count > 0)
						{
                            foreach (var temps in tempList)
                            {
                                if (disk.Model.Equals(temps.Model))
                                {
                                    disk.InstanceName = temps.InstanceName;
                                    disk.Temperature = temps.Temperature;
                                }
                            }
                        }
                    }
                    logDisk.Dispose();
                }
                disks.Add(disk);
                assocPart.Dispose();
            }
            searcher.Dispose();
            return disks;
        }
        List<DiskTemp> GetDiskTemperature()
		{
            List<DiskTemp> tempList = new List<DiskTemp>();
            ManagementObjectSearcher smartSearcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSStorageDriver_ATAPISmartData");
            string friendlyName;
            string instanceName;
            RegistryKey hLocal = Registry.LocalMachine;
            try
			{
                foreach (ManagementObject obj in smartSearcher.Get())
                {
                    DiskTemp disk = new DiskTemp();
                    // Получение имени диска.
                    instanceName = obj["InstanceName"].ToString();
                    if (instanceName.EndsWith("_0"))
                    {
                        instanceName = instanceName.Substring(0, instanceName.Length - 2);
                    }
                    friendlyName = Convert.ToString(hLocal.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\" + instanceName).GetValue("FriendlyName"));
                    disk.InstanceName = instanceName;
                    disk.Model = friendlyName;
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
                        */
                        if (vendorSpec[i * tableMultiplier + tableID] == temperatureID)
                        {
                            disk.Temperature = Convert.ToDouble(vendorSpec[i * 12 + tableRawValue]);
                        }
                    }
                    tempList.Add(disk);
                }
            }
            catch (Exception ex)
			{
                Console.WriteLine("Ошибка получения данных S.M.A.R.T дисков: {0}", ex.Message);
			}
            smartSearcher.Dispose();
            return tempList;
		}
    }
	class LinuxSystemInfo : IOperatingSystemSpecial
	{
		public List<USB> USBPortsInfo()
		{
			var data = SystemInformation.ProcessExecution("lsusb");
			var usbs = new List<USB>();
            
            const int spaceDel = 6;
            // Пример вывод команды lsusb в Linux
            // Bus 001 Device 002: ID 80ee:0021 VirtualBox USB Tablet
            // Константа нужна, чтобы имя USB-устройства отделить от
            //  его расположения и ID. Просто ради удобства.
            foreach (var d in data)
			{
                USB usb = new USB();
                int spaceCounter = 0;
                for (int i = 0; i < d.Length; i++)
				{
                    if (d[i].Equals(' '))
                        spaceCounter++;
                    if (spaceCounter == spaceDel)
					{
                        usb.Name = d.Substring(i + 1);
                        usb.DeviceID = d.Substring(0, i);
                        break;
					}
				}
                usbs.Add(usb);
			}
			return usbs;
		}
		public Memory MemoryInfo()
		{
			Memory memory = new Memory();
            var data = SystemInformation.ProcessExecution("bash", "./scripts/meminfo.sh");
            memory.Total = Double.Parse(data[0]);
            memory.Free = Double.Parse(data[1]);
            memory.Used = Double.Parse(data[2]);
            return memory;
		}
		public CentralProcessorUnit CPUInfo()
		{
            CentralProcessorUnit cpu = new CentralProcessorUnit();
			var data = SystemInformation.ProcessExecution("bash", "./scripts/cpuinfo.sh");
            try
			{
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                if (culture.ToString().Equals("ru-RU"))
				{
                    data[1] = data[1].Replace('.', ',');
                    data[4] = data[4].Replace('.', ',');
                    data[5] = data[5].Replace('.', ',');
				}
                
                cpu.Name = data[0];
                cpu.CurrentClockSpeed = Double.Parse(data[1]);
                cpu.NumberOfCores = Int32.Parse(data[2]);
                cpu.NumberOfLogicalProcessors = Int32.Parse(data[3]);
                cpu.LoadPercentage = Double.Parse(data[4]);
                cpu.Temperature = Double.Parse(data[5]);
            }
            catch (Exception)
			{
                cpu.Temperature = 0;
                Console.WriteLine("Ошибка получения температуры процессора!");
            }
            return cpu;
		}
        public List<Disk> DiskInfo()
        {
            var data = SystemInformation.ProcessExecution("bash", "./scripts/drivesinfo.sh");
            var disks = new List<Disk>();
            string[] driveInfo;
            var tempsInfo = GetDiskTemperature();
            foreach (var d in data)
			{
                Disk disk = new Disk();
                driveInfo = d.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                try
				{
                    disk.Label = driveInfo[0];
                    disk.Type = driveInfo[1];
                    disk.DriveFormat = driveInfo[2];
                    disk.TotalFreeSpace = Math.Round(Double.Parse(driveInfo[3]) / 1024.0, 2);
                    disk.TotalSize = Math.Round(Double.Parse(driveInfo[4]) / 1024.0, 2);
                    disk.UsedSpace = Math.Round(Double.Parse(driveInfo[5]) / 1024.0, 2);
                    disk.InstanceName = driveInfo[6];
				}
                catch (Exception e)
				{
                    Console.WriteLine("Ошибка получения данных о дисках: {0}", e);
				}
                foreach (var temp in tempsInfo)
				{
                    if (disk.InstanceName == temp.InstanceName)
					{
                        disk.Model = temp.Model;
                        disk.Temperature = temp.Temperature;
					}
				}
                disks.Add(disk);
			}
            return disks;
        }
        List<DiskTemp> GetDiskTemperature()
		{
            var disksTemp = new List<DiskTemp>();
            var data = SystemInformation.ProcessExecution("bash", "./scripts/drivestemp.sh");
            string[] driveTemp;
            foreach (var d in data)
            {
                DiskTemp temp = new DiskTemp();
                driveTemp = d.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    var culture = System.Globalization.CultureInfo.CurrentCulture;
                    if (culture.ToString().Equals("ru-RU"))
                    {
                        driveTemp[1] = driveTemp[1].Replace('.', ',');
                    }
                    temp.Model = driveTemp[0];
                    temp.Temperature = Double.Parse(driveTemp[1]);
                    temp.InstanceName = driveTemp[2];
                }
                catch (Exception e)
                {
                    temp.Temperature = 0;
                    Console.WriteLine("Ошибка получения температур дисков: {0}", e);
                }
                disksTemp.Add(temp);
            }
            return disksTemp;
		}
    }
}