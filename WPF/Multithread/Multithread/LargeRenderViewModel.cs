using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace Multithread
{
    public class LargeRenderViewModel
    {
        public object Sync { get; }

        private readonly Random _r = new();

        public ISeries[] Series { get; set; }

        private readonly ObservableCollection<ObservableValue> _values;

        private readonly int _delay = 1;

        public LargeRenderViewModel()
        {
            Sync = new object();

            var items = new List<ObservableValue>();
            for (var i = 0; i < 150; i++)
            {
                items.Add(new ObservableValue(_r.Next(0, 10)));
            }

            _values = new ObservableCollection<ObservableValue>(items);

            Series = new ISeries[]
            {
                new LineSeries<ObservableValue>()
                {
                    Values = _values
                }
            };

            int readTasks = 1;
            for (int i = 0; i < readTasks; i++)
            {
                Task.Run(ReadData);
            }
        }

        private async Task? ReadData()
        {
            await Task.Delay(1000);
            while (true)
            {
                await Task.Delay(_delay);
                lock (Sync)
                {
                    _values.Add(new ObservableValue(_r.Next(0, 10)));
                    _values.RemoveAt(0);
                }
            }
        }
    }
}
