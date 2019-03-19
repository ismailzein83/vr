using StandartLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //Class1 class1 = new Class1();
            //class1.Test();
            CSharpCompilationOutput output;
            Class1.TryCompileClass("erwt", "public class TTCL { }", out output);
        }
    }
}
