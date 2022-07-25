using System.Net;
using Hardware.Info;
using Monitor = Hardware.Info.Monitor;

public class WatchClientInfo
{
    public List<Battery> BatteryList { get; set; } = new();

    public List<BIOS> BiosList { get; set; } = new();

    public List<CPU> CpuList { get; set; } = new();

    public List<Drive> DriveList { get; set; } = new();

    public List<Keyboard> KeyboardList { get; set; } = new();

    public List<Memory> MemoryList { get; set; } = new();

    public List<Monitor> MonitorList { get; set; } = new();

    public List<Motherboard> MotherboardList { get; set; } = new();

    public List<Mouse> MouseList { get; set; } = new();

    public List<NetworkAdapter> NetworkAdapterList { get; set; } = new();

    public List<Printer> PrinterList { get; set; } = new();

    public List<SoundDevice> SoundDeviceList { get; set; } = new();

    public List<VideoController> VideoControllerList { get; set; } = new();

    public List<IPAddress> IpAddresses { get; set; } = new();

    public string HjmosClientPath { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool HjmosClientIsRunning { get; set; }
}