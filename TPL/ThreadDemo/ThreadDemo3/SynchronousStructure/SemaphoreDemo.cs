namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// Semaphore 内部维护一个数值，当数值为 0 时，阻塞调用 WaitOne() 的线程，当不为零时，允许调用 WaitOne() 的线程访问共享资源，同时把数值 -1。当调用 Release() 时，把数值 +1
/// Semaphore 是一个内核构造。可以实现系统范围内或者单个进程内访问同步访问一个共享资源池。
/// SemaphoreSlim 是一个混合构造。是 Semaphore 的轻量版，只能实现单个进程内同步访问一个共享资源池。
///
/// Semaphore 和 SemaphoreSlim 不支持线程一致，所以可以在不同线程对同一个 Semaphore 实例调用 WaitOne() 或 Release()
/// </summary>
public class SemaphoreDemo
{
    /// <summary>
    /// 测试在不同线程调用 WaitOne() Release()
    /// </summary>
    public void Test()
    {
        try
        {
            var pool = new Semaphore(0, 2);
            var doWork = new Action(() =>
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 WaitOne()");
                pool.WaitOne();
                Thread.Sleep(1000);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 完成工作");
            });

            var t = Task.Run(doWork);
            var t2 = Task.Run(doWork);

            Thread.Sleep(2000);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 Release()");
            pool.Release(2);

            Task.WaitAll(t, t2);
            //pool.Release(2);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// 测试跟其他进程通讯
    /// </summary>
    public void Test5()
    {
        Semaphore pool;
        if (Semaphore.TryOpenExisting("multi-process", out pool))
        {
            Console.WriteLine("等待 Semaphore");
            pool.WaitOne();
            Console.WriteLine("结束");
        }
        else
        {
            pool = new Semaphore(0, 1, "multi-process");

            while (true)
            {
                Console.WriteLine("按下 Enter 跟其他线程通讯");
                Console.ReadLine();
                pool.Release();
            }
        }
    }

    /// <summary>
    /// 测试 Semaphore
    /// </summary>
    public void Test4()
    {
        var pool = new Semaphore(1, 3); // 初始化计数 1，最大计数 3

        foreach (var i in Enumerable.Range(1, 5))
        {
            var t = new Thread(DoWork);
            t.Name = $"线程 {i}";
            t.Start();
        }

        Thread.Sleep(500);
        Console.WriteLine("按下 Enter 释放 3 个线程");
        Console.ReadLine();
        pool.Release(3); // 计数加3
        Thread.Sleep(500);
        Console.WriteLine("再按下 Enter 释放 1 个线程");
        Console.ReadLine();
        pool.Release(); // 计数加1

        void DoWork()
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} 开始");
            pool.WaitOne(); // 计数减1
            Console.WriteLine($"{Thread.CurrentThread.Name} 结束");
        }
    }

}