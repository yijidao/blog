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

            //TestLock(o);
            //TestReaderWriterLock();
            TestConditionVariablePattern();
            Console.WriteLine("结束");
            Console.ReadLine();

            //TestMutex();
        }

        static void TestLock()
        {
            var o = new object();

            lock (o)
            {
                Console.WriteLine("异常前");
                throw new Exception("异常");
                Console.WriteLine("异常后");
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
        static void TestPerformance()
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
                M1();
                simpleSpinLock.Leave();
            }
            Console.WriteLine($"使用用户构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 96ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                simpleHybridLock.Enter();
                M1();
                simpleHybridLock.Leave();
            }
            Console.WriteLine($"使用混合构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 5235 ms

            sw.Restart();
            for (int i = 0; i < count; i++)
            {
                simpleHybridLock2.Enter();
                M1();
                simpleHybridLock2.Leave();
            }
            Console.WriteLine($"使用具备锁线程拥有权的混合构造的同步，耗时：{sw.ElapsedMilliseconds}"); // 耗时 5235 ms

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

        static void TestReaderWriterLock()
        {
            var t2 = new Transaction2();
            t2.Test();
        }

        static void TestConditionVariablePattern()
        {
            var condition = new ConditionVariablePattern();
            condition.Test();
        }
    }
}