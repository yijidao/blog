namespace ThreadDemo3.SynchronousStructure;


/// <summary>
/// ManualResetEvent
/// - 实现原理：
///   跟 AutoResetEvent 一起继承自 EventWaitHandle，由内核维护一个 bool 值，当为 false 时，阻塞调用 Wait() 的线程，为 true 时，释放调用 Wait() 的线程。
/// 
/// - 跟 AutoResetEvent 的不同：
///   AutoResetEvent 调用 Set() 时，会释放一个被阻塞的线程，并且自动把 bool 值设置为 false，ManualResetEvent 会释放所有阻塞的线程，并且必须调用 Reset() 才会将 bool 值设置为 false。
///
/// - 其他注意点：
///   - 基于 windows 内核实现，所以可以使用 ManualResetEvent 来实现进程间或者跨 AppDomain 间的同步。
///   - 有个轻量化的实现，ManualResetEventSlim。
///   - ManualResetEvent 继承自 EventWaitHandle， EventWaitHandle 在构造函数传入名称，可以实现跨域跨进程同步。
/// </summary>
public class ManualResetEventDemo
{
    /// <summary>
    /// 测试 ManualResetEvent.Set() 和 ManualResetEvent.Reset()
    /// </summary>
    public void Test1()
    {
        

        var mre = new ManualResetEvent(false);

        foreach (var i in Enumerable.Range(1, 3))
        {
            StartThread(i);
        }
        Thread.Sleep(500);
        Console.WriteLine("按下 Enter 调用 Set()，释放所有线程");
        Console.ReadLine();
        mre.Set();
        Thread.Sleep(500);

        Console.WriteLine("ManualResetEvent 内部值为 true 时，不会阻塞线程。按下 Enter 启动一个新线程进行测试");
        Console.ReadLine();

        StartThread(4);
        Thread.Sleep(500);

        Console.WriteLine("按下 Enter 调用 Reset()，可以再次阻塞线程");
        Console.ReadLine();
        mre.Reset();
        Thread.Sleep(500);


        foreach (var i in Enumerable.Range(5, 2))
        {
            StartThread(i);
        }
        Thread.Sleep(500);

        Console.WriteLine("按下 Enter 调用 Set()，释放所有线程，结束 demo");
        Console.ReadLine();
        mre.Set();
        Thread.Sleep(500);


        void StartThread(int i)
        {
            var t = new Thread(() =>
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} 启动并调用 WaitOne()");
                mre.WaitOne();
                Console.WriteLine($"{Thread.CurrentThread.Name} 结束运行");
            })
            {
                Name = $"线程_{i}"
            };
            t.Start();
        }
    }


}