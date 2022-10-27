using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.CPUBound
{
    internal class TaskYieldTest
    {
        public async Task Test()
        {
            await Task.Yield();

            Console.WriteLine($"TaskYieldTest: [{Thread.CurrentThread.ManagedThreadId}]");
        }
    }
}
