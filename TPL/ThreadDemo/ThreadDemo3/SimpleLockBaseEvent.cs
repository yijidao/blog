using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    internal class SimpleLockBaseEvent : IDisposable
    {
        private AutoResetEvent _autoResetEvent = new (true);

        public void Enter()
        {
            _autoResetEvent.WaitOne();
        }

        public void Leave()
        {
            _autoResetEvent.Set();
        }


        public void Dispose()
        {
            _autoResetEvent.Dispose();
        }
    }
}
