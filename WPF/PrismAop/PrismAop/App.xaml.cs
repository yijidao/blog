using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DryIoc;
using Microsoft.Extensions.Caching.Memory;
using Prism.Ioc;
using PrismAop.Extensions;
using PrismAop.Interceptors;
using PrismAop.Service;

namespace PrismAop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));
            containerRegistry.Register<AsyncInterceptor<CacheInterceptor>>();

            containerRegistry.RegisterSingleton<ITestService, TestService>()
                .InterceptAsync<ITestService, CacheInterceptor>();

            containerRegistry.RegisterSingleton<ITestService2, TestService2>()
                .InterceptAsync<ITestService2, CacheInterceptor>();
        }

        protected override Window CreateShell() => new MainWindow();
    }
}
