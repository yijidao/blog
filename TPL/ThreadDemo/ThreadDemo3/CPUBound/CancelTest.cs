using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace ThreadDemo3.CPUBound
{
    /// <summary>
    /// 协作式超时和取消
    /// </summary>
    internal class CancelTest
    {
        public void Test()
        {
            var cts = new CancellationTokenSource(5000);
            cts.Token.Register(() => Console.WriteLine("Cancel 1"));
            cts.Token.Register(() => Console.WriteLine("Cancel 2"));
            
            //SpinWait

            var cts2 = new CancellationTokenSource();
            var cts3 = new CancellationTokenSource(2000);
            var cts4 = CancellationTokenSource.CreateLinkedTokenSource(cts2.Token, cts3.Token);

            cts2.Token.Register(() => Console.WriteLine("LineToken1 cancel"));
            cts3.Token.Register(() => Console.WriteLine("LineToken2 cancel"));
            cts4.Token.Register(() => Console.WriteLine("LinedToken cancel"));

            

        }

    }
}
