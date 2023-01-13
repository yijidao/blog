using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.UsingThread
{
    /// <summary>
    /// Thread.Yield() 让出控制权给CPU上的其他线程。
    /// Thread.Sleep(0) 让出控制权给同等优先级的线程执行，如果没有，就继续执行本线程。
    /// Thread.Sleep(1) 让出控制权给正在等待的线程执行。
    /// Thread.SpinWait() 不让出控制权，在CPU上自旋一段时间。
    /// 
    /// </summary>
    internal class ThreadDemo
    {
        public void Test()
        {
           
        }
    }
}
