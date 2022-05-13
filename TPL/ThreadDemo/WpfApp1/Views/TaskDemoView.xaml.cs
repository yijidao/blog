using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfApp1.Views
{
    /// <summary>
    /// TaskDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskDemoView : UserControl
    {
        public TaskDemoView()
        {
            InitializeComponent();
            //Task<string>? task = null;
            CancellationTokenSource? cts = null;
            
            createdTask.Click +=  (sender, args) =>
            {
                cts = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        await Task.Delay(2000, cts.Token);
                        Debug.WriteLine("keepAlive");
                    }
                    
                });
                //var t = ServiceDemo.GetAsync("Created");
                //if (t.Status == TaskStatus.Created)
                //{
                //    t.Start();
                //}
                //Debug.WriteLine(await t);
            };

            

            cancelTask.Click += async (_, _) =>
            {
                cts?.Cancel();
                //if (task == null)
                //{
                //    cts = new CancellationTokenSource();
                //    task = ServiceDemo.GetAsync("", cts.Token);
                //    Debug.WriteLine(await task);
                //    cts = null;
                //    task = null;
                //    return;
                //}
                //cts?.Cancel();

            };
        }
    }
}
