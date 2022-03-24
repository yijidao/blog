using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfApp1.Views
{
    /// <summary>
    /// TaskExceptionView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskExceptionView : UserControl
    {
        public TaskExceptionView()
        {
            InitializeComponent();
            fromException.Click += async (sender, args) =>
            {
                //ServiceDemo.GetAsyncWithException("RunError");

                //await ServiceDemo.GetAsyncWithException("RunError").ContinueWith(t =>
                //{
                //    if (t.IsFaulted)
                //    {
                //        Debug.WriteLine(t.Exception.Message);
                //    }
                //});


                try
                {
                    await ServiceDemo.GetAsyncWithException("RunError");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            };

            throwException.Click += async (sender, args) =>
            {
                //ServiceDemo.GetAsyncWithException("UsageError");

                //await ServiceDemo.GetAsyncWithException("UsageError").ContinueWith(t =>
                //{
                //    if (t.IsFaulted)
                //    {
                //        Debug.WriteLine(t.Exception.Message);
                //    }
                //});

                try
                {
                    await ServiceDemo.GetAsyncWithException("UsageError");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            };

        }
    }
}
