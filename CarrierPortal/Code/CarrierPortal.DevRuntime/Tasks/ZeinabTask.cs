using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrierPortal.DevRuntime.Tasks
{
    public class ZeinabTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Zeinab Task");

            Console.ReadKey();
        }
    }
}
