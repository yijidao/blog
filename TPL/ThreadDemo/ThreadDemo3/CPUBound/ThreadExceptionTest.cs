using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.CPUBound
{
    internal class ThreadExceptionTest
    {
        public async void Test()
        {
            //ThreadPool.QueueUserWorkItem(_ => ThreadThrowException()); // 未捕获异常导致程序崩溃

            //var t = new Thread(_ => ThreadThrowException()); // 未捕获异常导致程序崩溃
            //t.IsBackground = true;
            //t.Start();

            //var _ = Task.Run(ThreadThrowException); // 未捕获异常也不会导致程序崩溃
            //string? r = null;
            //Exception? e = null;

            //var t2 = new Thread(_ => SafeExecute(ThreadReturnValue, out r, out e)); // 通过委托获取返回值
            //t2.Start();
            //t2.Join();
            //Console.WriteLine(r);

            //var t3 = new Thread(_ => SafeExecute(ThreadThrowException, out r, out e)); // 通过委托处理异常
            //t3.Start();
            //t3.Join();
            //Console.WriteLine(e);

            //Console.WriteLine(await SafeExecute(ThreadReturnValue)); // 通过委托获取返回值

            //try
            //{
            //    await SafeExecute(ThreadThrowException); // 通过委托处理异常
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception);
            //}
        }

        public string ThreadThrowException()
        {
            Thread.Sleep(1000);
            throw new Exception("线程异常");
        }

        public string ThreadReturnValue()
        {
            Thread.Sleep(1000);
            return "done";
        }

        /// <summary>
        /// 捕获异常，并通过 out 获取返回值
        /// </summary>
        public void SafeExecute<T>(Func<T> func, out T? r, out Exception? e)
        {
            try
            {
                e = null;
                r = func();
            }
            catch (Exception? exception)
            {
                r = default;
                e = exception;
            }
        }

        /// <summary>
        /// 捕获异常，并通过 TaskCompletionSource 获取返回值
        /// </summary>
        public Task<T> SafeExecute<T>(Func<T> func)
        {
            var t = new TaskCompletionSource<T>();
            try
            {
                t.TrySetResult(func());
            }
            catch (Exception e)
            {
                t.SetException(e);
            }

            return t.Task;
        }


    }
}
