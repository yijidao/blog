using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// 使用 lock 和双检索实现的单例化
    /// </summary>
    internal class DoubleCheckLocking
    {
        private static DoubleCheckLocking? _value;

        private static readonly object _lock = new();

        private DoubleCheckLocking()
        {

        }

        public static DoubleCheckLocking GetInstance()
        {
            if (_value != null) return _value;
            lock (_lock)
            {
                if (_value == null)
                {
                    var t = new DoubleCheckLocking();
                    Volatile.Write(ref _value, t); 
                }
            }
            return _value;
        }
    }

    /// <summary>
    /// 这个没有使用双检锁，但是其实这个更优雅
    /// </summary>
    internal class DoubleCheckLocking2
    {
        private static DoubleCheckLocking2 _value = new();

        private DoubleCheckLocking2()
        {

        }

        public static DoubleCheckLocking2 GetInstance() => _value;
    }

    /// <summary>
    /// 使用 Interlocked 实现的单例，轻量且简单。
    /// 可能会同时调用多次构造函数，所以适合构造函数没有副作用的类
    /// </summary>
    internal class DoubleCheckLocking3
    {
        private static DoubleCheckLocking3? _value;

        private DoubleCheckLocking3()
        {

        }

        private DoubleCheckLocking3 GetInstance()
        {
            if (_value != null) return _value;
            Interlocked.CompareExchange(ref _value, new DoubleCheckLocking3(), null);
            return _value;
        }
    }
}
