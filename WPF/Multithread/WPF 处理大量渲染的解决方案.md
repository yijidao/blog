## 用 UI 多线程处理 WPF 大量渲染的解决方案
众所周知， WPF 的 UI 渲染是单线程的，所以如果我们异步或者新建线程去进行数据处理的时候，处理完，想要更新 UI 的时候，需要调用一下 Dispatcher.Invoke，将处理完的数据推入到  Dispatcher 中，等待更新界面，不然就会报调用线程无法访问此对象，因为另一个线程拥有该对象的错误。  
这就是为什么 WPF 中的大多数对象派生自 DispatcherObject，因为需要 Dispatcher 处理并发和线程的情况。  
但是工作中，有时候会遇到 UI 密集型的情况，也就是界面不停渲染导致界面出现卡住的情况，这时候就是 Dispatcher 忙不过来了，一般这时候，都会想要用多线程来渲染界面，但是一个 Dispatcher 只能为一个线程服务，所以我一般会将这种 UI 密集型的界面，单独放到一个弹窗中，再新建一个 UI 线程并指定 Dispatcher 来渲染。  
### 样例代码
代码也很简单，我直接贴啦！  
我在界面中放了两个按钮，一个用来打开负责大量渲染的窗口，一个用来关闭该窗口。
``` c#
Window w = null;

newWindowButton.Click += (sender, args) =>
{
    var thread = new Thread(() =>
    {
        w = new Window
        {
            Content = new LargeRenderView(),
            Width = 1200,
            Height = 1000
        };
        w.Show();
        Dispatcher.Run();  // 运行 Dispatcher，为新建的 UI 线程服务

    });
    thread.SetApartmentState(ApartmentState.STA); //  // 指定线程为单线程模式
    thread.Start();
};

closeWindowButton.Click += (sender, args) =>
{
    w.Close();
    if (w == null) return;
    if (w.Dispatcher.CheckAccess())
        w.Close();
    else
        w.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(w.Close));
            };

```
## 效果
可以看到新弹窗因为大量渲染，鼠标一直在转圈，无法操作，但是主窗口还是可以进行 UI 操作，所以主窗口没有被这个大量渲染影响到。
![效果](https://s4.ax1x.com/2022/03/04/ban4fK.gif)

## 最后
#### 这业务场景还有什么有用的解决方案，请留言教教我！
#### 觉得对你有帮助点个推荐或者留言交流一下呗！
#### 源码 https://github.com/yijidao/blog/tree/master/WPF/Multithread
#### WPF 线程模型文档 https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/threading-model?redirectedfrom=MSDN&view=netframeworkdesktop-4.8
