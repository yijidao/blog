using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.UsingThread
{
    internal class ThreadPoolDemo
    {
        public void Test()
        {
            var ti = new TaskInfo
            {
                Info = "其他信息"
            };
            var are = new AutoResetEvent(false);
            var handle = ThreadPool.RegisterWaitForSingleObject(are, DoWork, ti, 2000, false);
            ti.WaitHandle = handle;

            Thread.Sleep(3000);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 重新 signal AutoResetEvent");
            are.Set();
            Thread.Sleep(2000);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 第二次 signal AutoResetEvent");


            void DoWork(object? state, bool timedout)
            {
                if (timedout)
                {
                    Console.WriteLine("超时");
                }
                else
                {
                    var taskInfo = (TaskInfo)state;
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 执行完毕，取消 Callback");

                    taskInfo.WaitHandle.Unregister(null); // 取消回调，不然会回调会一直循环执行，而且应该用 Unregister 来取消，只在构造函数里面指定 executeOnlyOnce:true 的话，可能会无法 gc 回调。
                }
            }


        }

        class TaskInfo
        {
            public RegisteredWaitHandle WaitHandle { get; set; }

            public string Info { get; set; }
        }

    }
}
