namespace ThreadDemo3.SynchronousStructure;

public class WaitHandleDemo
{

    /// <summary>
    /// 测试 WaitHandle.WaitAll()， 成功运行返回 true, 支持超时，当超时时，返回 false
    ///  WaitHandle.WaitAny()， 成功运行返回对应的 索引，支持超时，当超时时，返回 WaitHandle.WaitTimeout
    /// </summary>
    public void Test()
    {
        var waitHandleList = new WaitHandle[] { new AutoResetEvent(false), new AutoResetEvent(false) };

        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[0]);
        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[1]);
        var timeout = WaitHandle.WaitAll(waitHandleList);
        Console.WriteLine($"是否超时：{!timeout}，WaitHandle.WaitAll() 结束");

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[0]);
        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[1]);
        timeout = WaitHandle.WaitAll(waitHandleList,1000);
        Console.WriteLine($"是否超时：{!timeout}，WaitHandle.WaitAll() 结束");

        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[0]);
        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[1]);
        var index = WaitHandle.WaitAny(waitHandleList);
        Console.WriteLine($"{index} 已经结束运行，WaitHandle.WaitAny() 结束");

        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[0]);
        ThreadPool.QueueUserWorkItem(DoWork, waitHandleList[1]);
        index = WaitHandle.WaitAny(waitHandleList, 1000);
        Console.WriteLine($"是否超时：{WaitHandle.WaitTimeout == index}，WaitHandle.WaitAny() 结束");
        

        void DoWork(object? state)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 开始");

            var r = new Random();
            var interval = 1000 * r.Next(2, 10);
            Thread.Sleep(interval);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 结束");

            ((AutoResetEvent)state).Set();
        }
    }

    /// <summary>
    /// 测试 WaitHandle.SignalAndWait()， 成功运行返回 true, 支持超时，当超时时，返回 false
    /// </summary>
    public void Test2()
    {
        var are = new AutoResetEvent(false);
        var are2 = new AutoResetEvent(false);

        foreach (var i in Enumerable.Range(1,5))
        {
            Console.WriteLine($"按下 Enter 启动线程 {i}");
            Console.ReadLine();
            var t = new Thread(DoWork)
            {
                Name = $"线程 {i}"
            };
            t.Start();
            WaitHandle.SignalAndWait(are, are2); // 给 are 发信号，同时等待 are2
        }

        Console.WriteLine("全部线程运行结束");

        void DoWork()
        {
            are.WaitOne();
            Console.WriteLine($"{Thread.CurrentThread.Name} 开始");
            Thread.Sleep(1000);
            Console.WriteLine($"{Thread.CurrentThread.Name} 结束");
            are2.Set();
        }
    }

    public void Test3()
    {

    }
}