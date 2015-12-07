using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class Country2
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Breakout> Breakouts { get; set; }
    }
}
