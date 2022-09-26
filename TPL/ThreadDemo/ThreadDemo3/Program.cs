using static System.Net.Mime.MediaTypeNames;

namespace ThreadDemo3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var volatileTest = new VolatileTest();
            volatileTest.Call();
            
        }

        

    }
}