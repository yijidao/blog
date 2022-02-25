# 使用 WebView2 封装一个生成 PDF 的 WPF 控件

最近在迁移项目到 .net6，发现项目中用的 PDF 库不支持 .net6，于是想着换一个库。结果找了一大圈，发现不是版本不支持，就是收费。  
嗐！还能咋办，只能自己搞一个 PDF 生成控件咯。  
## 环境准备 WPF + WebView2 + Vue
### WebView2
- WebView2.CoreWebView2.PrintToPdfAsync 可以将 html 文件生成 pdf。
- CEF 也有类似的 API，Evergreen WebView2 会自动更新，而且不需要将库打包到程序中，所以就用它了。
- WebView2 需要先安装到本机，[下载链接](https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section)。
### Vue
- 直接操作 Dom 不够方便，Vue 用法跟 WPF 的绑定方式又很相似，使用 vue 来定义 pdf 的 Html 的模板，可以让不会 h5 的同事也能轻松写模板文件，所以这里用 Vue 来操作 Dom 和数据绑定。
  
### Prism
- WPF 项目常用的框架，我这里用来注册预览 PDF 的弹窗，以及给弹窗传参。

## 以打印一个表格为例
### 1. 定义要生成 PDF 的表格
``` c#
// BuyBookView.xaml
<DataGrid
    Grid.Row="1"
    Margin="24,0"
    AutoGenerateColumns="False"
    FontSize="16"
    IsReadOnly="True"
    ItemsSource="{Binding Books}"
    TextBlock.TextAlignment="Center">

    <DataGrid.Columns>
        <DataGridTextColumn
            Width="*"
            Binding="{Binding Title}"
            Header="书名"
            HeaderStyle="{StaticResource CenterGridHeaderStyle}" />
        <DataGridTextColumn
            Width="100"
            Binding="{Binding Author}"
            Header="作者"
            HeaderStyle="{StaticResource CenterGridHeaderStyle}" />
        <DataGridTextColumn
            Width="100"
            Binding="{Binding Price}"
            Header="价格"
            HeaderStyle="{StaticResource CenterGridHeaderStyle}" />
    </DataGrid.Columns>
</DataGrid>

// BuyBookViewModel
public BuyBookViewModel(IDialogService dialogService)
{
    Title = "鸭霸的购书目录";
    Books = new List<Book>
    {
        new()
        {
            Title = "JavaScript权威指南 原书第7版",
            Author = "巨佬1",
            Price = 90.3
        },
        new()
        {
            Title = "深入浅出node.js",
            Author = "巨佬2",
            Price = 57.8
        },
        new()
        {
            Title = "编码：隐匿在计算机软硬件背后的语言",
            Author = "巨佬3",
            Price = 89.00
        }
    };
}
```
### 2. 定义预览 PDF 的弹窗
- 在 xaml 中引入 WebView2
``` c#
// PrintPdfView.xml
...
xmlns:wpf="clr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf"
...

<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <Grid>
        <wpf:WebView2 x:Name="webView2" />
    </Grid>
    <Grid Grid.Row="1">
        <Button
            x:Name="save"
            HorizontalAlignment="Right"
            Content="保存" />
    </Grid>
</Grid>
```
- 在 viewmodel 中定义弹窗接收的参数以及弹窗的属性
``` c#
// PrintPdfViewModel.cs
public class PrintPdfViewModel : BindableBase, IDialogAware
{
    private string _template;
    /// <summary>
    /// PDF 的 html 模板
    /// </summary>
    public string Template
    {
        get => _template;
        set => SetProperty(ref _template, value);
    }

    private ExpandoObject _data;
    /// <summary>
    /// 传递给 pdf 的数据
    /// </summary>
    public ExpandoObject Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
    }
    
    public void OnDialogOpened(IDialogParameters parameters)
    {
        // 弹窗接收 template 和 data 两个参数
        parameters.TryGetValue("template", out _template);
        parameters.TryGetValue("data", out _data);
    }

    public string Title => "预览 PDF";
}

```
## 3. 定义 WebView2 生成 PDF 的逻辑和 pdf 的模板文件
- 使用 vue 来定义 pdf 模板的逻辑，和调用 WebView2.CoreWebView2.PrintToPdfAsync 来生成 PDF。
- 因为客户端经常运行在内网或无网环境，所以这里就不用 cdn 引入 vuejs，而是直接将 vuejs 嵌入到客户端的资源文件中。
- 调用 WebView2.CoreWebView2.PostWebMessageAsJson 从 WPF 向 WebView2 发送数据。
``` c# 
// PrintPdfViewModel.xaml.cs
/// <summary>
/// 配置 WebView2，加载 vuejs，加载 pdf 模板，传递数据到 html 中
/// </summary>
/// <returns></returns>
private async Task Load()
{
    await webView2.EnsureCoreWebView2Async();
    webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false; // 禁止右键菜单

    var assembly = Assembly.GetExecutingAssembly();
    var resourceName = "PrintPdf.Views.vue.global.js";

    using var stream = assembly.GetManifestResourceStream(resourceName);
    if (stream != null)
    {
        using var reader = new StreamReader(stream);
        var vue = await reader.ReadToEndAsync();
        await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(vue);  // 加载 vuejs 
    }

    var vm = (PrintPdfViewModel)DataContext;

    webView2.CoreWebView2.NavigateToString(vm.Template); // 加载 pdf 模板

    webView2.CoreWebView2.NavigationCompleted += (sender, args) =>
    {
        var json = JsonSerializer.Serialize(vm.Data);
        webView2.CoreWebView2.PostWebMessageAsJson(json); // 将数据传递到 html 中
    };
}
```
- 点击保存时，选择路径并生成 PDF 文件。
``` c#
// PrintPdfViewModel.xaml.cs
save.Click += async (sender, args) =>
{
    var saveFileDialog = new SaveFileDialog
    {
        Filter = "txt files (*.pdf)|*.pdf",
        RestoreDirectory = true,
        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        FileName = $"test.pdf"
    };
    var result = saveFileDialog.ShowDialog();

    if (result != true)
        return;

    var printSetting = webView2.CoreWebView2.Environment.CreatePrintSettings();
    printSetting.ShouldPrintBackgrounds = true;

    var saveResult = await webView2.CoreWebView2.PrintToPdfAsync($"{saveFileDialog.FileName}", printSetting);
};

```
- 定义 pdf 的打印模板，并且使用 Vue 来实现绑定功能，调用 webview.addEventListener 来监听 WPF 传递给 WebView2 的数据。

``` html
<html lang="en">
<head>
   ...
</head>

<body>
    <div id="app">
        <div id="header">
            <h3>
                {{title}}
            </h3>
        </div>
        <div id="content">
            <table>
                <thead>
                    <tr>
                        <th>序号</th>
                        <th>书名</th>
                        <th>作者</th>
                        <th>价格</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, i) in books">
                        <th>{{i+1}}</th>
                        <td>{{item.Title}}</td>
                        <td>{{item.Author}}</td>
                        <td>{{item.Price}}</td>
                    </tr>
                </tbody>
            </table>

        </div>
    </div>
</body>
<script>
    // 调用 webview.addEventListener 来监听 WPF 传递给 WebView2 的数据。
    window.chrome.webview.addEventListener('message', event => generate(event.data));
    // 完成数据绑定
    function generate(data) {
        const app = Vue.createApp({
            data() {
                return {title, books} = data;
            },
        });
        app.mount('#app');
    }
</script>

</html>
```
- 在 WPF 客户端点击生成 PDF 时，打开 PDF 预览窗口，并且传递模板和数据给 WebView2
``` c# 
// BuyBookView.xaml
<Button Command="{Binding ShowPrintViewCommand}" Content="预览 PDF1 " />

 // BuyBookViewModel
ShowPrintViewCommand = new DelegateCommand(() =>
{
    var assembly = Assembly.GetExecutingAssembly();
    var resourceName = $"PrintPdf.ViewModels.test_print.html";

    using var stream = assembly.GetManifestResourceStream(resourceName); // 加载模板
    if (stream == null) return;
    using var reader = new StreamReader(stream);
    var t = reader.ReadToEnd();
    dynamic d = new ExpandoObject(); // 转换数据
    d.title = Title;
    d.books = Books;

    var p = new DialogParameters
    {
        {"template", t},
        {"data", d}
    };
    dialogService.ShowDialog(nameof(PrintPdfView), p, null);
});
```
## 4. 效果
