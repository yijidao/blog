using System.Collections.Concurrent;

namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// CountdownEvent 是一个混合构造，经常用于 fork/join 等场景。就是等待多个并行任务完成，再执行下一个任务
/// CountdownEvent 内部会维护一个计数，当计数为 0 时，会 signal 调用 WaitOne 的线程，解除线程的阻塞。
///
/// 注意点：
/// - 支持超时和CancellationToken
/// - 调用 Reset(int) 可以重新初始化 CountdownEvent
/// - 调用 Signal() Signal(int count) 把计数 -1 或 -count
/// - 调用 AddCount() AddCount(int) 把计数 +1 或 +count
/// </summary>

public class CountdownEventDemo
{
    public void Test()
    {
        var cde = new CountdownEvent(1);

        foreach (var _ in Enumerable.Range(0, 10))
        {
            cde.AddCount();
            Task.Run(() => DoWork(cde));
        }
        cde.Signal();
        cde.Wait();
        Console.WriteLine("CountDownEvent 结束"); // join

        // fork
        void DoWork(CountdownEvent cde)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 Signal()");
            cde.Signal();

        }
    }

    public void Test2()
    {
        var queue = new ConcurrentQueue<int>(Enumerable.Range(1, 100));
        var cde = new CountdownEvent(queue.Count);

        var doWork = new Action(() =>
        {
            while (queue.TryDequeue(out var result))
            {
                Thread.Sleep(100);
                Console.WriteLine(result);
                cde.Signal();
            }
        });

        var _ = Task.Run(doWork); // fork
        var _2 = Task.Run(doWork); // fork


        var complete = new Action(() =>
        {
            cde.Wait(); // join
            Console.WriteLine($"queue Count {queue.Count}");
        });

        var t = Task.Run(complete);
        var t2 = Task.Run(complete);

        Task.WaitAll(t, t2);


        Console.WriteLine($"CountdownEvent 重新初始化");
        cde.Reset(2); // 调用 Reset() 将 cde 重新初始化
        cde.AddCount(10); // 调用 AddCount() cde 内部计数 + 1
        var cts = new CancellationTokenSource(1000);

        try
        {
            cde.Wait(cts.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        cde.Dispose();

    }
}