using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    internal class SingletonApp
    {
        public static void SingletonAppForSemaphore()
        {
            // 用其他内核构造也可以实现
            using var semaphore = new Semaphore(0, 1, "SingletonApp", out var createdNew);
            Console.WriteLine(createdNew ? $"创建内核对象{nameof(SingletonApp)}" : $"内核对象{nameof(SingletonApp)}已存在");
            Console.ReadLine();
        }
    }
}
