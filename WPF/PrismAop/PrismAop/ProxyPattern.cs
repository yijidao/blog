using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Castle.DynamicProxy;

namespace PrismAop
{
    /// <summary>
    /// 代理模式
    /// 代理模式实现了一个类对另一个类方法调用的控制。
    /// 
    /// 代理模式一般有三个角色
    /// - ISubject 接口
    /// - ISubject 的实现类 RealSubject
    /// - ISubject 的代理类 ProxySubject
    ///
    /// 代理模式又一般分为三种
    /// - 普通代理
    /// - 强制代理
    /// - 动态代理，AOP 就是通过动态代理实现
    ///
    /// AOP 有四个关键知识点
    /// - 切入点(JoinPoint)，就是被代理类中的方法
    /// - 通知(Advice)，通知就是代理类中实现控制的方法，一般有前置通知、后置通知、环绕通知等等
    /// - 织入(Weave)，就是按顺序调用通知和被代理目标方法的过程
    /// - 切面（Aspect)，多个切入点就是一个切面
    /// 
    /// </summary>
    public class ProxyPattern
    {
        public void InvokeRealSubject()
        {
            ISubject subject = new RealSubject();
            Format("不通过代理直接调用", () => subject.DoSomething("目标类 DoSomething"));
        }

        public void InvokeCommonProxy()
        {
            ISubject subject = new Proxy();
            Format("普通代理", () => subject.DoSomething("目标类 DoSomething"));
        }

        public void InvokeDynamicProxy()
        {
            var generator = new ProxyGenerator();
            ISubject subject = generator.CreateInterfaceProxyWithTarget<ISubject>(new RealSubject(), new CastleInterceptor());
            Format("动态代理", () => subject.DoSomething("目标类 DoSomething"));
        }

        public void InvokeDynamicProxyAsync()
        {
            var generator = new ProxyGenerator();
            ISubject subject = generator.CreateInterfaceProxyWithTarget<ISubject>(new RealSubject(), new AsyncCastleInterceptor());
            Format("动态代理异步方法", () => subject.DoSomethingAsync("目标类 DoSomething"));
        }

        private void Format(string msg, Action action)
        {
            Debug.WriteLine($"--------- start ---------");
            Debug.WriteLine($"--------- {msg} ---------");
            action.Invoke();
            Debug.WriteLine($"--------- end ---------");
        }

        private async Task Format(string msg, Func<Task> action)
        {
            Debug.WriteLine($"--------- start ---------");
            Debug.WriteLine($"--------- {msg} ---------");
            await action.Invoke();
            Debug.WriteLine($"--------- end ---------");
        }
    }

    public interface ISubject
    {
        
        void DoSomething(string value);

        Task DoSomethingAsync(string value);
    }

    public class RealSubject : ISubject
    {
        public void DoSomething(string value)
        {
            Debug.WriteLine(value);
        }

        public async Task DoSomethingAsync(string value)
        {
            await Task.Delay(2000);
            Debug.WriteLine(value);
        }
    }

    public class Proxy : ISubject
    {
        private readonly ISubject _realSubject;

        public Proxy()
        {
            _realSubject = new RealSubject();
        }

        /// <summary>
        /// 这就是切入点
        /// </summary>
        /// <param name="value"></param>
        public void DoSomething(string value)
        {
            // 这个过程就是织入
            Before();
            _realSubject.DoSomething(value);
            After();
        }

        public Task DoSomethingAsync(string value)
        {
            throw new NotImplementedException();
        }

        public void Before()
        {
            Debug.WriteLine("普通代理类前置通知");
        }

        public void After()
        {
            Debug.WriteLine("普通代理类后置通知");
        }
    }

    public class CastleInterceptor : StandardInterceptor
    {
        protected override void PostProceed(IInvocation invocation)
        {
            Debug.WriteLine("Castle 代理类前置通知");

        }

        protected override void PreProceed(IInvocation invocation)
        {
            Debug.WriteLine("Castle 代理类后置通知");
        }
    }

    public class AsyncCastleInterceptor : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Before();
            await proceed(invocation, proceedInfo);
            After();
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Before();
            var result = await proceed(invocation, proceedInfo);
            After();
            return result;
        }

        public void Before()
        {
            Debug.WriteLine("异步 Castle 代理类前置通知");
        }

        public void After()
        {
            Debug.WriteLine("异步 Castle 代理类后置通知");
        }
    }

}
