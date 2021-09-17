using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultipleProcessClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Process> _pluginProcessList = new List<Process>();

        public MainWindow()
        {
            InitializeComponent();

            load1.Click += (sender, args) =>
            {
                var path = $@"{Directory.GetCurrentDirectory()}\Plugin1.dll";
                var plugin = LoadPlugin(path);
                region1.Child = plugin;
            };

            load2.Click += (sender, args) =>
            {
                var path = $@"{Directory.GetCurrentDirectory()}\Plugin2.dll";
                var plugin = LoadPlugin(path);
                region2.Child = plugin;
            };

            Application.Current.Exit += (sender, args) =>
            {
                _pluginProcessList.Where(x => !x.HasExited).ToList().ForEach(x => x.Kill());
            };
        }

        private FrameworkElement LoadPlugin(string pluginDll)
        {
            using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "PluginProcess.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"{pluginDll} {pipeServer.GetClientHandleAsString()}"
                };

                var process = new Process { StartInfo = startInfo };
                process.Start();
                _pluginProcessList.Add(process);
                pipeServer.DisposeLocalCopyOfClientHandle();
                using (var reader = new StreamReader(pipeServer))
                {
                    var handle = new IntPtr(int.Parse(reader.ReadLine()));
                    return new ViewHost(handle);
                }
            }
        }
    }

    class ViewHost : HwndHost
    {
        private readonly IntPtr _handle;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);

        public ViewHost(IntPtr handle) => _handle = handle;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            SetParent(new HandleRef(null, _handle), hwndParent);
            return new HandleRef(this, _handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
        }
    }
}
