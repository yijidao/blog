using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// 另一个混合锁
    /// 支持锁自旋、锁递归、锁线程所有权
    /// </summary>
    internal class SimpleHybridLock2
    {
        /// <summary>
        /// _waiters 用户构造使用， _spinCount 自旋次数， _recursion 拥有锁的线程递归次数， _owningThreadId 拥有锁的线程Id
        /// </summary>
        private int _waiters = 0, _spinCount = 4000, _recursion = 0, _owningThreadId = 0;
        private AutoResetEvent _waiterLock = new(false);

        public void Enter()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            if (_owningThreadId == threadId)
            {
                _recursion++;
                return;
            }

            var spinWait = new SpinWait();
            for (var i = 0; i < _spinCount; i++) // 自旋
            {
                if (Interlocked.CompareExchange(ref _waiters, 1, 0) == 0)
                {
                    GotLock();
                    return;
                }

                spinWait.SpinOnce();
            }


            if (Interlocked.Increment(ref _waiters) > 1)
            {
                _waiterLock.WaitOne();
                GotLock();
            }


            void GotLock()
            {
                _owningThreadId = threadId;
                _recursion = 1;
            }
        }

        public void Leave()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            if (threadId != _owningThreadId)
            {
                throw new Exception("拥有锁的线程才能释放锁");
            }

            if (--_recursion > 0)
            {
                return;
            }

            _owningThreadId = 0;

            if (Interlocked.Decrement(ref _waiters) == 0)
            {
                return;
            }

            _waiterLock.Set();
        }


    }
}
