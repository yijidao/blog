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
            //ThreadPool.QueueUserWorkItem(_ => ThreadThrowException());
            //var t = new Thread(_ => ThreadThrowException());
            //    t.IsBackground = true;
            //    t.Start();
            string? r = null;
            Exception? e = null;
            var t = new Thread(_ => SafeExecute(ThreadReturnValue, out r, out e));
            t.Start();
            t.Join();
            Console.WriteLine(r);

            var t2 = new Thread(_ => SafeExecute(ThreadThrowException, out r, out e));
            t2.Start();
            t2.Join();
            Console.WriteLine(e);

            Console.WriteLine(await SafeExecute(ThreadReturnValue));

            try
            {
                await SafeExecute(ThreadThrowException);
                var t5= Task.Run(() => { });
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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
