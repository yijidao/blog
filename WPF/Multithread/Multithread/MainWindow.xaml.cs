using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Multithread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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
                    Dispatcher.Run(); // 运行 Dispatcher，为新建的 UI 线程服务

                });
                thread.SetApartmentState(ApartmentState.STA);  // 指定线程为单线程模式
                thread.Start();
            };

            closeWindowButton.Click += (sender, args) =>
            {
                if (w == null) return;
                if (w.Dispatcher.CheckAccess())
                    w.Close();
                else
                    w.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(w.Close));
            };
        }
    }
}
