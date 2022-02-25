using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrintPdf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace PrintPdf.ViewModels
{
    public class BuyBookViewModel : BindableBase
    {
        private List<Book> _books;
        private string _title;

        public List<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ICommand ShowPrintViewCommand { get; }

        public ICommand ShowPrintView2Command { get; }

        public BuyBookViewModel(IDialogService dialogService)
        {
            Title = "鸭霸的购书目录";
            Books = new List<Book>
            {
                new()
                {
                    Title = "JavaScript权威指南 原书第7版",
                    Author = "巨佬1",
                    Price = 90.3
                },
                new()
                {
                    Title = "深入浅出node.js",
                    Author = "巨佬2",
                    Price = 57.8
                },
                new()
                {
                    Title = "编码：隐匿在计算机软硬件背后的语言",
                    Author = "巨佬3",
                    Price = 89.00
                }
            };

            ShowPrintViewCommand = new DelegateCommand(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"PrintPdf.ViewModels.test_print.html";

                using var stream = assembly.GetManifestResourceStream(resourceName); // 加载模板
                if (stream == null) return;
                using var reader = new StreamReader(stream);
                var t = reader.ReadToEnd();
                dynamic d = new ExpandoObject(); // 转换数据
                d.title = Title;
                d.books = Books;

                var p = new DialogParameters
                {
                    {"template", t},
                    {"data", d}
                };
                dialogService.ShowDialog(nameof(PrintPdfView), p, null);

            });

            ShowPrintView2Command = new DelegateCommand(() =>
            {

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"PrintPdf.ViewModels.test_print2.html";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) return;
                using var reader = new StreamReader(stream);
                var t = reader.ReadToEnd();
                dynamic d = new ExpandoObject();
                d.title =Title;
                d.books = Books;
                d.columns = new List<Column>
                {
                    new()
                    {
                        Header = "书名",
                        Binding = nameof(Book.Title)
                    },
                    new()
                    {
                        Header = "作者",
                        Binding = nameof(Book.Author)
                    },
                    new()
                    {
                        Header = "价格",
                        Binding = nameof(Book.Price)
                    },
                };
                var p = new DialogParameters
                {
                    {"template", t},
                    {"data", d}
                };
                dialogService.ShowDialog(nameof(PrintPdf2View), p, null);
            });
        }
    }


    public class Book
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public double Price { get; set; }
    }

    public class Column
    {
        public string Header { get; set; }

        public string Binding { get; set; }
    }
}
