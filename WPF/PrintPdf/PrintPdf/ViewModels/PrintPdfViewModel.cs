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
    public class PrintPdfViewModel : BindableBase, IDialogAware
    {
        private string _template;
        /// <summary>
        /// PDF 的 html 模板
        /// </summary>
        public string Template
        {
            get => _template;
            set => SetProperty(ref _template, value);
        }

        private ExpandoObject _data;
        /// <summary>
        /// 传递给 pdf 的数据
        /// </summary>
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
            // 弹窗接收 template 和 data 两个参数
            parameters.TryGetValue("template", out _template);
            parameters.TryGetValue("data", out _data);
        }

        public string Title => "预览 PDF";
        public event Action<IDialogResult>? RequestClose;
    }
}
