using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PrintPdf.ViewModels
{
    public class PrintPdf2ViewModel : BindableBase, IDialogAware
    {
        private string _template;
        public string Template
        {
            get => _template;
            set => SetProperty(ref _template, value);
        }

        private ExpandoObject _data;
        public ExpandoObject Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }


        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("template", out _template);
            parameters.TryGetValue("data", out _data);
        }

        public string Title => "预览 PDF";
        public event Action<IDialogResult>? RequestClose;
    }
}
