﻿using System.Diagnostics;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Hello, World!");
                Debug.WriteLine("HelloWorld");
                Task.Delay(1000).Wait();
            }
            
        }
    }
}