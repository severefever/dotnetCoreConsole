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
        public string model;
        public string label;
        public string type;
        public string driveFormat;
        public double totalFreeSpace;
        public double totalSize;
        public double usedSpace;
        public string instanceName;
        public double temperature;
    }
    struct DiskTemp
	{
        public string model;
        public string instanceName;
        public double temperature;
	}
    public struct OperatingSystem
    {
        public string description;
        public string architecture;
    }
    public struct NetInts
    {
        public string name;
        public NetworkInterfaceType type;
        public string description;
        public List<(string, string)> dns;
        public List<string> gateway;
        public List<(string, string)> unicast;
    }
    public struct USB
    {
        public string name;
        public string deviceID;
    }
    public struct Memory
	{
        public double total;
        public double used;
        public double free;
	}
    public struct CentralProcessorUnit
	{
        public string name;
        public int numberOfCores;
        public int numberOfLogicalProcessors;
        public double currentClockSpeed;
        public double loadPercentage;
        public double temperature;
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
            operatingSystem.description = RuntimeInformation.OSDescription;
            operatingSystem.architecture = RuntimeInformation.OSArchitecture.ToString();
            return operatingSystem;
        }
        public static int GetNumberOfProcesses()
        {
            return Process.GetProcesses().Length;
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
                adapterInfo.name = adapter.Name;
                adapterInfo.type = adapter.NetworkInterfaceType;
                adapterInfo.description = adapter.Description;
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;
                // DNS-адреса.
                if (properties.DnsAddresses.Count > 0)
                {
                    adapterInfo.dns = new List<(string, string)>();
                    foreach (var dnsAdd in properties.DnsAddresses)
                    {
                        adapterInfo.dns.Add((dnsAdd.ToString(), dnsAdd.MapToIPv4().ToString()));
                    }
                }
                // Адреса шлюзов.
                if (properties.GatewayAddresses.Count > 0)
                {
                    adapterInfo.gateway = new List<string>();
                    foreach (var gtwAdd in properties.GatewayAddresses)
                    {
                        adapterInfo.gateway.Add(gtwAdd.Address.ToString());
                    }
                }
                // Маски подсетей.
                if (properties.UnicastAddresses.Count > 0)
                {
                    adapterInfo.unicast = new List<(string, string)>();
                    foreach (var uni in properties.UnicastAddresses)
                    {
                        adapterInfo.unicast.Add((uni.IPv4Mask.ToString(), uni.Address.MapToIPv4().ToString()));
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
                    usb.name = mo["Name"].ToString();
                    usb.deviceID = mo["DeviceID"].ToString();
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
            memory.total = Math.Round(totalMemory);
            memory.free = Math.Round(availableMemory);
            memory.used = Math.Round(totalMemory - availableMemory);
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
                cpu.name = mo["Name"].ToString();
                cpu.numberOfCores = Int32.Parse(mo["NumberOfCores"].ToString());
                cpu.numberOfLogicalProcessors = Int32.Parse(mo["NumberOfLogicalProcessors"].ToString());
                cpu.currentClockSpeed = Double.Parse(mo["CurrentClockSpeed"].ToString());
                cpu.loadPercentage = Double.Parse(mo["LoadPercentage"].ToString());
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
                cpu.temperature = temp;
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
                        disk.type = Convert.ToString((DriveType)Int32.Parse(logDiskEnu["DriveType"].ToString()));
                        disk.model = moDisk["Model"].ToString();
                        disk.driveFormat = logDiskEnu["FileSystem"].ToString();
                        disk.totalFreeSpace = Math.Round(Double.Parse(logDiskEnu["FreeSpace"].ToString()) / 1073741824.0, 2);
                        disk.totalSize = Math.Round(Double.Parse(logDiskEnu["Size"].ToString()) / 1073741824.0, 2);
                        disk.usedSpace = Math.Round(disk.totalSize - disk.totalFreeSpace, 2);
                        disk.label = logDiskEnu["Name"].ToString();

                        if (tempList.Count > 0)
						{
                            foreach (var temps in tempList)
                            {
                                if (disk.model.Equals(temps.model))
                                {
                                    disk.instanceName = temps.instanceName;
                                    disk.temperature = temps.temperature;
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
                    disk.instanceName = instanceName;
                    disk.model = friendlyName;
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
                            disk.temperature = Convert.ToDouble(vendorSpec[i * 12 + tableRawValue]);
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
                        usb.name = d.Substring(i + 1);
                        usb.deviceID = d.Substring(0, i);
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
            var data = SystemInformation.ProcessExecution("bash", "../../../../meminfo.sh");
            memory.total = Double.Parse(data[0]);
            memory.free = Double.Parse(data[1]);
            memory.used = Double.Parse(data[2]);
            return memory;
		}
		public CentralProcessorUnit CPUInfo()
		{
            CentralProcessorUnit cpu = new CentralProcessorUnit();
			var data = SystemInformation.ProcessExecution("bash", "../../../../cpuinfo.sh");
            try
			{
                cpu.name = data[0];
                cpu.currentClockSpeed = Double.Parse(data[1]);
                cpu.numberOfCores = Int32.Parse(data[2]);
                cpu.numberOfLogicalProcessors = Int32.Parse(data[3]);
                cpu.loadPercentage = Double.Parse(data[4]);
                cpu.temperature = Double.Parse(data[5]);
            }
            catch (Exception)
			{
                cpu.temperature = 0;
                Console.WriteLine("Ошибка получения температуры процессора!");
            }
            return cpu;
		}
        public List<Disk> DiskInfo()
        {
            var data = SystemInformation.ProcessExecution("bash", "../../../../drivesinfo.sh");
            var disks = new List<Disk>();
            string[] driveInfo;
            var tempsInfo = GetDiskTemperature();
            foreach (var d in data)
			{
                Disk disk = new Disk();
                driveInfo = d.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                try
				{
                    disk.label = driveInfo[0];
                    disk.type = driveInfo[1];
                    disk.driveFormat = driveInfo[2];
                    disk.totalFreeSpace = Double.Parse(driveInfo[3]);
                    disk.totalSize = Double.Parse(driveInfo[4]);
                    disk.usedSpace = Double.Parse(driveInfo[5]);
                    disk.instanceName = driveInfo[6];
				}
                catch (Exception e)
				{
                    Console.WriteLine("Ошибка получения данных о дисках: {0}", e);
				}
                foreach (var temp in tempsInfo)
				{
                    if (disk.instanceName == temp.instanceName)
					{
                        disk.model = temp.model;
                        disk.temperature = temp.temperature;
					}
				}
                disks.Add(disk);
			}
            return disks;
        }
        List<DiskTemp> GetDiskTemperature()
		{
            var disksTemp = new List<DiskTemp>();
            var data = SystemInformation.ProcessExecution("bash", "../../../../drivestemp.sh");
            string[] driveTemp;
            foreach (var d in data)
            {
                DiskTemp temp = new DiskTemp();
                driveTemp = d.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    temp.model = driveTemp[0];
                    temp.temperature = Double.Parse(driveTemp[1]);
                    temp.instanceName = driveTemp[2];
                }
                catch (Exception e)
                {
                    temp.temperature = 0;
                    Console.WriteLine("Ошибка получения температур дисков: {0}", e);
                }
                disksTemp.Add(temp);
            }
            return disksTemp;
		}
    }
}