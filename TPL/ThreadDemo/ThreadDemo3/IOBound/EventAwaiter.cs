using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.IOBound
{
    /// <summary>
    /// 自定义 Awaiter
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    internal class EventAwaiter<TEventArgs> : INotifyCompletion
    {

        private ConcurrentQueue<TEventArgs> _events = new();
        private Action? _continuation;

        public void OnCompleted(Action continuation)
        {
            Volatile.Write(ref _continuation, continuation);
        }

        public void EventRaised(object? sender, TEventArgs eventArgs)
        {
            _events.Enqueue(eventArgs);
            var continuation = Interlocked.Exchange(ref _continuation, null);
            continuation?.Invoke();
        }

        #region 状态机代码

        public EventAwaiter<TEventArgs> GetAwaiter() => this;

        public bool IsCompleted => _events.Count > 0;

        public TEventArgs? GetResult()
        {
            _events.TryDequeue(out var value);
            return value;
        }

        #endregion
    }

    internal class EventAwaiterTest
    {
        public void Test()
        {
            ShowException();

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    switch (i)
                    {
                        case 0: throw new InvalidOperationException();
                        case 1: throw new ObjectDisposedException("");
                        case 2: throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public async void ShowException()
        {
            var eventAwaiter = new EventAwaiter<FirstChanceExceptionEventArgs>();
            AppDomain.CurrentDomain.FirstChanceException += eventAwaiter.EventRaised;
            while (true)
            {
                var eventArgs = await eventAwaiter;
                Console.WriteLine($"AppDomain Exception: {eventArgs?.Exception.GetType()}");
            }
        }
    }
}
