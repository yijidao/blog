using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            button1.Click += (sender, args) => SyncOverAsyncDeadlock();
        }

        public void SyncOverAsyncDeadlock()
        {
            var task = SimulatedWorkAsync();
            task.Wait();
            Console.WriteLine($"Sync Over");
        }

        public async Task SimulatedWorkAsync()
        {
            await Task.Delay(1000);
            Console.WriteLine($"Async Over");
        }
    }
}
