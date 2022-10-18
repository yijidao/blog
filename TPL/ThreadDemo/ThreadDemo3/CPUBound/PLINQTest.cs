using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.CPUBound
{
    internal class PLINQTest
    {

        public void ObsoleteMethod(Assembly assembly)
        {
            var r = from type in assembly.GetExportedTypes().AsParallel()

                    from method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    let obsoleteAttrType = typeof(ObsoleteAttribute)
                    where Attribute.IsDefined(method, obsoleteAttrType)
                    orderby type.FullName
                    let obsoleteAttrObj = (ObsoleteAttribute)Attribute.GetCustomAttribute(method, obsoleteAttrType)
                    select $"Type={type.FullName}  Method={method}  Message={obsoleteAttrObj.Message}";
            var l = r.ToArray();

            foreach (var s in r)
            {
                Console.WriteLine(s);
            }
        }

        /// <summary>
        /// 每一项都是 CPU Bound 的耗时操作
        /// </summary>
        public void Test2()
        {
            var source = Enumerable.Range(1, 400000);
            var source2 = Enumerable.Range(1, 400000);
            var sw = Stopwatch.StartNew();
            var parallelTest = source.AsParallel().Where(IsPrime).ToList();
            sw.Stop();
            Console.WriteLine($"PLINQ 耗时：[{sw.ElapsedMilliseconds}]"); // PLINQ 耗时：[1725]
            sw = Stopwatch.StartNew();
            var withoutParallelTest = source2.Where(IsPrime).ToList();
            sw.Stop();
            Console.WriteLine($"LINQ 耗时：[{sw.ElapsedMilliseconds}]");// LINQ 耗时：[6317]


            // 素数
            bool IsPrime(int n)
            {
                var m = n / 2;
                for (var i = 2; i <= m; i++)
                {
                    if (n % i == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 每一项都是耗时很短的操作
        /// </summary>
        public void Test3()
        {
            var source = Enumerable.Range(1, 400000000);
            var source2 = Enumerable.Range(1, 400000000);
            var sw = Stopwatch.StartNew();
            var parallelTest = source.AsParallel().Where(x => x % 2 == 0).ToList();
            sw.Stop();
            Console.WriteLine($"PLINQ 耗时：[{sw.ElapsedMilliseconds}]"); // PLINQ 耗时：[10014]
            sw = Stopwatch.StartNew();
            var withoutParallelTest = source2.Where(x => x % 2 == 0).ToList();
            sw.Stop();
            Console.WriteLine($"LINQ 耗时：[{sw.ElapsedMilliseconds}]");// LINQ 耗时：[3292]

        }

    }
}
