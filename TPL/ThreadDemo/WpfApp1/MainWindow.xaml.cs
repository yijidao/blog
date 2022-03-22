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
            taskRun2.Click += (sender, args) => TaskRun2();
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

        private async Task TaskRun2()
        {
            Debug.WriteLine($"UI => {Thread.CurrentThread.ManagedThreadId}");
            await PrismAsync(5000000).ConfigureAwait(false);
            Debug.WriteLine($"TaskRun2 after => {Thread.CurrentThread.ManagedThreadId}");

        }

        private Task PrismAsync(long size)
        {
            Debug.WriteLine($"PrimeAsync before => {Thread.CurrentThread.ManagedThreadId}");
            var t = Task.Run(() =>
            {
                Debug.WriteLine($"PrimeAsync => {Thread.CurrentThread.ManagedThreadId}");

                for (long i = 3; i < size; i++)
                {
                    for (long j = 2; j < Math.Sqrt(i); j++)
                    {
                        if (i % j == 0) break;
                    }
                }
            });
            Debug.WriteLine($"PrimeAsync after => {Thread.CurrentThread.ManagedThreadId}");
            return t;
        }

        private void Prime(long size)
        {
            Console.WriteLine($"Prime => {Thread.CurrentThread.ManagedThreadId}");
            for (long i = 3; i < size; i++)
            {
                for (long j = 2; j < Math.Sqrt(i); j++)
                {
                    if (i % j == 0) break;
                }
            }
        }
    }
}
