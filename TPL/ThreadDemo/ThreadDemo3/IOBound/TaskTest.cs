using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo3.IOBound
{
    internal class TaskTest
    {
        class Type1 { }
        class Type2 { }

        private Task<Type1> GetType1()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(500);
                return new Type1();
            });
        }

        private Task<Type2> GetType2()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(500);
                return new Type2();
            });
        }

        public async Task<string> Test(int argument)
        {
            var local = argument;

            await GetType1();

            for (int i = 0; i < 3; i++)
            {
                await GetType2();
            }

            return "Done";
        }
    }
}
