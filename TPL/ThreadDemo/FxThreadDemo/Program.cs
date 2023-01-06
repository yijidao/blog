using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FxThreadDemo
{
    #region 在网上找到了一个讨论 https://social.msdn.microsoft.com/Forums/vstudio/en-US/005ba845-6763-4da3-ad53-5bb6c5bc7569/locking-with-monitor-across-appdomain?forum=netfxbcl


    //class Program

    //{

    //    static void Main(string[] args)

    //    {

    //        #region MyRegion

    //        Console.WriteLine("Thread Start " + Thread.CurrentThread.ManagedThreadId);

    //        int mainThreadId = Thread.CurrentThread.ManagedThreadId;

    //        string assembly = Assembly.GetEntryAssembly().FullName;

    //        AppDomain otherDomain = AppDomain.CreateDomain("other_domain");

    //        RemoteObject ro = null;

    //        ro = (RemoteObject)otherDomain.CreateInstanceAndUnwrap(assembly, typeof(RemoteObject).FullName);

    //        Thread newThread = new Thread(new ThreadStart(() => ro.Print(mainThreadId)));

    //        newThread.Start();

    //        ro.Print(mainThreadId);


    //        Console.ReadLine();

    //        #endregion

    //    }

    //}


    //public class RemoteObject : System.MarshalByRefObject

    //{

    //    public RemoteObject()

    //    {


    //    }


    //    public void Print(int threadId)

    //    {

    //        if (Thread.CurrentThread.ManagedThreadId == threadId)

    //        {

    //            Thread.Sleep(5000);

    //            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "sleep over and trys to access the blocked code");

    //        }

    //        lock (this)

    //        {

    //            if (Thread.CurrentThread.ManagedThreadId == threadId)

    //            {

    //                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "comes into the blocked code");

    //            }


    //            if (Thread.CurrentThread.ManagedThreadId != threadId)

    //            {

    //                int i = 0;

    //                while (i < 10)

    //                {

    //                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "is in blocked code");

    //                    Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);

    //                    Thread.Sleep(1000);

    //                    i++;

    //                }

    //                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "goes out the blocked code");

    //            }

    //        }

    //    }

    //}
    #endregion


    /// <summary>
    /// 这个 demo 是为了测试 Monitor 结合派生自 MarshalByRefObject 的对象，是否能实现不同 AppDomain 之间的独占锁。
    /// 有这个想法是因为在官网看到了一句话
    /// Note that you can synchronize on an object in multiple application domains if the object used for the lock derives from MarshalByRefObject.
    ///
    /// https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor?view=net-7.0
    /// 经过测试，Monitor 结合 MarshalByRefObject 的子类也不能实现不同 AppDomain 之间的独占锁，所以官网这句话就很怪哈。
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {

            var newDomain = AppDomain.CreateDomain("NewDomain");
            newDomain.Load(typeof(Worker).Assembly.FullName);
            var newDomainWorker = (Worker)newDomain.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, typeof(Worker).FullName);

            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Enter()");

                    Monitor.Enter(newDomainWorker);
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Wait()");
                    Monitor.Wait(newDomainWorker);

                }
                finally
                {
                    Monitor.Exit(newDomainWorker);
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Exit()");

                }
            });


            Console.ReadKey();
            newDomainWorker.TestPulse();
            Console.ReadKey();
            newDomainWorker.TestPulse();
            Console.ReadKey();

        }



    }
    public class Worker : MarshalByRefObject
    {
        public Worker()
        {
            TestWait();
        }


        public DomainLock LockObject { get; } = new DomainLock();

        public void TestWait()
        {
            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Enter()");

                    Monitor.Enter(LockObject);
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Wait()");
                    Monitor.Wait(LockObject);

                }
                finally
                {
                    Monitor.Exit(LockObject);
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Exit()");

                }
            });
        }


        public void TestPulse()
        {
            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Enter()");

                    Monitor.Enter(LockObject);
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Pulse()");
                    Monitor.Pulse(LockObject);

                }
                finally
                {
                    Monitor.Exit(LockObject);
                    Debug.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} 调用 Exit()");

                }
            });
        }
    }



    public class DomainLock : MarshalByRefObject
    {

    }
}
