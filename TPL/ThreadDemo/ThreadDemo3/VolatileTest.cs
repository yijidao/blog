namespace ThreadDemo3;

class VolatileTest
{
    public void Call()
    {
        var t = new Thread(Test);
        t.Start();

        while (!Volatile.Read(ref Flag))
        {

        }

        Console.WriteLine("Call() 执行结束");
        Console.ReadLine();

    }
    private bool Flag = false; // 需要添加 volatile

    void Test()
    {
        Thread.Sleep(500);
        Flag = true; // 在 Release 模式下会被优化掉，导致 Main() 无法执行到 Hello World
        Console.WriteLine("退出线程");
    }

    public void Test2()
    {
        var switchTrue = false;

        var t = new Thread(() =>
        {
            var x = 0;
            while (!Volatile.Read(ref switchTrue))
            {
                x++;
            }
            Console.WriteLine($"x: {x}");
        });
        t.IsBackground = true;
        t.Start();

        Thread.Sleep(100);
        switchTrue = true;
        Console.WriteLine("ok");
    }

    public void Test3()
    {
        int x = 0, y = 0;

        ThreadPool.QueueUserWorkItem(_ => DoWork());
        ThreadPool.QueueUserWorkItem(_ => DoWork2());

        void DoWork()
        {
            y = 10; // 这里可能会发生指令重排，也就是可能变成先执行 x = 1，执行 y = 10。如果这种指令重排真的发生了，那么 DoWork2() 就可能会打印 y = 0 了。
            x = 1;
        }

        void DoWork2()
        {
            if (x == 1)
            {
                Console.WriteLine(y);
            }
        }
    }

}