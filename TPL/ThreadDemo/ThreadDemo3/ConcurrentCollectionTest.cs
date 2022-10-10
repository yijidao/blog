using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    internal class ConcurrentCollectionTest
    {
        /// <summary>
        /// GetEnumerator() 会返回并发集合的一个快照，而集合可能随时改变。
        /// </summary>
        public void TestGetEnumerator()
        {
            var queue = new ConcurrentQueue<DateTime>();
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Thread.Sleep(3000);

                var e = queue.GetEnumerator();
                while (e.MoveNext())
                {
                    Console.WriteLine($"Current  {e.Current}"); // 打印三次
                } 
            });


            for (int i = 0; i < 5; i++)
            {
                queue.Enqueue(DateTime.Now); // 添加五个子项
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 并发集合都实现了 IProducerConsumerCollection<T>，这个接口主要用来保证集合线程安全，并实现生产者消费者模式，主要用于 BlockingCollection<T>.
        /// BlockingCollection<T> 会阻塞消费者线程，直到 CompleteAdding() 被调用，代表集合已经不会再生成任何内容，消费者线程才会被释放。
        /// </summary>
        public void TestBlockingCollection()
        {
            var queue = new ConcurrentQueue<DateTime>();
            var blockCollection = new BlockingCollection<DateTime>(queue);

            ThreadPool.QueueUserWorkItem(Consumer, blockCollection);

            Thread.Sleep(1000);

            for (int i = 0; i < 5; i++)
            {
                blockCollection.Add(DateTime.Now);
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);

            blockCollection.CompleteAdding();


            void Consumer(object o)
            {
                var bc = (BlockingCollection<DateTime>)o;
                Console.WriteLine($"[Consumer] start {DateTime.Now}");
                foreach (var dateTime in bc.GetConsumingEnumerable())
                {
                    Console.WriteLine($"[Consumer] {dateTime}");
                }

                Console.WriteLine($"[Consumer] end {DateTime.Now}");
            }
        }
    }
}
