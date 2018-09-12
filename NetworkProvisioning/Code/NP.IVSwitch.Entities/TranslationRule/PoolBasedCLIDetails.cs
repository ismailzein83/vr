using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class PoolBasedCLIDetailsCollection : List<PoolBasedCLIDetails>
    {

    }
    public class PoolBasedCLIDetails
    {
        public string CLIPattern { get; set; }
    }
}
