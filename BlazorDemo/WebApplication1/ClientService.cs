using System.Net.NetworkInformation;
using Hardware.Info;

namespace WebApplication1;

public class ClientService : IWatchClient
{

    public ClientService()
    {

    }

    private async Task Init()
    {
        var hardwareInfo = new HardwareInfo();

        hardwareInfo.RefreshAll();
        var watchClientInfo = new WatchClientInfo
        {
            DriveList = hardwareInfo.DriveList,
            BatteryList = hardwareInfo.BatteryList,
            BiosList = hardwareInfo.BiosList,
            CpuList = hardwareInfo.CpuList,
            KeyboardList = hardwareInfo.KeyboardList,
            MemoryList = hardwareInfo.MemoryList,
            MonitorList = hardwareInfo.MonitorList,
            MotherboardList = hardwareInfo.MotherboardList,
            IpAddresses = HardwareInfo.GetLocalIPv4Addresses().ToList(),
            MouseList = hardwareInfo.MouseList,
            NetworkAdapterList = hardwareInfo.NetworkAdapterList,
            PrinterList = hardwareInfo.PrinterList,
            SoundDeviceList = hardwareInfo.SoundDeviceList,
            VideoControllerList = hardwareInfo.VideoControllerList,
        };



    }

    public Task<bool> Run()
    {
        throw new NotImplementedException();
    }

    Task<bool> IWatchClient.Shutdown()
    {
        throw new NotImplementedException();
    }

    public Task<WatchClientInfo> GetWatchClientInfo()
    {
        throw new NotImplementedException();
    }

}