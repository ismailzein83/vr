using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestRuntime
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            while (true)
            {
                Console.WriteLine("hello");
                Thread.Sleep(1000);
            }
        }
    }
}
