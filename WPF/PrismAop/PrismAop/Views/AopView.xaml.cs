using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using PrismAop.Extensions;
using PrismAop.Interceptors;
using PrismAop.Service;

namespace PrismAop.Views
{
    /// <summary>
    /// AopView.xaml 的交互逻辑
    /// </summary>
    public partial class AopView : UserControl
    {
        public AopView()
        {
            InitializeComponent();
            var proxyPattern = new ProxyPattern();

            common.Click += (sender, args) => proxyPattern.InvokeCommonProxy();
            target.Click += (sender, args) => proxyPattern.InvokeRealSubject();
            dynamic.Click += (sender, args) => proxyPattern.InvokeDynamicProxy();
            dynamicAsync.Click += (sender, args) => proxyPattern.InvokeDynamicProxyAsync();
            cache.Click += (sender, args) => ContainerLocator.Container.Resolve<ITestService>().GetLargeData();
            cache2.Click += (sender, args) => ContainerLocator.Container.Resolve<ITestService2>().GetLargeData();
            cache3.Click += (sender, args) => ContainerLocator.Container.Resolve<ITestService>().Get("1", "2", "3");
            cache4.Click += (sender, args) =>
            {
                var result = ContainerLocator.Container.Resolve<ITestService>().Get(new List<string>
                {
                    "1", "2", "3"
                }).Result;

                foreach (var r in result)
                {
                    Debug.WriteLine(r);
                }
            };
        }
    }
}
