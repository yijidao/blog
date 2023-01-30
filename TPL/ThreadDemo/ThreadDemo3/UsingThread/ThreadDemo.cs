using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.UsingThread
{
    /// <summary>
    /// ### 线程控制权、自旋
    /// Thread.Yield() 让出控制权给CPU上的其他线程。
    /// Thread.Sleep(0) 让出控制权给同等优先级的线程执行，如果没有，就继续执行本线程。
    /// Thread.Sleep(1) 让出控制权给正在等待的线程执行。
    /// Thread.SpinWait() 不让出控制权，在CPU上自旋一段时间。
    ///
    /// ### 线程插槽、ThreadStaticAttribute
    /// Thread.AllocateDataSlot() Thread.AllocateDataSlot() 可以给线程设置一个插槽，插槽里面的值是线程相关的，也就是每个线程特有的，同一个变量不同线程无法互相修改。一般在静态构造方法中初始化。
    /// Thread.GetData() Thread.SetData() 可以对插槽取值和赋值
    /// ThreadStaticAttribute 标记静态变量时，该变量是线程相关的，不同线程的静态变量值是不一样的
    /// ### ThreadStaticAttribute 和插槽的不同：
    /// - 插槽是动态的，在运行时进行赋值的，而且 Thread.GetData() 返回值是 object，如果线程所需的值类型不固定，可以使用插槽。
    /// - [ThreadStatic] IDE 可以提供编译检查，性能和安全性更好，如果线程所需的值类型是固定的，就应该使用 [ThreadStatic]。
    /// ### 注意点：
    /// 插槽和 [ThreadStatic] 中的值一般不初始化，因为跟线程相关，在哪个线程初始化，只有那个线程可以看到这个初始化后的值，所以初始化也就没啥意义了。
    ///
    /// ### Thread.Interrupt()
    /// 当线程处于 Wait State 时（也就是调用了 Sleep() 或者调用 WaitOne() 被阻塞），会中断线程，并抛出 System.Threading.ThreadInterruptedException
    /// 如果线程正常运行，如死循环，那么 Interrupt() 就不会中断线程。
    /// 处于死循环的线程一般使用 CancellationToken 来协助式取消。
    /// 
    /// </summary>
    internal class ThreadDemo
    {
        /// <summary>
        /// 测试 ThreadStaticAttribute
        /// </summary>
        public void Test()
        {
            Parallel.Invoke(StaticThreadDemo.Test, StaticThreadDemo.Test, StaticThreadDemo.Test); // 打印对应线程的ID，证明被 [ThreadStatic] 标记过的字段是线程相关的。
        }

        /// <summary>
        /// 测试 LocalDataStoreSlot
        /// </summary>
        public void Test2()
        {
            Parallel.Invoke(StaticThreadDemo.Test2, StaticThreadDemo.Test2, StaticThreadDemo.Test2); // 打印对应线程的ID，证明 LocalDataStoreSlot 是线程相关的。
        }

        /// <summary>
        /// 测试 Thread.Interrupt()
        /// </summary>
        public void Test3()
        {
            var sleepSwitch = false;
            var pool = new SemaphoreSlim(0, 1);

            var t = new Thread(DoWork);
            t.Start();
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 {t.ManagedThreadId} 的 Interrupt()");
            t.Interrupt();
            Thread.Sleep(3000);
            sleepSwitch = true;

            var t2 = new Thread(DoWork2);
            t2.Start();
            Thread.Sleep(2000);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 {t2.ManagedThreadId} 的 Interrupt()");
            t2.Interrupt();

            void DoWork()
            {
                try
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 开始执行");
                    while (!sleepSwitch)
                    {
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 自旋 SpinWait()");
                        Thread.SpinWait(10000000);
                    }
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 休眠 Sleep()");
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
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 开始执行");
                    pool.Wait();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        public void Test4()
        {
            var pool = new SemaphoreSlim(0, 1);
            var t = new Thread(DoWork);
            var t2 = new Thread(DoWork2);

            void DoWork()
            {
                pool.Wait(); // 等待信号量
            }
            void DoWork2()
            {
                Thread.Sleep(Timeout.Infinite); // 永久休眠
            }
        }
    }


    static class StaticThreadDemo
    {
        [ThreadStatic]
        private static int? _threadId = null;

        public static void Test()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            Thread.Sleep(500);
            Console.WriteLine($"ThreadId:{Thread.CurrentThread.ManagedThreadId}  ThreadStatic: {_threadId}");
        }

        private static LocalDataStoreSlot _localSlot;

        static StaticThreadDemo()
        {
            _localSlot = Thread.AllocateDataSlot();
            
        }

        public static void Test2()
        {
            Thread.SetData(_localSlot, Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(500);
            Console.WriteLine($"ThreadId:{Thread.CurrentThread.ManagedThreadId} LocalSlot:{Thread.GetData(_localSlot)}");
        }
    }
}
