using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace dotnetCoreConsole
{
    public struct Processes
    {
        public int NumberOfProcesses { get; set; }
    }
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
}
