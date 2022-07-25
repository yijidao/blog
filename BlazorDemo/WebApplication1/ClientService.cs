using System.Diagnostics;
using System.Net.NetworkInformation;
using Hardware.Info;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using WebApplication1.Options;

namespace WebApplication1;

public class ClientService : IWatchClient
{
    private readonly IOptions<HjmosClientOptions> _hjmosClientOptions;


    public ClientService(IOptions<HjmosClientOptions> hjmosClientOptions)
    {
        _hjmosClientOptions = hjmosClientOptions;
    }

    private async Task Init()
    {


        var connection = new HubConnectionBuilder()
            .WithUrl("http://")
            .Build();

    }

    public Task<bool> Run()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Shutdown()
    {
        throw new NotImplementedException();
    }

    public async Task<WatchClientInfo> UploadClientInfo()
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
            Name = _hjmosClientOptions.Value.Name,
            HjmosClientPath = _hjmosClientOptions.Value.Path,
            //HjmosClientIsRunning = Process.GetProcessesByName()
        };
        return watchClientInfo;
    }

}