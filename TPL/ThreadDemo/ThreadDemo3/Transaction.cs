using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    internal class Transaction
    {
        private DateTime _timeLastTrans;

        public DateTime TimeLastTrans
        {
            get
            {
                Monitor.Enter(this);
                var t = _timeLastTrans;
                Monitor.Exit(this);
                return t;
            }
        }

        public void PerformTransaction()
        {
            Monitor.Enter(this);
            _timeLastTrans = DateTime.Now;
            Monitor.Exit(this);
        }
    }

    public class TransactionTest
    {
        public void Test()
        {
            var t = new Transaction();
            Monitor.Enter(t);
            ThreadPool.QueueUserWorkItem(_ => t.PerformTransaction());
            Monitor.Exit(t);
        }
    }
}
