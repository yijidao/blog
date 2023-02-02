using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// 条件变量模式
    /// 如果希望线程在复合条件满足情况下执行，可以使用条件变量模式。
    /// 不使用自旋的原因是，自旋浪费 cpu 资源，而且 Interlock 往往只支持原子化条件，不支持复合条件。
    /// </summary>
    internal class ConditionVariablePattern
    {
        private readonly object _lock = new();
        private bool _condition = false;

        public void Thread1()
        {
            Monitor.Enter(_lock);
            while (!_condition)
            {
                Monitor.Wait(_lock);

                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]  {DateTime.Now}");
                Thread.Sleep(1000);

            }
            Monitor.Exit(_lock);
        }

        public void Thread2()
        {
            Monitor.Enter(_lock);
            _condition = true;
            Monitor.PulseAll(_lock);
            Monitor.Exit(_lock);
        }


        public void Test()
        {
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => Thread1());
            }

            Thread.Sleep(2000);
            Thread2();
        }
    }

    /// <summary>
    /// 使用条件变量模式实现的同步队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SynchronizedQueue<T>
    {
        private readonly object _lock = new();
        private Queue<T> _queue = new();


        public static void Test()
        {
            var t = new SynchronizedQueue<DateTime>();
            ThreadPool.QueueUserWorkItem(_ => DoWork());
            ThreadPool.QueueUserWorkItem(_ => DoWork2());

            void DoWork()
            {
                var r = new Random();
                while (true)
                {
                    var interval = 1000 * r.Next(1, 5);
                    Thread.Sleep(interval);
                    t.Enqueue(DateTime.Now);
                }
            }

            void DoWork2()
            {
                while (true)
                {
                    Console.WriteLine(t.Dequeue());
                }
            }
        }

        public void Enqueue(T value)
        {
            Monitor.Enter(_lock);
            _queue.Enqueue(value);
            Monitor.PulseAll(_lock);
            Monitor.Exit(_lock);
        }

        public T Dequeue()
        {
            Monitor.Enter(_lock);
            if (_queue.Count == 0)
            {
                Monitor.Wait(_lock);
            }

            var t = _queue.Dequeue();

            Monitor.Exit(_lock);
            return t;
        }
    }
}
