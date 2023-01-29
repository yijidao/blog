using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// ReaderWriterLockerSlim 用法
    /// </summary>
    internal class Transaction2
    {
        private DateTime _timeLastTrans;

        public DateTime TimeLastTrans
        {
            get
            {
                _lock.EnterReadLock();
                Thread.Sleep(1000);
                var t = _timeLastTrans;
                Console.WriteLine($"调用 ReadLock {Thread.CurrentThread.ManagedThreadId}");

                _lock.ExitReadLock();
                return t;
            }
        }

        private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

        public void PerformTransaction()
        {
            _lock.EnterWriteLock();
            _timeLastTrans = DateTime.Now;
            Console.WriteLine($"调用 WriteLock {Thread.CurrentThread.ManagedThreadId}");
            _lock.ExitWriteLock();
        }

        public void Test()
        {
            PerformTransaction();

            ThreadPool.QueueUserWorkItem(_ => Console.WriteLine(TimeLastTrans));

            PerformTransaction();
            Thread.Sleep(500); // 就算睡眠500ms，在锁释放后，依旧先进行读操作，读完才有写操作。
            ThreadPool.QueueUserWorkItem(_ => Console.WriteLine(TimeLastTrans)); 
        }
    }
}
