using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.IOBound
{
    /// <summary>
    /// 未完成 Task 日志类
    /// </summary>
    internal static class TaskLogger
    {
        public enum TaskLogLevel
        {
            None,
            Pending
        }

        public static TaskLogLevel LogLevel { get; set; }

        private static ConcurrentDictionary<Task, TaskLogEntry> _log = new();

        public static IEnumerable<TaskLogEntry> GeTaskLogEntries => _log.Values;

        public sealed class TaskLogEntry
        {
            public Task Task { get; set; }

            public string? Tag { get; set; }

            public DateTime LogTime { get; set; }

            public string? CallerMemberName { get; set; }

            public string? CallerFilePath { get; set; }

            public int CallerLineNumber { get; set; }

            public override string ToString()
            {
                return $"LogTime={LogTime}, Tag={Tag}, Member={CallerMemberName}, File={CallerFilePath}({CallerLineNumber})";
            }
        }

        /// <summary>
        /// 记录 Task 的详细信息
        /// </summary>
        /// <param name="task"></param>
        /// <param name="tag"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public static Task Log(this Task task, string? tag = null, [CallerMemberName] string? callerMemberName = null, [CallerFilePath] string? callerFilePath = null, [CallerLineNumber] int callerLineNumber = -1)
        {
            if (LogLevel == TaskLogLevel.None)
            {
                return task;
            }

            var entry = new TaskLogEntry
            {
                Tag = tag,
                Task = task,
                LogTime = DateTime.Now,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
            _log[task] = entry;

            task.ContinueWith(t =>
            {
                _log.TryRemove(t, out var _);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }

        public static Task<TResult> Log<TResult>(this Task<TResult> task, string? tag = null,
            [CallerMemberName] string? callerMemberName = null, [CallerFilePath] string? callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            return (Task<TResult>)Log((Task)task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 取消 IO 操作
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="originTask"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> originTask, CancellationToken ct)
        {
            var cancelTask = new TaskCompletionSource();

            ct.Register(() => cancelTask.TrySetResult());

            var t = await Task.WhenAny(originTask, cancelTask.Task);

            if (t == cancelTask.Task)
            {
                ct.ThrowIfCancellationRequested();
            }
            return await originTask;
        }

        /// <summary>
        /// 取消 IO 操作
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="originTask"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task WithCancellation(this Task originTask, CancellationToken ct)
        {
            var cancelTask = new TaskCompletionSource();

            ct.Register(() => cancelTask.TrySetResult());

            var t = await Task.WhenAny(originTask, cancelTask.Task);

            if (t == cancelTask.Task)
            {
                ct.ThrowIfCancellationRequested();
            }
            await originTask;
        }

    }
}
