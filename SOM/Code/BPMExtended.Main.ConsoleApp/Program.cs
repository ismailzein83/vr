using BPMExtended.Main.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SOMClient client = new SOMClient())
            {
                var resp = client.Post<TestInput,WFResponse>("", new TestInput());
            }
        }
    }

    public class TestInput
    {
        public TestInputArgument InputArgument { get; set; }
    }

    public class TestInputArgument
    {
        public string CustomerId { get; set; }
    }

    public class WFResponse
    {

    }
}
