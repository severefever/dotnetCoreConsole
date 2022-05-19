using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dotnetCoreConsole
{
	using static SystemInformation;
	public class SystemData
	{
		public List<USB> USBs { get; set; }
		public Memory RAM { get; set; }
		public CentralProcessorUnit CPU { get; set; }
		public List<Disk> Drives { get; set; }
		//
		public OperatingSystem OS { get; set; }
		public Processes Procs { get; set; }
		public List<NetInts> NetAdapters { get; set; }
		public SystemData() { }
		public SystemData(SystemInformation userSystem)
		{
			Parallel.Invoke(
				() => { Procs = GetNumberOfProcesses(); },
				() => {	OS = GetOSInfo(); },
				() => {	NetAdapters = GetNetworkInterfacesInfo(); },
				() => {	USBs = userSystem.GetUSBPortsInfo(); },
				() => {	CPU = userSystem.GetCPUInfo(); },
				() => {	RAM = userSystem.GetSystemMemoryInfo();	},
				() => {	Drives = userSystem.GetDiskInfo(); }
			);
		}
	}
}
