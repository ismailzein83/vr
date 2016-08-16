using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime;

namespace TOne.WhS.Runtime.Tasks
{
    public class AliAtouiTask : ITask
    {
        public void Execute()
        {
            Console.WriteLine("aaa");
            Console.ReadLine();
        }
    }
}
