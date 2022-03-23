namespace ThreadDemo2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var t = new Test();
            //await t.Start();

            //await t.StartNestTask();
            await t.StartContinueTask();

            Console.ReadKey();
        }
    }

    class Test
    {
        public async Task Start()
        {
            Console.WriteLine($"Start before => {Thread.CurrentThread.ManagedThreadId}");
            await Task.Run(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId));
            Console.WriteLine($"Start end => {Thread.CurrentThread.ManagedThreadId}");
        }

        public async Task StartNestTask()
        {
            Console.WriteLine($"StartNestTask before => { Thread.CurrentThread.ManagedThreadId}");

            await Task.Run(async () =>
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                await Task.Run(() =>
                {
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                });
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            });

            Console.WriteLine($"StartNestTask end => {Thread.CurrentThread.ManagedThreadId}");
        }


        public async Task StartContinueTask()
        {
            Console.WriteLine($"StartContinueTask before => { Thread.CurrentThread.ManagedThreadId}");
            await Task.Run(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId));
            await Task.Run(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId));
            Console.WriteLine($"StartContinueTask end => {Thread.CurrentThread.ManagedThreadId}");

        }
    }
}
