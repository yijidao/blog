using System.Reflection.PortableExecutable;

namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// `ReaderWriterLockSlim` 构造的逻辑如下：
///- 一个线程向数据写入时，请求访问的其他所有线程都阻塞。
///- 一个线程向数据读取时，请求读取的其他线程允许继续执行，但是请求写入的线程仍被阻塞。
///- 一个向数据写入的线程结束后，要么解除一个写入线程(writer)的阻塞，使它能向数据写入，要么解除所有读取线程(reader)的阻塞，使它们能够进行并发读取。如果没有线程被阻塞，则锁进入自由状态，可以被下一个 reader 或者 writer 线程获取。
///- 所有向数据读取的线程结束后，一个 writer 线程被解除阻塞，使它能向数据写入。如果没有线程被阻塞，则锁进入自由状态，可以被下一个reader 或者 writer 线程获取。
///
/// ReaderWriterLockSlim 可以使用 ReaderWriterLockSlim.EnterUpgradeableReadLock() 将并发执行的读锁升级为互斥执行的写锁，从而实现先查再更新的功能。
/// </summary>
public class ReaderWriterLockSlimDemo
{
    
}

/// <summary>
/// 
/// </summary>
class SynchronizedCache
{
    private ReaderWriterLockSlim _lock = new();

    private Dictionary<int, string> _cache = new();

    /// <summary>
    /// EnterReadLock 获取读锁，可以并发读取。
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Read(int key)
    {
        try
        {
            _lock.EnterReadLock();

            return _cache[key];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// EnterWriteLock 获取写锁，互斥写入。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(int key, string value) 
    {
        try
        {
            _lock.EnterWriteLock();
            _cache.Add(key,value);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// 测试 ReaderWriterLockSlim 超市功能
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool Add(int key, string value, int timeout)
    {


        if (_lock.TryEnterWriteLock(timeout))
        {
            try
            {
                _cache.Add(key, value); 
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 使用 ReaderWriterLockSlim.EnterUpgradeableReadLock() 实现先读再写
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public AddOrUpdateStatus AddOrUpdate(int key, string value)
    {
        try
        {
            _lock.EnterUpgradeableReadLock();
            if (_cache.TryGetValue(key, out var oldValue))
            {
                if (oldValue == value)
                {
                    return AddOrUpdateStatus.Unchanged;
                }
                else
                {
                    try
                    {
                        _lock.EnterWriteLock();
                        _cache[key] = value;
                        return AddOrUpdateStatus.Updated;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
            else
            {
                try
                {
                    _lock.EnterWriteLock();
                    _cache.Add(key, value);
                    return AddOrUpdateStatus.Added;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        finally
        {
            _lock.ExitUpgradeableReadLock();
        }
    }

    public void Delete(int key)
    {
        try
        {
            _lock.EnterWriteLock();
            _cache.Remove(key);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public enum AddOrUpdateStatus
    {
        Added,
        Updated,
        Unchanged
    };

}