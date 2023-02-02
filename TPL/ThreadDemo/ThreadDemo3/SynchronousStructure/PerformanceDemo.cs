using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.SynchronousStructure
{
    internal class PerformanceDemo
    {
        /// <summary>
        /// 测试用户模式构造和内核模式构造，在锁没有发生竞争的情况下的性能差距
        /// </summary>
        public void Test()
        {
            var count = 1000 * 10000;
            var spinLock = new SpinLock(false);
            var are = new AutoResetEvent(true);
            var pool = new Semaphore(1, 1);

            var sw = Stopwatch.StartNew();
            foreach (var _ in Enumerable.Range(0, count))
            {
                var lockTaken = false;
                spinLock.Enter(ref lockTaken);
                DoWork();
                spinLock.Exit(lockTaken);
            }
            Console.WriteLine($"在没有竞争的场景下，执行一个空方法一千万次，SpinLock 耗时：{sw.ElapsedMilliseconds} ms");

            sw.Restart();
            foreach (var _ in Enumerable.Range(0, count))
            {
                are.WaitOne();
                DoWork();
                are.Set();
            }
            Console.WriteLine($"在没有竞争的场景下，执行一个空方法一千万次，AutoResetEvent 耗时：{sw.ElapsedMilliseconds} ms");

            sw.Restart();
            foreach (var _ in Enumerable.Range(0, count))
            {
                pool.WaitOne();
                DoWork();
                pool.Release();
            }
            Console.WriteLine($"在没有竞争的场景下，执行一个空方法一千万次，Semaphore 耗时：{sw.ElapsedMilliseconds} ms");

            // 空方法
            void DoWork()
            {

            }
        }


        /// <summary>
        /// 测试用户模式构造和内核模式构造，在锁没有发生竞争的情况下的性能差距
        /// 使用用户构造的同步，耗时：153
        /// 使用混合构造的同步，耗时：150
        /// 使用具备锁线程拥有权的混合构造的同步，耗时：439
        /// 直接调用一个没有实现的方法，耗时：37
        /// 使用内核构造的同步，耗时：5826
        /// </summary>
        public void TestPerformance()
        {
            var count = 10000 * 1000;// 一千万
            var simpleSpinLock = new SimpleSpinLock();
            var simpleEventLock = new SimpleLockBaseEvent();
            var simpleHybridLock = new SimpleHybridLock();
            var simpleHybridLock2 = new SimpleHybridLock2();


            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                simpleSpinLock.Enter();
                DoWork();
                simpleSpinLock.Exit();
            }
            Console.WriteLine($"使用用户构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 96ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                simpleHybridLock.Enter();
                DoWork();
                simpleHybridLock.Exit();
            }
            Console.WriteLine($"使用混合构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 5235 ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                simpleHybridLock2.Enter();
                DoWork();
                simpleHybridLock2.Leave();
            }
            Console.WriteLine($"使用具备锁线程拥有权的混合构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 5235 ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                DoWork();
            }
            Console.WriteLine($"直接调用一个没有实现的方法，耗时：{sw.ElapsedMilliseconds}"); // 耗时 31ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                simpleEventLock.Enter();
                DoWork();
                simpleEventLock.Leave();
            }
            Console.WriteLine($"使用内核构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 5235 ms
            Console.ReadLine();
            void DoWork()
            {
            }
        }
    }
}
