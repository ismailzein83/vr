using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrierPortal.DevRuntime
{
    class Program
    {
        static void Main(string[] args)
        {
            MainForm f = new MainForm();
            f.ShowDialog();
            Console.ReadKey();
            return;

        }
    }
}
