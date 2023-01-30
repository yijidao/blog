## 进程和线程
### 不同程序执行需要进行调度和独立的内存空间
在单核计算机中，CPU 是独占的，内存是共享的，这时候运行一个程序的时候是没有问题。但是运行多个程序的时候，为了不发生一个程序霸占整个 CPU 不释放的情况（如一个程序死循环无法结束了，那么其他程序就没有机会运行了），就需要开发者给不同程序划分不同的执行时间。为了避免不同程序之间互相操作数据或代码，导致程序被破坏的情况，就需要开发者给程序划分独立的内存范围。也就是程序需要开发者进行调度以及和划分独立的内存空间。  
### 进程是应用程序的一个实例
为了避免每个开发者来进行这个工作，所以有了操作系统。操作系统负责整个计算机的程序调度，让每个程序都有机会使用CPU，同时使用来进程来为程序维护一个独立虚拟空间，确保程序间的运行不会互相干扰。所以进程就是程序的一个实例，拥有程序需要使用的资源集合，确保自己的资源不会被其他进程破坏。

### 线程是操作系统进行调度的最小单位
这时候一个进程一次只能处理一个任务，如果需要一边不停输出 hellowork，一边计时，那么需要启动两个进程。如果需要对一个队列同时入队出队，那么不仅需要两个进程，还需要两个进程可以访问相同的内存空间。所以为了进程可以并发地处理任务，同时共享相同的资源，就需要给进程一个更小的调度单位，也就是线程，因此，线程也叫轻量化进程。所以在现代计算机中，操作继续不会直接调度进程在 CPU 上执行，而是调度线程在 CPU 上执行，所以说，线程是操作系统进行调度的最小单位。

## 线程操作
### 取消线程、插槽和 ThreadStatic
### 新建线程、启动线程、线程优先级
``` c#
public void Test()
{
    var t = new Thread(() => { }); // 使用无参委托
    var t2 = new Thread(state => { }); // 使用 object? 参数委托
    var t3 = new Thread(DoWork);
    var t4 = new Thread(DoWork2);
    t.Priority = ThreadPriority.Highest; // 设置线程的优先级，默认是 ThreadPriority.Normal
    t.Start(); // 不传入参数，启动线程
    t2.Start("参数"); // 传入参数，启动线程

    void DoWork() {}
    void DoWork2(object? state) {}
}
```
### 阻塞线程
1. 当线程调用 Sleep() 或者等待锁时，进入阻塞状态。
``` c#
public void Test()
{
    var pool = new SemaphoreSlim(0, 1);
    var t = new Thread(DoWork);
    var t2 = new Thread(DoWork2);
    t.Start();
    t2.Start();

    void DoWork()
    {
        pool.Wait(); // 等待信号量
    }
    void DoWork2()
    {
        Thread.Sleep(Timeout.Infinite); // 永久休眠
    }
}
```
2. Thread.Sleep() 不仅用于休眠，也可以用于让出当前 CPU 时间，让其他正在等待 CPU 的线程也有机会抢到 CPU 时间。   
   tip：相似的方法，Thread.Yield() 也有让出 CPU 时间的功能。  
   tip：不同的方法，Thread.SpinWait() 不会让出 CPU 控制权，而是进行自旋。
``` c#
Thread.Sleep(0) 让出控制权给同等优先级的线程执行，如果没有，就继续执行本线程。
Thread.Sleep(1) 让出控制权给正在等待的线程执行。
Thread.Yield() 让出控制权给CPU上的其他线程。
Thread.SpinWait() 不让出控制权，在CPU上自旋一段时间。
```
### 线程中断
当线程处于阻塞状态时，其他线程调用阻塞线程的 Thread.Interrupt() 时，会中断线程并抛出 System.Threading.ThreadInterruptedException。  
tip：如果线程没有处于阻塞状态，那么调用 Thread.Interrupt() 则不会有效果。
``` c#
public void Test3()
{
    var sleepSwitch = false;
    var pool = new SemaphoreSlim(0, 1);

    var t = new Thread(DoWork);
    t.Start();
    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 {t.ManagedThreadId} 的 Interrupt()");
    t.Interrupt();
    Thread.Sleep(3000);
    sleepSwitch = true;

    var t2 = new Thread(DoWork2);
    t2.Start();
    Thread.Sleep(2000);
    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} 调用 {t2.ManagedThreadId} 的 Interrupt()");
    t2.Interrupt();

    void DoWork()
    {
        try
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 开始执行");
            while (!sleepSwitch)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 自旋 SpinWait()");
                Thread.SpinWait(10000000); // 只是进行自旋，不阻塞线程，所以不会被中断
            }
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 休眠 Sleep()");
            Thread.Sleep(Timeout.Infinite);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    void DoWork2()
    {
        try
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: 开始执行");
            pool.Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
```
### 取消线程


## 线程异常
### 线程异常会导致程序崩溃
## 线程池操作

## 线程同步
### 死锁和竞争
