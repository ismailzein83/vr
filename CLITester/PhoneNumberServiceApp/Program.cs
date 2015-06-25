using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneNumberServiceApp.PhoneNumberWebReference;
namespace PhoneNumberServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            PhoneNumberService pp = new PhoneNumberService(); 
            PhoneNumberReturn ret = pp.RequestForCall("sama", "sama", "415", "01");
            Console.WriteLine(ret.Number);
        }
    }
}
