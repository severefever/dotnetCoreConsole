using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dotnetCoreConsole
{
	public partial class SystemInformation
	{
		public List<USB> USBs { get; set; }
		public Memory RAM { get; set; }
		public CentralProcessorUnit CPU { get; set; }
		public List<Disk> Drives { get; set; }
		public OperatingSystem OS { get; set; }
		public Processes Procs { get; set; }
		public List<NetInts> NetAdapters { get; set; }

		public string SerializeData(SystemInformation systemToSerialize)
		{
			try
			{
				return JsonConvert.SerializeObject(systemToSerialize);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return null;
			}
		}
		public string SerializeAllData(SystemInformation systemToSerialize)
		{
			Parallel.Invoke(
				() => {	Procs = GetNumberOfProcesses();	},
				() => {	OS = GetOSInfo(); },
				() => { NetAdapters = GetNetworkInterfacesInfo(); },
				() => { USBs = GetUSBPortsInfo(); },
				() => { CPU = GetCPUInfo();	},
				() => { RAM = GetSystemMemoryInfo(); },
				() => { Drives = GetDiskInfo(); }
				);
			return SerializeData(systemToSerialize);
		}
		public string SerializeData(SystemInformation systemToSerialize, JsonSerializerSettings settings)
		{
			try
			{
				return JsonConvert.SerializeObject(systemToSerialize, settings);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return null;
			}
		}
		public string SerializeAllData(SystemInformation systemToSerialize, JsonSerializerSettings settings)
		{
			Parallel.Invoke(
				() => { Procs = GetNumberOfProcesses(); },
				() => { OS = GetOSInfo(); },
				() => { NetAdapters = GetNetworkInterfacesInfo(); },
				() => { USBs = GetUSBPortsInfo(); },
				() => { CPU = GetCPUInfo(); },
				() => { RAM = GetSystemMemoryInfo(); },
				() => { Drives = GetDiskInfo(); }
				);
			return SerializeData(systemToSerialize, settings);
		}
		public SystemInformation DeserializeData(string jsonToDeserialize)
		{
			try
			{
				return JsonConvert.DeserializeObject<SystemInformation>(jsonToDeserialize);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return null;
			}
		}
	}
}
