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
    /// PrintPdf2View.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPdf2View : UserControl
    {
        public PrintPdf2View()
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

        private async Task Load()
        {
            await webView2.EnsureCoreWebView2Async();
            webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

            var assembly = Assembly.GetExecutingAssembly();
            var vueFile = "PrintPdf.Views.vue.global.js";
            var controlsFile = "PrintPdf.Views.controls.js";

            using var vueStream = assembly.GetManifestResourceStream(vueFile);
            using var controlsStream = assembly.GetManifestResourceStream(controlsFile);
            if (vueStream == null || controlsStream == null)
                return;

            using var vueReader = new StreamReader(vueStream);
            using var controlsReader = new StreamReader(controlsStream);
            var vue = await vueReader.ReadToEndAsync();
            await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(vue);
            var controls = await controlsReader.ReadToEndAsync();
            await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(controls);


            var vm = (PrintPdf2ViewModel)DataContext;

            webView2.CoreWebView2.NavigateToString(vm.Template);

            webView2.CoreWebView2.NavigationCompleted += (sender, args) =>
            {
                var json = JsonSerializer.Serialize(vm.Data);
                webView2.CoreWebView2.PostWebMessageAsJson(json);
            };
        }
    }
}
