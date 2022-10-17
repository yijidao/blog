using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.CPUBound
{
    internal class TaskExceptionTest
    {
        public void Test()
        {
            var parent = new Task<string[]>(() =>
            {
                var result = new string[2];
                new Task(() =>
                    {
                        Thread.Sleep(1000);
                        throw new Exception("子异常1");
                        result[0] = "children1 done";
                    }, TaskCreationOptions.AttachedToParent)
                    .Start();
                new Task(() =>
                    {
                        Thread.Sleep(500);
                        throw new Exception("子异常2");
                        result[1] = "children2 done";
                    }, TaskCreationOptions.AttachedToParent)
                    .Start();
                return result;

            });
            parent.Start();
            parent.ContinueWith(r => Array.ForEach(r.Result, Console.WriteLine));
            try
            {
                parent.Wait();
            }
            catch (AggregateException e)
            {
                try
                {
                    e.Handle(ie =>
                    {
                        var isHandle = ie.InnerException?.Message.Contains("1") ?? false; // 只处理异常1
                        if (isHandle)
                        {
                            Console.WriteLine($"{ie.InnerException?.Message} 已处理");
                        }
                        return isHandle;
                    });
                }
                catch (AggregateException exception) // 捕获到包含异常2的AggregateException
                {
                    throw exception;
                }
            }
        }


        public async void Test2()
        {
            var parent = new Task<string[]>(() =>
            {
                var result = new string[2];
                new Task(() =>
                    {
                        Thread.Sleep(1000);
                        throw new Exception("子异常1");
                        result[0] = "children1 done";
                    }, TaskCreationOptions.AttachedToParent)
                    .Start();
                new Task(() =>
                    {
                        Thread.Sleep(500);
                        throw new Exception("子异常2");
                        result[1] = "children2 done";
                    }, TaskCreationOptions.AttachedToParent)
                    .Start();
                return result;

            });
            parent.Start();
            try
            {
                await parent.ContinueWith(r => Array.ForEach(r.Result, Console.WriteLine));
            
            }
            catch (AggregateException e)
            {
                
                try
                {
                    e.Handle(ie =>
                    {
                        var isHandle = ie.InnerException?.Message.Contains("1") ?? false; // 只处理异常1
                        if (isHandle)
                        {
                            Console.WriteLine($"{ie.InnerException?.Message} 已处理");
                        }
                        return isHandle;
                    });
                }
                catch (AggregateException exception) // 捕获到包含异常2的AggregateException
                {
                    throw exception;
                }
            }
        }
    }
}
