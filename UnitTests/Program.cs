using System;

namespace StorageManager
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitTests.TestingSuite suite = new UnitTests.TestingSuite();
            suite.test();
            Console.ReadKey();
        }
    }
}