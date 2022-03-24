using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1.Views
{
    internal static class ServiceDemo
    {
        public static Task<string> GetAsync(string p)
        {
            if (p == "Created")
            {
                return new Task<string>(() => "OK"); // 直接 new Task 并返回状态是 Created，需要调用 Task.Start 才能启动异步任务，不能强制要求调用方调用 Start,所以不应该直接使用task 的构造器来生成 task 对象
            }


            return Task.Run(async () =>
            {
                await Task.Delay(200);
                return "OK";
            });
        }

        public static Task<string> GetAsync(string p, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(5000, token);
                return "OK";
            }, token);
        }

        public static Task<string> GetAsyncWithException(string p)
        {
            if (p == "UsageError")
            {
                throw new Exception("使用方法错误");
            }

            var t = Task.Run(async () =>
            {
                await Task.Delay(500);
                if (p == "RunError")
                {
                    await Task.FromException<string>(new Exception("运行出错"));
                }
                return "OK";
            });
            return t;
        }

    }
}
