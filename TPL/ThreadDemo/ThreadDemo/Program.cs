using System;
using System.Diagnostics;

namespace ThreadDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //var test = new Test();
            //test.Start(20);

            var test2 = new Test2();
            test2.Start();

            Console.ReadKey();
        }

    }

    class Test
    {
        private int ThreadCount { get; set; }

        private Stopwatch Stopwatch { get; set; }


        public void Start(int threadCount)
        {
            ThreadCount = threadCount;
            Stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < threadCount; i++)
            {
                var t = new Thread(TimeConsuming);
                t.Start();
            }
        }

        private void TimeConsuming()
        {
            Prime(5000000);

            lock (this)
            {
                ThreadCount--;
                if (ThreadCount == 0)
                {
                    Stopwatch.Stop();
                    Console.WriteLine(Stopwatch.Elapsed);
                }
            }
        }

        private void Prime(long size)
        {
            for (long i = 3; i < size; i++)
            {
                for (long j = 2; j < Math.Sqrt(i); j++)
                {
                    if(i % j == 0) break;
                }
            }
        }
    }

    class Test2
    {
        private AutoResetEvent _blockThread1 = new(false);
        private AutoResetEvent _blockThread2 = new(true);
        private string _threadOutput;

        public void Start()
        {
            var t = new Thread(DisplayThread1);
            var t2 = new Thread(DisplayThread2);
            t.Start();
            t2.Start();
        }


        void DisplayThread1()
        {
            while (true)
            {
                _blockThread1.WaitOne();
                Console.WriteLine("Display Thread 1");
                _threadOutput = "Thread 1";
                Thread.Sleep(1000);
                Console.WriteLine($"Thread 1 Output --> {_threadOutput}");
                _blockThread2.Set();
            }
        }

        void DisplayThread2()
        {
            while (true)
            {
                _blockThread2.WaitOne();
                Console.WriteLine("Display Thread2");
                _threadOutput = "Thread 2";
                Thread.Sleep(1000);
                Console.WriteLine($"Thread 2 Output --> {_threadOutput}");
                _blockThread1.Set();
            }
        }
    }
}
