namespace ThreadDemo3;

/// <summary>
/// 一个使用 Interlocked 来实现的，并发性能很高的测试类
/// 可以发起多个 web 请求，在所有请求完成或者取消后，打印一次报告，并且支持超时机制。
/// </summary>
public class MultiWebRequests
{
    private AsyncCoordinator _ac = new();

    private Dictionary<string, object?> _servers;

    private MultiWebRequests(int timeout = Timeout.Infinite, params string[] servers)
    {
        _servers = servers.ToDictionary(x => x, _ => (object?)null);
    }

    public static void Start(int timeout = Timeout.Infinite, params string[] servers)
    {
        var multiWebRequests = new MultiWebRequests(timeout, servers);
        var httpClient = new HttpClient();
        foreach (var server in multiWebRequests._servers.Keys)
        {
            multiWebRequests._ac.AboutToBegin();
            httpClient.GetByteArrayAsync(server)
                .ContinueWith(t => multiWebRequests.ComputeResult(server, t));
        }
        multiWebRequests._ac.AllBegun(multiWebRequests.AllDone);
    }

    private void Cancel() => _ac.Cancel();

    private void AllDone(CoordinationStatus status)
    {
        switch (status)
        {
            case CoordinationStatus.Cancel:
                Console.WriteLine("请求已取消");
                break;
            case CoordinationStatus.Timeout:
                Console.WriteLine("请求已超时");
                break;
            case CoordinationStatus.AllDone:
                Console.WriteLine("请求输出如下");
                foreach (var server in _servers)
                {
                    var msg = server.Value is Exception
                        ? $"请求发生异常{server.Value.GetType().Name}"
                        : $"返回{server.Value}字节";
                    Console.WriteLine($"{server.Key} {msg}");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }


    private void ComputeResult(string server, Task<byte[]> task)
    {
        object result = null;
        if (task.Exception != null)
        {
            result = task.Exception;
        }
        else
        {
            result = task.Result.Length;
        }

        _servers[server] = result;
        _ac.JustEnded();
    }

}

class AsyncCoordinator
{
    private int _opCount = 1, _statusReported;
    private Action<CoordinationStatus> _callback;
    private Timer _timer;

    public void AboutToBegin(int opsToAdd = 1)
    {
        Interlocked.Add(ref _opCount, opsToAdd);
    }

    public void JustEnded()
    {
        if (Interlocked.Decrement(ref _opCount) == 0)
        {
            ReportStatus(CoordinationStatus.AllDone);
        }
    }

    public void AllBegun(Action<CoordinationStatus> callback, int timeout = Timeout.Infinite)
    {
        _callback = callback;
        if (timeout != Timeout.Infinite)
        {
            _timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
        }
        JustEnded();
    }

    private void TimeExpired(object? o) => ReportStatus(CoordinationStatus.Timeout);

    public void Cancel() => ReportStatus(CoordinationStatus.Cancel);

    private void ReportStatus(CoordinationStatus status)
    {
        // 只输出一次报告
        if (Interlocked.Exchange(ref _statusReported, 1) == 0)
        {
            _callback(status);
        }
    }

}

enum CoordinationStatus
{
    Cancel,
    Timeout,
    AllDone
}