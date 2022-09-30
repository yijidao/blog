using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// 一个简单的混合锁
    /// </summary>
    internal class SimpleHybridLock : IDisposable
    {
        private int _waiter;
        private AutoResetEvent _waiterLock = new(false);


        public void Enter()
        {
            if (Interlocked.Increment(ref _waiter) == 1)
            {
                return;
            }

            _waiterLock.WaitOne();
        }

        public void Leave()
        {
            if (Interlocked.Decrement(ref _waiter) == 0)
            {
                return;
            }

            _waiterLock.Set();
        }

        public void Dispose()
        {
            _waiterLock.Dispose();
        }
    }
}
