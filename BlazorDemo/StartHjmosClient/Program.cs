using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartHjmosClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Process.Start(args[0]);
        }
    }
}
