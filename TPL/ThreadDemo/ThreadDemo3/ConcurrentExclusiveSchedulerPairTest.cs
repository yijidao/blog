using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// 异步的同步构造
    /// </summary>
    internal class ConcurrentExclusiveSchedulerPairTest
    {
        /// <summary>
        /// 使用 ConcurrentExclusiveSchedulerPair 实现 ReaderWriterLockSlim 的读写语义
        /// </summary>
        public void TestConcurrentExclusiveSchedulerPair()
        {
            var pair = new ConcurrentExclusiveSchedulerPair();
            var concurrentTaskFactory = new TaskFactory(pair.ConcurrentScheduler); // 并发任务工厂
            var exclusiveTaskFactory = new TaskFactory(pair.ExclusiveScheduler); // 独占任务工厂
            var queue = new ConcurrentQueue<DateTime>();

            // 独占写
            var tasks = Enumerable.Range(0, 5).Select(_ => exclusiveTaskFactory.StartNew(() =>
            {
                Task.Delay(1000).Wait();
                queue.Enqueue(DateTime.Now);
            })).ToList();

            Task.WhenAll(tasks).Wait();

            // 并发读
            for (int i = 0; i < 5; i++)
            {
                concurrentTaskFactory.StartNew(() =>
                {
                    if (queue.TryDequeue(out var value))
                    {
                        Console.WriteLine($"[{DateTime.Now}] {value}");
                    }
                });
            }
        }

        /// <summary>
        /// 使用 WaitAsync 实现同步构造的互斥语义
        /// SemaphoreSlim.WaitAsync() 就是一个异步的同步构造，可以把 SemaphoreSlim 的 maxCount 设置为 1，从而实现 monitor 的行为。
        /// WaitAsync 是异步的，这样就不会阻塞线程，比起直接调用 WaitOne 每次都阻塞一个线程，性能更好。
        /// </summary>
        public async void TestWailAsync()
        {
            var semaphore = new SemaphoreSlim(0, 1);

            Print();
            Print();
            Print();
            Release();
            async Task Release()
            {
                while (true)
                {
                    semaphore.Release();
                    await Task.Delay(1000);

                    Console.WriteLine($"[Release]  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
                }
            }

            async Task Print()
            {
                while (true)
                {
                    await semaphore.WaitAsync();

                    Console.WriteLine($"[Print]  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
                    await Task.Delay(2000);

                }

            }
        }
    }
}
