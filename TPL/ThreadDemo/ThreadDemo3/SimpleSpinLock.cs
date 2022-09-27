using System.Diagnostics;

namespace ThreadDemo3;

/// <summary>
/// 一个简单的自旋锁
/// 有以下注意点：
/// 1. 自旋锁在获取不到锁的时候，会进行空转。所以在自旋的时候，会占用 CPU，所以一般不在单 CPU 机器上用。
/// 2. 当占有锁的线程优先级比获取锁的线程更低的时候，会导致占有锁的线程一直获取不到CPU进行工作，从而无法释放锁，导致活锁。
///    所以使用自旋锁的线程，应该禁用线程优先级提升功能。
/// FCL 提供了 System.Threading.SpinWait 和 System.Thread.SpinLock(是用来 SpinWait 增强性能)
/// 
/// </summary>
public class SimpleSpinLock
{

    private int _count;
    public void Enter()
    {
        
        while (true)
        {
            if (Interlocked.Exchange(ref _count, 1) == 0)
            {
                return;
            }
        }
    }

    public void Leave()
    {
        Volatile.Write(ref _count, 0);
    }
}