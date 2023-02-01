﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreadDemo3.CPUBound;
using ThreadDemo3.IOBound;
using ThreadDemo3.SynchronousStructure;
using ThreadDemo3.UsingThread;
using static System.Net.Mime.MediaTypeNames;

namespace ThreadDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            //{
            //    Console.WriteLine("==================== UnhandledException ==================");
            //    Console.WriteLine(eventArgs.ExceptionObject);
            //    Console.WriteLine("==================== UnhandledException ==================");
            //};

            //TestVolatile();

            //MultiWebRequests.Start(-1, "http://www.baidu.com", "http://www.sina.com", "http://www.taobao.com", "http://www.jd.com");
            //Console.ReadLine();

            //SingletonApp.SingletonAppForSemaphore();

            //TestPerformance();

            //TestLock(o);
            //TestReaderWriterLock();
            //TestConditionVariablePattern();
            //TestConcurrentCollection();
            //TestConcurrentExclusiveSchedulerPair();
            //TestAwaiter();
            //TestTaskLogger();
            //TestCancellationTokenSource();
            //TestTaskException();
            //TestParallel();
            //TestPLINQ();
            //TestThreadException();
            //TestSynchronizationContext();
            //TestTaskYield();
            //TestManualResetEvent();
            //TestSpinWait();
            //TestSpinLock();
            //TestMonitor();
            //TestMutex2();
            //TestCountDownEvent();
            //TestSemaphore(args.FirstOrDefault() ?? "");
            //TestBarrier();
            //TestThread();
            //TestThreadPool();
            //TestCancelThread();
            //Console.WriteLine(string.Join(',', args));

            //while (true)
            //{                                                                                                                                                                
            //    Console.WriteLine(DateTime.Now);
            //    Thread.Sleep(5000);
            //}

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
                simpleSpinLock.Exit();
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

        static void TestConcurrentCollection()
        {
            var t = new ConcurrentCollectionTest();
            //t.TestBlockingCollection();
            t.TestGetEnumerator();
        }

        static void TestConcurrentExclusiveSchedulerPair()
        {
            var t = new ConcurrentExclusiveSchedulerPairTest();
            //t.TestWailAsync();
            t.TestConcurrentExclusiveSchedulerPair();
        }

        static void TestAwaiter()
        {
            var t = new EventAwaiterTest();
            t.Test();
        }

        static async void TestTaskLogger()
        {
#if DEBUG
            TaskLogger.LogLevel = TaskLogger.TaskLogLevel.Pending;
#endif
            var list = new List<Task>
            {
                Task.Delay(2000).Log("2s op"),
                Task.Delay(5000).Log("5s op"),
                Task.Delay(6000).Log("6s op"),
            };
            try
            {
                await Task.WhenAll(list).WithCancellation(new CancellationTokenSource(3000).Token);
            }
            catch (OperationCanceledException e)
            {
            }

            foreach (var entry in TaskLogger.GeTaskLogEntries)
            {
                Console.WriteLine(entry);
            }

        }

        static void TestCancellationTokenSource()
        {
            var t = new CancelTest();
            t.Test();
        }

        static void TestTaskException()
        {
            //new TaskExceptionTest().Test();
            new TaskExceptionTest().Test2();

            //var parent = t.Test();
            //parent.ContinueWith(task => Array.ForEach(task.Result, Console.WriteLine));
            //parent.Wait();
            //Thread.Sleep(2000);
        }

        static void TestParallel()
        {
            var t = new ParallelTest();
            //t.Test();
            t.Test2();
        }

        static void TestPLINQ()
        {



            var t = new PLINQTest();
            t.Test2();
            t.Test3();
            //var sw = Stopwatch.StartNew();
            //t.ObsoleteMethod(typeof(Type).Assembly);
            //t.ObsoleteMethod(typeof(Type).Assembly);
            //t.ObsoleteMethod(typeof(Type).Assembly);
            //t.ObsoleteMethod(typeof(Type).Assembly);
            //t.ObsoleteMethod(typeof(Type).Assembly);
            //t.ObsoleteMethod(typeof(Type).Assembly);

            //sw.Stop();
            //Console.WriteLine($"耗时：{sw.ElapsedMilliseconds}");
        }

        static void TestThreadException()
        {
            var t = new ThreadExceptionTest();
            t.Test();
        }

        static void TestSynchronizationContext()
        {
            var sc = new SynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(sc);
            var t = new SynchronizationContextTest();
            t.Test();
        }

        static void TestTaskYield()
        {
            Console.WriteLine($"Start [{Thread.CurrentThread.ManagedThreadId}]");
            var t = new TaskYieldTest();
            t.Test();
            Console.WriteLine($"End [{Thread.CurrentThread.ManagedThreadId}]");

        }

        static void TestManualResetEvent()
        {
            var t = new ManualResetEventDemo();
            t.Test1();
        }

        static void TestSpinWait()
        {
            var t = new SpinWaitDemo();
            t.Test();
        }

        static void TestSpinLock()
        {
            var t = new SpinLockDemo();
            //t.Test();
            //t.Test2();
            t.Test3();
        }

        static void TestMonitor()
        {
            var t = new MonitorDemo();
            //t.Test();
            //t.Test2();
            t.Test3();
        }

        static void TestMutex2()
        {
            var t = new MutexDemo();
            //t.Test();
            t.Test2();
        }

        static void TestCountDownEvent()
        {
            var t = new CountdownEventDemo();
            //t.Test();
            t.Test2();
        }

        static void TestSemaphore(string input)
        {
            var t = new SemaphoreDemo();
            //t.Test();
            switch (input)
            {
                case "test2":
                    t.Test2();
                    break;
                case "test3":
                    t.Test3();
                    break;
            }
        }

        static void TestBarrier()
        {
            var t = new BarrierDemo();
            t.Test();
        }

        static void TestThread()
        {
            var t = new ThreadDemo();
            //t.Test();
            //t.Test2();
            t.Test3();
        }

        static void TestThreadPool()
        {
            var t = new ThreadPoolDemo();
            t.Test();
        }

        static void TestCancelThread()
        {
            var t = new CancelThreadDemo();
            //t.Test();
            //t.Test2();
            //t.Test3();
            t.Test4();
        }

        static void TestVolatile()
        {
            var t = new VolatileTest();
            //t.Call();
            //t.Test2();
            //t.Test3();
        }

    }
}