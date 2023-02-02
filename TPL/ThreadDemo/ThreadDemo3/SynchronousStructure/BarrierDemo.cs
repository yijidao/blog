using System.Text;

namespace ThreadDemo3.SynchronousStructure;

/// <summary>
/// Barrier 可以通过 participantCount 来指定一个数值，同时会维护一个内部数值 total，每次调用 SignalAndWait() 的时候，阻塞调用线程，同时把total +1，
/// 等到 total == participantCount，调用 postPhaseAction，通过 postPhaseAction 来确定汇总每个线程的数据，并且执行下个阶段的工作。
///
/// Barrier 适合一种特殊场景，把一个大任务拆分成多个小任务，然后每个小任务又会分阶段执行。
/// 像是 Parallel 的 Plus 版，如果任务步骤很多，用 Parallel 来分拆很麻烦，可以考虑用 Barrier。
///
/// 注意点：
/// - 每个步骤执行不一定都会成功，所以可以给 SignalAndWait(int) 指定一个超时时间，超时了就不执行下个步骤了。
/// 
/// </summary>
public class BarrierDemo
{
    public void Test()
    {
        var words = new string[] { "山", "飞", "千", "鸟", "绝" };
        var words2 = new string[] { "人", "灭", "径", "万", "踪" };
        var solution = "千山鸟飞绝，万径人踪灭";
        bool success = false;

        var barrier = new Barrier(2, b =>
        {
            var sb = new StringBuilder();
            sb.Append(string.Concat(words));
            sb.Append('，');
            sb.Append(string.Concat(words2));
            
            Console.WriteLine(sb.ToString());
            //Thread.Sleep(1000);
            if (string.CompareOrdinal(solution, sb.ToString()) == 0)
            {
                success = true;
                Console.WriteLine($"已完成");
            }
            Console.WriteLine($"当前阶段数：{b.CurrentPhaseNumber}");

        });

        var t = Task.Run(() => DoWork(words));
        var t2 = Task.Run(() => DoWork(words2));

        Console.ReadLine();

        void DoWork(string[] words)
        {
            while (!success)
            {
                var r = new Random();
                for (int i = 0; i < words.Length; i++)
                {
                    var swapIndex = r.Next(i, words.Length);
                    (words[swapIndex], words[i]) = (words[i], words[swapIndex]);
                }

                barrier.SignalAndWait();
            }
        }
    }
}