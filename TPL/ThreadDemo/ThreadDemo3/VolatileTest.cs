namespace ThreadDemo3;

class VolatileTest
{
    public void Call()
    {
        var t = new Thread(Test);
        t.Start();

        while (!Flag)
        {

        }

        Console.WriteLine("Hello, World!");
        Console.ReadLine();

    }
    private  bool Flag = false; // 需要添加 volatile

    void Test()
    {
        Thread.Sleep(500);
        Flag = true; // 在 Leave 模式下会被优化掉，导致 Main() 无法执行到 Hello World
        Console.WriteLine("退出线程");
    }
}