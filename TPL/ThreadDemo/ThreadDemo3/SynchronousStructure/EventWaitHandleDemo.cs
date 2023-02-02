namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// EventWaitHandle 是一个内核构造，内部维护一个 bool 值，当值为 true 时，signal 调用了 WaitOne() 的线程，解除该线程的阻塞。
/// 构造函数最多接收三个参数
///
/// 注意点：
/// EventWaitHandle 跟 ManualResetEvent 和 AutoResetEvent 的不同是，EventWaitHandle 支持命名，所以它是一个系统级的同步构造，跟 Mutex 一样，可以跨线程跨域同步。
/// </summary>
public class EventWaitHandleDemo
{
    public void Test()
    {
        var ewh = new EventWaitHandle(false, EventResetMode.ManualReset, "");
    }

    /// <summary>
    /// 测试 EventWaitHandle 跟其他线程通信
    /// </summary>
    public void Test2()
    {
        EventWaitHandle ewh;
        if (EventWaitHandle.TryOpenExisting("multi-process", out ewh))
        {
            Console.WriteLine("等待 EventWaitHandle");
            ewh.WaitOne();
            Console.WriteLine("结束运行");
        }
        else
        {
            ewh = new EventWaitHandle(false, EventResetMode.AutoReset, "multi-process");
            while (true)
            {
                Console.WriteLine("按下 Enter 跟其他线程通讯");
                Console.ReadLine();
                ewh.Set();
            }
        }
    }
}