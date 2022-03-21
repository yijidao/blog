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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            deadlock.Click += (sender, args) => SyncOverAsyncDeadlock();
            taskRun.Click += (sender, args) => TaskRun();
        }

        public void SyncOverAsyncDeadlock()
        {
            var task = SimulatedWorkAsync();
            task.Wait();
            Debug.WriteLine($"Sync Over");
        }

        public async Task SimulatedWorkAsync()
        {
            await Task.Delay(1000);
            Debug.WriteLine($"Async Over");
        }

        public void TaskRun()
        {
            Debug.WriteLine($"UI => {Thread.CurrentThread.ManagedThreadId}");
            var t = Task.Run(async () =>
            {
                await Task.Delay(1000);
                Debug.WriteLine($"Task => {Thread.CurrentThread.ManagedThreadId}");
            }).ContinueWith(async t =>
            {
                await Task.Delay(100);
                Debug.WriteLine($"Continue Task => {Thread.CurrentThread.ManagedThreadId}");
            });

        }
    }
}
