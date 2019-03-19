using StandardOnlyLib;
using System;

namespace ConsoleAppCore1
{
    class Program
    {
        static void Main(string[] args)
        {
            StandardOnlySqlClass obj1 = new StandardOnlySqlClass();
            obj1.TestSQL();
            obj1.TestDynamic();
        }
    }
}
