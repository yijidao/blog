using System.Diagnostics;

namespace ThreadDemo3.SynchronousStructure;


/// <summary>
/// Mutex 是一个内核模式构造
/// 支持重入锁、线程所有权、跨 AppDomain 同步、跨进程同步
///
/// 在构造函数中传入名称可以把 mutex 设置为系统级别的锁，从而跨域跨进程。
/// - 当 Mutex.WaitOne() 但是拥有 Mutex 的进程被中止掉了，如任务管理器直接杀掉进程，那样正在等待 Mutex 的线程就会抛出 System.Threading.AbandonedMutexException
/// </summary>
public class MutexDemo
{
    /// <summary>
    /// Mutex 支持重入锁，支持线程一致
    /// </summary>
    public void Test()
    {
        var mutex = new Mutex(false);
        var count = 0;
        DoWork(mutex);

        void DoWork(Mutex mutex)
        {
            try
            {
                mutex.WaitOne();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 获取 Mutex");
                Interlocked.Increment(ref count);
                Thread.Sleep(1000);
                if (Interlocked.CompareExchange(ref count, 3, 3) == 3)
                {
                    return;
                }
                DoWork(mutex);
            }
            finally
            {
                mutex.ReleaseMutex(); // 调用几次 WaitOne() 就必须调用几次 ReleaseMutex()，并且调用 WaitOne() 和 ReleaseMutex() 必须在同一个线程。
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 释放 Mutex");
            }
        }
    }

    /// <summary>
    /// 在构造函数中传入名称可以把 mutex 设置为系统级别的锁，从而跨域跨进程。
    /// </summary>
    public void Test2()
    {
        try
        {
            using var mutex = new Mutex(false, "Mutex1");
            mutex.WaitOne();
            Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} {DateTime.Now:yyyy-MM-dd HH:mm:ss} 获取 Mutex");
            Thread.Sleep(5000);
            mutex.ReleaseMutex();
            Console.WriteLine($"{Process.GetCurrentProcess().ProcessName} {DateTime.Now:yyyy-MM-dd HH:mm:ss} 释放 Mutex");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}