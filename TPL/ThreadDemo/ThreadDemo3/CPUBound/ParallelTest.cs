using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.CPUBound
{
    internal class ParallelTest
    {
        private void DoWork(int  index = 0)
        {
            Thread.Sleep(500);
            Console.WriteLine(index);
            throw new Exception($"异常 {index}");
        }

        public void Test()
        {
            // 同步调用，可以使用 Parallel 替代
            //for (int i = 0; i < 10; i++)
            //{
            //    DoWork(i);
            //}
            try
            {
                var r = Parallel.For(0, 10, DoWork);
                Console.WriteLine("Parallel.For() Over");

                Parallel.ForEach(Enumerable.Range(10, 10), DoWork);
                Console.WriteLine("Parallel.ForEach() Over");

                Parallel.Invoke(() => DoWork(20), () => DoWork(21), () => DoWork(22));
                Console.WriteLine("Parallel.Invoke() Over");
            }
            catch (AggregateException e)
            {
                Array.ForEach(e.Flatten().InnerExceptions.Select(x => x.Message).ToArray(),Console.WriteLine);
            }
            
        }

        public void Test2()
        {
            var result = 0;

            //Parallel.For(0, 100, (i, loopState) =>
            //{
            //    if(i >=5) loopState.Break();

            //    Console.WriteLine(i);
            //});
            var list = Enumerable.Range(0, 20000).ToArray();
            //var parallelResult = Parallel.ForEach(list, () => 10,
            //    (i, state, arg3) =>
            //    {
            //        //Thread.Sleep(i*100);
            //        Thread.Sleep(500);

            //        //if (i >= 5)
            //        //{
            //        //    //state.Stop();
            //        //    state.Break();
            //        //}

            //        //if (state.ShouldExitCurrentIteration)
            //        //{
            //        //    return 0;
            //        //}

            //        Console.WriteLine($"i:{i} arg3:{arg3}");
            //        return i + arg3;
            //    },
            //    i =>
            //    {
            //        Console.WriteLine($"localFinally: {i}");
            //        Interlocked.Add(ref result, i);
            //    });

            var parallelResult = Parallel.For(0, list.Length, () => 0,(i, state, subtotal) =>
            {
                //Console.WriteLine($"body:[{list[i]}]");
                subtotal += list[i];
                return subtotal;
            }, (x) =>
            {
                Console.WriteLine($"finally:[{x}]");
                Interlocked.Add(ref result, x);
            });

            Console.WriteLine(result); 
            Console.WriteLine($"IsCompleted:[{parallelResult.IsCompleted}]  LowestBreakIteration:[{parallelResult.LowestBreakIteration}]");
        }
    }
}
