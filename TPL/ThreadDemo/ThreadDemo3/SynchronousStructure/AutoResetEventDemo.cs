using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.SynchronousStructure
{
    internal class AutoResetEventDemo
    {
        public void Test()
        {
            var are = new AutoResetEvent(false);

            Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    Console.WriteLine("按下 Enter 释放一个线程");
                    Console.ReadLine();
                    are.Set();
                }
            });

            foreach (var i in Enumerable.Range(1,5))
            {
                var t = new Thread(DoWork);
                t.Name = $"线程 {i}";
                t.Start();
            }

            void DoWork()
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} 开始");
                are.WaitOne();
                Console.WriteLine($"{Thread.CurrentThread.Name} 结束");
            }
        }
    }
}
