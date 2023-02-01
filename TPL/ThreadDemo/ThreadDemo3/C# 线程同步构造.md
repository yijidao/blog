## 内核模式构造、混合构造、内存屏障
## 自旋、线程所有权、递归

# 同步构造
当线程 A 在等待一个同步构造，另一个线程 B 持有构造一直不释放，那么就会导致线程 A 阻塞。同步构造有用户模式构造和内核模式构造。
- 用户模式构造通过 CPU 指令来协调线程，所以速度很快。也意味着不受操作系统控制，所以等待构造的线程会不停自旋，浪费 CPU 时间。
- 内核模式构造通过操作系统来协调线程。内核构造在获取时，需要先转换成本机代码，在转换成内核代码，返回时则需要反方向再转换一次，所以速度会比用户构造慢很多。
  因为使用了操作系统来协调线程，所以也有了更加强大的功能。
  1. 不同线程在竞争一个资源时，操作系统会阻塞线程，所以不会自旋。
  2. 可以实现托管线程和本机线程的同步。
  3. 可以跨进程跨 domain 同步。
  4. 可以利用 WaitHandle 类的方法实现多个构造的同步或者超时机制。
> 活锁和死锁：  
> 当线程获取不到资源，从而不停在 CPU 上自旋等待资源，就会形成活锁。这是通过用户构造实现的。  
> 当线程获取不到资源，被操作系统阻塞，就会形成死锁。这是通过内核构造实现的。

## 用户模式构造
.Net 提供了两种用户构造，易变构造 Volatile、互锁构造 Interlocked，这两种构造都提供了原子性读写的功能。  
.Net 提供了基于易变构造、互锁构造、SpinWait 实现的自旋锁 SpinLock。
> 原子性读写：  
> 在 32 位 CPU 中，CPU 一次只能存储 32 位的数据，所以如果是 64 位的数据类型（如 double），就得执行两次 MOV 指令，所以在 32 位 CPU 和 32 位操作系统中，不同线程对 64 位的数据类型进行读写可能得到不同的结果。原子性读写就是保证了即使是 64 位的数据类型，不同线程读写也会得到相同的结果。现在的 CPU 和操作系统基本都是 64 位的，所以一般也不会遇到这种问题。

### 易变构造 Volatile 和 volatile 关键字
Volatile 一般用于阻止编译器代码优化，编译器优化代码会优化掉一些在单线程情况下无用的变量或者语句，在多线程代码下有时候会导致程序运行结果跟设计的不一样。  
Volatile.Read() 强制对变量的取值必须在调用时读取，Volatile.Write() 强制对变量的赋值必须在调用时写入。
``` c#
/// <summary>
/// 在 debug 模式下不开启代码优化，所以需要用 release 模式下生成。
/// 执行 dotnet build -c release --no-incremental 后运行代码，如果没有标记为易变，则不会打印 x。
/// </summary>
public void Test2()
{
    var switchTrue = false;

    var t = new Thread(() =>
    {
        var x = 0;
        while (!switchTrue) // 如果没有标记变量为易变，编译器会把 while(!switchTrue) 优化为 while(true) 从而导致永远不会打印出 x 的值
        //while (!Volatile.Read(ref switchTrue)) // 标记为易变，可以保证在调用时才进行取值，不会进行代码优化。
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
```

### 互锁构造 Interlocked
1. Interlocked 除了保证原子性读写外，还提供了很多方便的方法，在调用的地方建立了内存屏障，所以可以用来实现各种锁。
``` c#
/// <summary>
/// 用 Interlocked 实现一个简单的自旋锁
/// 注意：
/// 1. 自旋锁在获取不到锁的时候，会进行空转。所以在自旋的时候，会占用 CPU，所以一般不在单 CPU 机器上用。
/// 2. 当占有锁的线程优先级比获取锁的线程更低的时候，会导致占有锁的线程一直获取不到CPU进行工作，从而无法释放锁，导致活锁。
///    所以使用自旋锁的线程，应该禁用线程优先级提升功能。
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

    public void Exit()
    {
        Volatile.Write(ref _count, 0);
    }
}
```
2. Interlocked 也经常用来实现单例模式。实现单例模式经常用 lock 关键字和双检索模式的，但我都是用 Interlocked 或者 Lazy，因为更轻量代码也简单。
``` c#
/// <summary>
/// 使用 Interlocked 实现的单例，轻量且简单。
/// 可能会同时调用多次构造函数，所以适合构造函数没有副作用的类
/// </summary>
internal class DoubleCheckLocking3
{
    private static DoubleCheckLocking3? _value;

    private DoubleCheckLocking3()
    {

    }

    private DoubleCheckLocking3 GetInstance()
    {
        if (_value != null) return _value;
        Interlocked.CompareExchange(ref _value, new DoubleCheckLocking3(), null);
        return _value;
    }
}

/// <summary>
/// 使用 lock 和双检索实现的单例化
/// </summary>
internal class DoubleCheckLocking
{
    private static DoubleCheckLocking? _value;

    private static readonly object _lock = new();

    private DoubleCheckLocking()
    {

    }

    public static DoubleCheckLocking GetInstance()
    {
        if (_value != null) return _value;
        lock (_lock)
        {
            if (_value == null)
            {
                var t = new DoubleCheckLocking();
                Volatile.Write(ref _value, t); 
            }
        }
        return _value;
    }
}
```

### 自旋锁 SpinLock
.Net 提供了一个轻量化的同步构造 SpinLock，很适合在不常发生竞争的场景使用。如果发生竞争了，会先在 CPU 上自旋一段时间，如果还不能获取到资源，就会让出 CPU 控制权给其他线程（使用 SpinWait 实现的）。  
1. SpinLock 不支持重入锁，当给构造函数 SpinLock(bool) 传入 true 时，重入锁会抛出异常，否则就会死锁。
> 重入锁（Re-Enter）: 就是一个线程调用了 SpinLock.Enter() 后，没有调用 SpinLock.Exit()，再次调用了 SpinLock.Enter()。
``` c#
/// <summary>
/// 测试 SpinLock 重入锁
/// </summary>
public void Test3()
{
    var spinLock = new SpinLock(true); // 如果传 true，如果 SpinLock 重入锁，就会抛出异常，传 false 则不会，只会死锁。

    ThreadPool.QueueUserWorkItem(_ => DoWork());

    void DoWork()
    {
        var lockTaken = false;

        for (int i = 0; i < 10; i++)
        {
            try
            {
                Thread.Sleep(100);
                if (!spinLock.IsHeldByCurrentThread)  // SpinLock.IsHeldByCurrentThread 可以判断是不是当前线程拥有锁，如果是就不再获取锁
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 获取锁，i 为 {i}");
                    spinLock.Enter(ref lockTaken);
                }
                //spinLock.Enter(ref lockTaken); // 重入锁会死锁

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        if (lockTaken) // 使用 lockTaken 来判断锁是否已经被持有
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 释放锁");
            spinLock.Exit();
        }
        Console.WriteLine("结束");
    }
}
```

2. SpinLock 是 Struct 类型的，所以注意装箱拆箱。
``` c#
/// <summary>
/// 测试装箱拆箱问题
/// </summary>
public void Test4()
{
    var spinLock = new SpinLock(false);
    Task.Run(() => DoWork(ref spinLock));
    Task.Run(() => DoWork(ref spinLock));

    // SpinLock 是 Struct 类型，要注意装箱拆箱的问题，试试看不加 ref 关键字的效果
    void DoWork(ref SpinLock spinLock)
    {
        var lockTaken = false;
        Thread.Sleep(500);
        spinLock.Enter(ref lockTaken);
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 获取锁");
    }
}
```
## 内核模式构造
