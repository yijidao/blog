using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.CPUBound
{
    internal class SynchronizationContextTest
    {
        public void Test()
        {
            var sc = SynchronizationContext.Current;

            
            
            Console.WriteLine($"[{DateTime.Now}] Start {Thread.CurrentThread.ManagedThreadId}");
            //var c = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"[{DateTime.Now}] ThreadPool Start {Thread.CurrentThread.ManagedThreadId}");
                //sc?.Post(_ =>
                //{
                //    Console.WriteLine($"[{DateTime.Now}] Post Start {Thread.CurrentThread.ManagedThreadId}");

                //    Thread.Sleep(3000);
                //    Console.WriteLine($"[{DateTime.Now}] Post End {Thread.CurrentThread.ManagedThreadId}");
                //}, null);

                sc?.Send(_ =>
                {
                    Console.WriteLine($"[{DateTime.Now}] Send Start {Thread.CurrentThread.ManagedThreadId}");

                    Thread.Sleep(3000);
                    Console.WriteLine($"[{DateTime.Now}] Send End {Thread.CurrentThread.ManagedThreadId}");
                }, null);
                Console.WriteLine($"[{DateTime.Now}] ThreadPool End {Thread.CurrentThread.ManagedThreadId}");

            });

            Console.WriteLine($"[{DateTime.Now}] End {Thread.CurrentThread.ManagedThreadId}");

        }

    }
}
