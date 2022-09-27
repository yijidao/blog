namespace ThreadDemo3;

/// <summary>
/// 一个使用 AutoResetEvent 实现的递归锁，因为跟踪线程和递归都是托管代码，只有在第一次获取和最终放弃的时候使用 AutoResetEvent,所以性能更好
/// </summary>
public class RecursionLockBaseEvent : IDisposable
{
    private AutoResetEvent _lock = new(true);

    private int _owningThreadId, _recursionCount;

    public void Enter()
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;

        if (threadId == _owningThreadId)
        {
            _recursionCount++;
            return;
        }
            
        _lock.WaitOne();
        _owningThreadId = threadId;
        _recursionCount = 1;
    }

    public void Leave()
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        if (threadId != _owningThreadId)
        {
            throw new InvalidOperationException("必须是拥有锁的线程才能释放锁");
        }

        if (--_recursionCount == 0)
        {
            _owningThreadId = 0;
            _lock.Set();
        }

    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}