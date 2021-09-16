using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace PluginProcess
{
    class Program
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        static void Main(string[] args)
        {
            if (args.Length != 2) return;

            var dllPath = args[0];
            var serverHandle = args[1];
            var dll = Assembly.LoadFile(dllPath);
            var startupType = dll.GetType($"{dll.GetName().Name}.PluginStartup");
            var startup = Activator.CreateInstance(startupType);
            var view =(FrameworkElement)  startupType.GetMethod("CreateView").Invoke(startup, null);
            

            using (var pipeCline = new AnonymousPipeClientStream(PipeDirection.Out, serverHandle))
            {
                using (var writer = new StreamWriter(pipeCline))
                {
                    writer.AutoFlush = true;
                    var handle = ViewToHwnd(view);
                    writer.WriteLine(handle.ToInt32());
                }
            }
            Dispatcher.Run();
        }

        private static IntPtr ViewToHwnd(FrameworkElement element)
        {
            var p = new HwndSourceParameters()
            {
                ParentWindow = new IntPtr(-3),
                WindowStyle = 1073741824
            };
            var hwndSource= new HwndSource(p)
            {
                RootVisual = element,
                SizeToContent = SizeToContent.Manual,
            };
            hwndSource.CompositionTarget.BackgroundColor = Colors.White;
            return hwndSource.Handle;
        }
    }



}
