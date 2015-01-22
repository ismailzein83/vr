using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRuntime
{
    public class RabihTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("Hello from Rabih!");
        }
    }
}
