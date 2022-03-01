## 在 WPF 客户端实现 AOP 和接口缓存
随着业务越来越复杂，最近决定把一些频繁查询但是数据不会怎么变更的接口做一下缓存，这种功能一般用 AOP 就能实现了，找了一下客户端又没现成的直接可以用，嗐，就只能自己开发了。  

### 代理模式和AOP  

理解代理模式后，对 AOP 自然就手到擒来，所以先来点前置知识。  
代理模式是一种使用一个类来控制另一个类方法调用的范例代码。  
代理模式有三个角色：
- ISubject 接口，职责是定义行为。
- ISubject 的实现类 RealSubject，职责是实现行为。
- ISubject 的代理类 ProxySubject，职责是控制对 RealSubject 的访问。  

代理模式有三种实现：
- 普通代理。
- 强制代理，强制的意思就是不能直接访问 RealSubject 的方法，必须通过代理类访问。
- 动态代理，动态的意思是通过反射生成代理类，AOP 一般就是基于动态代理。

AOP 有四个关键知识点：
- 切入点 JoinPoint。就是 RealSubject 中的被控制访问的方法。
- 通知 Advice，就是代理类中的方法，可以控制或者增强 RealSubject 的方法，有前置通知、后置通知、环绕通知等等
- 织入 Weave，就是按顺序调用通知和 RealSubject 方法的过程。
- 切面 Aspect，多个切入点就会形成一个切面。

``` c#
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
```
我使用的是 Castle.Core 这个库来实现动态代理。但是这个代理有返回值的异步方法自己写起来比较费劲，但是 github 已经有不少库封装了实现过程，这里我用 Castle.Core.AsyncInterceptor 来实现异步方法的代理。
``` c#
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
```
### 实现切面类和接口缓存
实现过程：
1. 定义 CacheAttribute 特性来标记需要缓存的方法。
2. 定义 CacheInterceptor 切面，实现在内存缓存数据的逻辑。
3. 使用切面，生成对接口的动态代理类，并且将代理类注入到 IOC 容器中。
4. 界面通过 IOC 取得的接口实现类来访问实现。

客户端使用了 Prism 的 IOC 来实现控制反转，Prism 支持多种 IOC，我这里使用 DryIoc，因为其他几个 IOC 已经不更新了。  
客户端内存缓存使用 Microsoft.Extensions.Caching.Memory，这个算是最常用的了。

- 定义 CacheAttribute 特性来标记需要缓存的方法。
``` c#
[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute : Attribute
{
    public string? CacheKey { get; }
    public long Expiration { get; }

    public CacheAttribute(string? cacheKey = null, long expiration = 0)
    {
        CacheKey = cacheKey;
        Expiration = expiration;
    }

    public override string ToString() => $"{{ CacheKey: {CacheKey ?? "null"}, Expiration: {Expiration} }}";
}

```

- 定义 CacheInterceptor 切面类，实现在内存缓存数据的逻辑
``` c# 
public class CacheInterceptor : AsyncInterceptorBase
    {
        private readonly IMemoryCache _memoryCache;

        public CacheInterceptor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        ...
        // 拦截异步方法
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var attribute = invocation.Method.GetCustomAttribute<CacheAttribute>();
            if (attribute == null)
            {
                return await proceed(invocation, proceedInfo).ConfigureAwait(false);
            }

            var cacheKey = attribute.CacheKey ?? GenerateKey(invocation);
            if (_memoryCache.TryGetValue(cacheKey, out TResult cacheValue))
            {
                if (cacheValue is string[] array)
                {
                    Debug.WriteLine($"[Cache]  Key: {cacheKey}, Value: {string.Join(',', array)}");
                }

                return cacheValue;
            }
            else
            {
                cacheValue = await proceed(invocation, proceedInfo).ConfigureAwait(false);
                _memoryCache.Set(cacheKey, cacheValue);
                return cacheValue;
            }
        }
        // 生成缓存的 Key
        private string GenerateKey(IInvocation invocation)
        {
            ...
        }
        // 格式化一下
        private string FormatArgumentString(ParameterInfo argument, object value)
        {
            ...
        }
    }
```
- 定义扩展类来生成切面，并且实现链式编程，可以方便地对一个接口添加多个切面类。
``` c#
public static class DryIocInterceptionAsyncExtension
{
    private static readonly DefaultProxyBuilder _proxyBuilder = new DefaultProxyBuilder();
    // 生成切面
    public static void Intercept<TService, TInterceptor>(this IRegistrator registrator, object serviceKey = null)
        where TInterceptor : class, IInterceptor
    {
        var serviceType = typeof(TService);

        Type proxyType;
        if (serviceType.IsInterface())
            proxyType = _proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(
                serviceType, ArrayTools.Empty<Type>(), ProxyGenerationOptions.Default);
        else if (serviceType.IsClass())
            proxyType = _proxyBuilder.CreateClassProxyTypeWithTarget(
                serviceType, ArrayTools.Empty<Type>(), ProxyGenerationOptions.Default);
        else
            throw new ArgumentException(
                $"{serviceType} 无法被拦截, 只有接口或者类才能被拦截");

        registrator.Register(serviceType, proxyType,
            made: Made.Of(pt => pt.PublicConstructors().FindFirst(ctor => ctor.GetParameters().Length != 0),
                Parameters.Of.Type<IInterceptor[]>(typeof(TInterceptor[]))),
            setup: Setup.DecoratorOf(useDecorateeReuse: true, decorateeServiceKey: serviceKey));
    }
    // 链式编程，方便添加多个切面
    public static IContainerRegistry InterceptAsync<TService, TInterceptor>(
        this IContainerRegistry containerRegistry, object serviceKey = null)
        where TInterceptor : class, IAsyncInterceptor
    {
        var container = containerRegistry.GetContainer();
        container.Intercept<TService, AsyncInterceptor<TInterceptor>>(serviceKey);
        return containerRegistry;
    }
}
```
- 定义目标接口，并且在方法上标记一下
```c#
public interface ITestService
{
    /// <summary>
    /// 一个查询大量数据的接口
    /// </summary>
    /// <returns></returns>
    [Cache]
    Task<string[]> GetLargeData();
}

public class TestService : ITestService
{
    public async Task<string[]> GetLargeData()
    {
        await Task.Delay(2000);
        var result = new[]{"大","量","数","据"};
        Debug.WriteLine("从接口查询数据");
        return result;
    }
}
```

- 向 IOC 容器注入切面类和业务接口。
```c#
public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // 注入缓存类
        containerRegistry.RegisterSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));
        // 注入切面类
        containerRegistry.Register<AsyncInterceptor<CacheInterceptor>>();
        // 注入接口和应用切面类
        containerRegistry.RegisterSingleton<ITestService, TestService>()
            .InterceptAsync<ITestService, CacheInterceptor>();
        containerRegistry.RegisterSingleton<ITestService2, TestService2>()
            .InterceptAsync<ITestService2, CacheInterceptor>();
    }
    ...
}
```
### 效果
```c#
// AopView.xaml
<Button x:Name="cache" Content="Aop缓存接口数据" />

// AopView.xaml.cs
cache.Click += (sender, args) => ContainerLocator.Container.Resolve<ITestService>().GetLargeData();

// 输出
// 第一次点击打印
// 从接口查询数据

// 之后点击打印
// [Cache]  Key: PrismAop.Service.TestService2.GetLargeData(), Value: 大,量,数,据

```
### 最后
#### 其实还有很多细节可以完善一下，比如说缓存刷新规则，服务端刷新客户端缓存等等，不过客户端 AOP 的实现差不多就这样了。
#### 觉得对你有帮助点个推荐或者留言交流一下呗！
#### 源码 https://github.com/yijidao/blog/tree/master/WPF/PrismAop