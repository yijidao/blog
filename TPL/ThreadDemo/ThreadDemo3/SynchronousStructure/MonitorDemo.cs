namespace ThreadDemo3.SynchronousStructure;
/// <summary>
/// Monitor 是一个互斥锁，一般用来锁定代码块、实例方法、静态方法，lock(object) 就是基于 Monitor 实现。
/// Monitor 是一个静态类
/// Monitor 是一个最常用的同步混合构造，提供了自旋、线程所有权、线程递归的功能。
/// 跟 SpinLock 等使用场景有所不同，SpinLock 一般用来锁定具体的共享资源对象。
///
/// 注意点：
/// - Monitor.Enter(字符串)，因为字符串有留用，所以会导致不同线程、不同 AppDomain 之间的变成互斥访问。
/// - Monitor.Enter(值类型)，因为Monitor.Enter() 只接收引用类型，所以值类型就必须装箱，这里就涉及到装箱拆箱的问题，可能导致无法互斥访问。
/// - Monitor.Enter()，需要关注关注程序集中立加载的问题，可能会导致不同 AppDomain 之间变成互斥访问。
/// - Monitor.Enter() 有线程所有权功能，所以 Enter 和 Exit 必须在同一个线程。
/// </summary>
public class MonitorDemo
{
    /// <summary>
    /// 测试 Monitor.Wait(object)、Monitor.Pulse(object)、Monitor.PulseAll(object)
    /// 注意点：
    /// 调用 Wait()、Pulse()、PulseAll() 也必须先调用 Enter() 获取锁，退出的时候也必须调用 Exit() 释放锁
    /// </summary>
    public void Test()
    {
        var lockObj = new object();

        Task.Factory.StartNew(() =>
        {
            Thread.Sleep(500);
            Console.WriteLine("按下 c 调用 Monitor.Pulse(object)");

            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                try
                {
                    Monitor.Enter(lockObj);
                    Monitor.Pulse(lockObj);
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
            }
            Thread.Sleep(500);

            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                try
                {
                    Monitor.Enter(lockObj);
                    Monitor.PulseAll(lockObj);
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
            }
        });

        Parallel.Invoke(DoWork, DoWork, DoWork);

        void DoWork()
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 启动");
            try
            {
                Monitor.Enter(lockObj);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 获得 Monitor");
                Thread.Sleep(100);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 Monitor.Wait()");
                Monitor.Wait(lockObj);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 重新获得 Monitor");

            }
            finally
            {
                Monitor.Exit(lockObj);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 释放 Monitor");
            }
        }
    }


    /// <summary>
    /// 测试 Monitor.Enter(字符串)
    /// 因为字符串会被留用，所以会导致不同线程间互斥访问。
    /// </summary>
    public void Test2()
    {
        var mre = new ManualResetEventSlim(false);

        Task.Run(() =>
        {
            Console.WriteLine("按下 c 启动");
            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                mre.Set();
            }
        });

        Parallel.Invoke(DoWork, DoWork, DoWork);


        void DoWork()
        {
            mre.Wait();

            try
            {
                Monitor.Enter("1");
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 进入同步代码块");
                Thread.Sleep(1000);

            }
            finally
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 退出同步代码块");
                Monitor.Exit("1");
            }

        }
    }

    /// <summary>
    /// 测试 Monitor.Enter(值类型)
    /// 因为 Monitor.Enter(object) 参数是 object，所以值类型必须装箱，那样其实就会有问题了。
    /// 值类型在堆栈上，没有引用，引用类型在堆上，有引用，所以装箱就是在堆上新建一个实例，然后复制栈上值的内容，拆箱就是把堆上实例的值，复制到栈上。
    /// </summary>
    public void Test3()
    {
        var mre = new ManualResetEventSlim(false);
        var i = 1;
        //Object o = i;


        Task.Run(() =>
        {
            Console.WriteLine("按下 c 启动");
            if (Console.ReadKey().Key == ConsoleKey.C)
            {
                mre.Set();
            }
        });

        Parallel.Invoke(DoWork, DoWork, DoWork);

        void DoWork()
        {
            mre.Wait();
            object o = i;
            try
            {
                Monitor.Enter(o);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 进入同步代码块");
                Thread.Sleep(1000);
            }
            finally
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 退出同步代码块");
                Monitor.Exit(o);
            }

        }
    }
}

