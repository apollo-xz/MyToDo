using System;

namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int? a = 42;
            string b = null;
            if (a is int valueOfA)
            {
                Console.WriteLine($"a is {a}");
            }
            else
            {
                Console.WriteLine("a does not have a value");
            }
            // Output:
            // a is 42
        }
    }
}
