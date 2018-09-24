using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class CLIPatternDetail
    {
        public string CLIPattern { get; set; }
        public string Prefix { get; set; }
        public string Destination { get; set; }
        public int? RandMin { get; set; }
        public int? RandMax { get; set; }
        public string DisplayName { get; set; }
    }
}
