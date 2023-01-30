namespace ThreadDemo3.UsingThread;

/// <summary>
/// 取消正在执行的线程有多种方法
/// - Thread.Interrupt()
/// - WaitHandle Timeout
/// - CancellationToken
/// - CancellationTokenSource.CreateLinkedTokenSource()
/// </summary>
public class CancelThreadDemo
{
    /// <summary>
    /// 使用 Thread.Interrupt() 取消处于 Wait state（调用了 Sleep() 或正被锁阻塞） 的线程，如果线程正常执行，即使死循环了，也无法中断。
    /// </summary>
    public void Test()
    {
        var pool = new SemaphoreSlim(0, 1);

        var t = new Thread(DoWork);
        var t2 = new Thread(DoWork);
        t.Start();
        t2.Start();

        Thread.Sleep(2000);
        t.Interrupt();
        Thread.Sleep(2000);
        t2.Interrupt();

        void DoWork()
        {
            try
            {
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void DoWork2()
        {
            try
            {
                pool.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    /// <summary>
    /// 使用 CancellationToken 取消处于死循环的线程，或者超时取消
    /// </summary>
    public void Test2()
    {
        var cts = new CancellationTokenSource(5000);

        Task.Run(() =>
        {
            Console.WriteLine("按下 c 取消线程，或者五秒后取消");

            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                cts.Cancel();
            }
        });
        var t = new Thread(DoWork);
        t.Start(cts.Token);

        void DoWork(object? state)
        {
            var ct = (CancellationToken)state;
            while (!ct.IsCancellationRequested)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 自旋");
                Thread.SpinWait(10000000);
            }

            Console.WriteLine("结束执行");
        }
    }

    /// <summary>
    /// 使用 WaitHandle.WaitAny 取消被阻塞的线程，或者超时取消，或者使用 CancellationToken 协助式取消
    /// </summary>
    public void Test3()
    {
        var pool = new Semaphore(0, 1);
        var cts = new CancellationTokenSource();

        Task.Run(() =>
        {
            Console.WriteLine("按下 c 调用 CancellationTokenSource.Cancel() 取消线程，或者按下 v 调用 Semaphore.Release() 取消线程，或者五秒后取消");

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.C:
                    cts.Cancel();
                    break;
                case ConsoleKey.V:
                    pool.Release();
                    break;
            }

            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                cts.Cancel();
            }
        });

        var t = new Thread(DoWork);
        t.Start();

        void DoWork()
        {
            var signalIndex = WaitHandle.WaitAny(new WaitHandle[] { pool, cts.Token.WaitHandle }, 5000);

            if (signalIndex == 0)
            {
                Console.WriteLine("调用 Semaphore.Release() 取消线程");
            }
            else if (cts.Token.IsCancellationRequested)
            {
                Console.WriteLine("CancellationTokenSource.Cancel() 取消线程");
            }
            else if (signalIndex == WaitHandle.WaitTimeout)
            {
                Console.WriteLine("超时取消");
            }
            Console.WriteLine("结束运行");
        }
    }

    /// <summary>
    /// 使用多个 CancellationToken 取消处于死循环的线程
    /// </summary>
    public void Test4()
    {
        var externalCts = new CancellationTokenSource();
        Task.Run(() =>
        {
            Console.WriteLine("按下 c 取消线程，或者五秒后取消");

            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                externalCts.Cancel();
            }
        });


        var t = new Thread(DoWork);
        t.Start();

        void DoWork()
        {
            var internalCts = new CancellationTokenSource(5000);

            var linkCts = CancellationTokenSource.CreateLinkedTokenSource(externalCts.Token, internalCts.Token);

            try
            {
                while (true)
                {

                    linkCts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 自旋");
                    Thread.SpinWait(10000000);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}