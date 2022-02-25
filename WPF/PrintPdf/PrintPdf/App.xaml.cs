using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PrintPdf.Views;
using Prism.Ioc;

namespace PrintPdf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<PrintPdfView>();
            containerRegistry.RegisterDialog<PrintPdf2View>();
        }

        protected override Window CreateShell() => new MainWindow();
    }
}
