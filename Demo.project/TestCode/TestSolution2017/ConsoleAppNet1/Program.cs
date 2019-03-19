using StandardOnlyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppNet1
{
    class Program
    {
        static void Main(string[] args)
        {
            StandardOnlySqlClass obj1 = new StandardOnlySqlClass();
            obj1.TestSQL();
            //obj1.TestDynamic();
        }
    }
}
