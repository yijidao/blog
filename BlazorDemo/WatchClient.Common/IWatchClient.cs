public interface IWatchClient
{
    public Task<bool> Run();

    public Task<bool> Shutdown();

    public Task<WatchClientInfo> UploadClientInfo();
}