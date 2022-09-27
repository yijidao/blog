using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3
{
    /// <summary>
    /// 可以使用 Interlocked.CompareExchange 来实现各种情况下的逻辑
    /// 
    /// </summary>
    internal class InterlockedAnything
    {
        public static int Maximum(ref int target, int value)
        {
            int currentVal = target, startVal, desiredVal;
            do
            {
                startVal = currentVal;
                desiredVal = Math.Max(startVal, value);
                if (target == startVal)
                {
                    target = desiredVal; // 这里模拟可能在多线程情况下，target 被其他线程修改的情况
                }
                // Interlocked.CompareExchange(ref location, desireVal, compare) 会比较 location 和 compare，如果相等，则将 location 赋值为 desireVal，并返回location 的初始值，如果不等，则不赋值，并返回 location 初始值
                currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal); 
            } while (currentVal != startVal);

            return desiredVal;
        }
    }
}
