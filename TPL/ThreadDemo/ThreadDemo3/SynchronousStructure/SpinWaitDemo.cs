using System.Diagnostics;

namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// 代码来自
/// https://learn.microsoft.com/en-us/dotnet/standard/threading/how-to-use-spinwait-to-implement-a-two-phase-wait-operation
/// 看了很久，没看懂，这个到底是为了干啥。
/// 不过这个样例程序是为了表达，SpinWait 一般都用来跟其他内核构造锁组合，形成混合构造，从而提高性能。
/// - 注意点
///   - SpinWait 一般会跟内核构造一起组合，形成混合构造，从而提高性能。在没有竞争的时候使用用户构造，有竞争的时候使用内核构造。
///   - SpinWait.SpinOnce() 代表自旋一次，自旋多次之后会把线程时间片让渡出去，给其他线程使用，免得浪费 CPU 一直在空跑。
///   - SpinWait.NextSpinWillYield 属性为 true 时，代表下次调用 SpinOnce 会不再占用 cpu，并且切换线程上下文，这样可以避免不停空转，从而使CPU没有机会去调度其他线程。
///   - 如果 SpinWait.NextSpinWillYield 返回 true 时，就可以切换成内核构造来进行同步操作了。
/// 
/// </summary>
public class SpinWaitDemo
{

    public void Test()
    {
        var latch = new Latch();
        var count = 2;
        var cts = new CancellationTokenSource();

        latch.Set();

        Task.Factory.StartNew(() =>
        {
            Console.WriteLine("按下 c 取消");
            if (Console.ReadKey(true).KeyChar == 'c')
            {
                cts.Cancel();
            }
        });

        Parallel.Invoke(Start, Start, Start);

        latch.DisplayLog(); 
        if (cts != null) cts.Dispose();

        void Start()
        {
            while (!cts.IsCancellationRequested)
            {
                if (latch.Wait(50))
                {
                    double d = 0;
                    if (count % 2 == 0)
                    {
                        d = Math.Sqrt(count);
                    }

                    Interlocked.Increment(ref count);

                    latch.Set();
                }
            }
        }
    }

    class Latch
    {
        private object _latchLock = new();

        /// <summary>
        /// 0:reset() 1:set()
        /// </summary>
        private int _state = 0;

        private volatile int totalKernelWaits = 0;

        private ManualResetEvent _mre = new(false);

        private long[] _spinCountLog = new long[20];

        public void DisplayLog()
        {
            for (int i = 0; i < _spinCountLog.Length; i++)
            {
                Console.WriteLine($"{_spinCountLog[i]:N0} 次在在自旋 {i} 次之后调用 Wait() 成功");
            }
            Console.WriteLine($"使用 {totalKernelWaits} 次内核调用 Wait() 成功");
            Console.WriteLine("日志结束");
        }


        public void Set()
        {
            lock (_latchLock)
            {
                _state = 1;
                _mre.Set();
            }
        }

        public bool Wait(int timeout = Timeout.Infinite)
        {
            var spinner = new SpinWait();
            Stopwatch watch;

            while (_state == 0)
            {
                watch = Stopwatch.StartNew();
                if (!spinner.NextSpinWillYield) // 一般都会用这个属性进行判断，如果自旋锁已经切换上下文了，那就可以直接切换成内核构造了。
                {
                    spinner.SpinOnce();
                }
                else
                {
                    Interlocked.Increment(ref totalKernelWaits);
                    var realTimeout = timeout - watch.ElapsedMilliseconds;

                    if (realTimeout <= 0 || !_mre.WaitOne((int)realTimeout))
                    {
                        Trace.WriteLine("wait() 超时");
                        return false;
                    }
                }
            }

            Interlocked.Increment(ref _spinCountLog[spinner.Count]);

            Interlocked.Exchange(ref _state, 0);
            return true;
        }


    }

}