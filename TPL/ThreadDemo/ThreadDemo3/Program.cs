using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace ThreadDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var volatileTest = new VolatileTest();
            //volatileTest.Call();

            //MultiWebRequests.Start(-1, "http://www.baidu.com", "http://www.sina.com", "http://www.taobao.com", "http://www.jd.com");
            //Console.ReadLine();

            //SingletonApp.SingletonAppForSemaphore();

            //TestPerformance();

            //TestMutex();
        }

        // 测试用户模式构造和内核模式构造，在锁没有发生竞争的情况下的性能差距
        static void TestPerformance()
        {
            var count = 10000 * 1000;// 一千万
            var simpleSpinLock = new SimpleSpinLock();
            var simpleEventLock = new SimpleLockBaseEvent();


            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                simpleSpinLock.Enter();
                M1();
                simpleSpinLock.Leave();
            }
            Console.WriteLine($"使用用户构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 96ms


            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                M1();
            }
            Console.WriteLine($"直接调用一个没有实现的方法，耗时：{sw.ElapsedMilliseconds}"); // 耗时 31ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                simpleEventLock.Enter();
                M1();
                simpleEventLock.Leave();
            }
            Console.WriteLine($"使用内核构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 5235 ms
            Console.ReadLine();
            void M1()
            {
            }
        }

        static void TestMutex()
        {
            var mutex = new Mutex(true);
            M1();
            void M1()
            {
                mutex.WaitOne();
                M2();
                mutex.ReleaseMutex();
            }

            void M2()
            {
                mutex.WaitOne();
                var t = new Thread(() => mutex.ReleaseMutex()); // 会抛出异常，因为 mutex 必须要同一线程上进行锁定和解锁，否则会抛出 ApplicationException
                t.Start();
                mutex.ReleaseMutex();
            }
        }

    }
}