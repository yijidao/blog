namespace ThreadDemo3;

public class SimpleLockBaseSemaphore
{
    private Semaphore _semaphore;

    public SimpleLockBaseSemaphore(int concurrentCount)
    {
        _semaphore = new Semaphore(concurrentCount, concurrentCount);
    }

    public void Enter()
    {
        _semaphore.WaitOne();
    }

    public void Leave()
    {
        _semaphore.Release(1);
    }
}