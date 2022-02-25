using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
using Microsoft.Win32;
using PrintPdf.ViewModels;

namespace PrintPdf.Views
{
    /// <summary>
    /// PrintPdfView.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPdfView : UserControl
    {
        public PrintPdfView()
        {
            InitializeComponent();
            Load();

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
        }

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
    }
}
