using System.Diagnostics;
using System.Text;

namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// SpinLock 自旋锁，是一个用户构造，使用 SpinWait 实现。
/// 一般自旋锁会一直在 CPU 上空转，所以会浪费 CPU 时间，也会影响 CPU 去调度其他线程，
/// 但是 SpinLock 实现更加优秀，内部使用了 SpinWait 实现，所以自旋一段时间后，就会把 CPU 让出去，让其他线程也有机会使用 CPU。
///
/// SpinLock 不支持重入锁（Re-enter），当一个线程反复获取同一个锁时，会死锁（Deadlock），启用线程跟踪(SpinLock(true))可以在重入锁时抛出异常，方便定位问题。
/// 
/// 注意点：
/// - new SpinLock(false) 不启用线程跟踪，会提高性能。 // 注意：线程跟踪跟线程所有权不是一个概念，线程所有权是指一个锁获取和释放必须在同一个线程，SpinLock 的线程跟踪只是为了方便调试死锁的问题。
/// - SpinLock.Exit(false) 不启用内存栅栏，也会提高性能。
/// - SpinLock.IsHeldByCurrentThread 可以判断锁是否被当前线程持有，SpinLock.IsHeld 可以判断锁是否被任意线程持有。
/// - SpinLock.Enter(ref bool) 获取锁之后，不使用时应该调用 SpinLock.Exit(bool) 释放锁，一般在 try{}finally{} 中操作。
/// </summary>
public class SpinLockDemo
{
    /// <summary>
    /// 测试 SpinLock 和 Lock（也就是 Monitor，Monitor 是混合构造，其实应该用内核构造来测试更明显）的性能差距
    /// SpinLock 在启用线程跟踪权之后，性能会降低。
    /// SpinLock.Exit(false) 传false 也会提高性能。
    /// </summary>
    public void Test()
    {

        var count = 100_000;
        var queue = new Queue<int>();
        var lockObj = new object();
        var spinLock = new SpinLock(false);

        UseLock();
        queue.Clear();
        UseSpinLock();

        void UpdateWithSpinLock(int i)
        {
            var lockToken = false;

            try
            {
                spinLock.Enter(ref lockToken);
                queue.Enqueue(i);
            }
            finally
            {
                spinLock.Exit(false);
            }
        }

        void UseSpinLock()
        {
            var sw = Stopwatch.StartNew();
            Parallel.Invoke(() =>
            {
                foreach (var i in Enumerable.Range(0, count))
                {
                    UpdateWithSpinLock(i);
                }
            }, () =>
            {
                foreach (var i in Enumerable.Range(0, count))
                {
                    UpdateWithSpinLock(i);
                }
            });
            sw.Stop();
            Console.WriteLine($"SpinLock 耗时：{sw.ElapsedMilliseconds} ms");
        }

        void UpdateWithLock(int i)
        {
            lock (lockObj)
            {
                queue.Enqueue(i);
            }
        }

        void UseLock()
        {
            var sw = Stopwatch.StartNew();
            Parallel.Invoke(() =>
            {
                foreach (var i in Enumerable.Range(0, count))
                {
                    UpdateWithLock(i);
                }
            }, () =>
            {
                foreach (var i in Enumerable.Range(0, count))
                {
                    UpdateWithLock(i);
                }
            });
            sw.Stop();
            Console.WriteLine($"Lock 耗时：{sw.ElapsedMilliseconds} ms");
        }

    }

    /// <summary>
    /// 测试重入锁
    /// </summary>
    public void Test2()
    {
        var spinLock = new SpinLock(false);

        Parallel.Invoke(DoWork, DoWork, DoWork);


        void DoWork()
        {
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            var sb = new StringBuilder();
            foreach (var i in Enumerable.Range(0, 100))
            {
                var lockToken = false;
                try
                {
                    //if (!spinLock.IsHeldByCurrentThread) // IsHeldByCurrentThread 可以确定锁是否被当前线程使用
                    //{
                    //    spinLock.Enter(ref lockToken);
                    //}
                    
                    spinLock.Enter(ref lockToken);

                    Thread.SpinWait(50_000);
                    sb.Append(Thread.CurrentThread.ManagedThreadId);
                    sb.Append("  Enter-");
                }
                catch (LockRecursionException e)
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 重入锁异常");
                    throw;
                }
                finally
                {
                    if (lockToken)
                    {
                        spinLock.Exit(false);
                        sb.Append("Exited ");
                    }
                }

                if (i % 4 != 0)
                {
                    Console.Write(sb.ToString());
                }
                else
                {
                    Console.WriteLine(sb.ToString());
                }

                sb.Clear();
            }
            //spinLock.Exit(false);
        }
    }
}