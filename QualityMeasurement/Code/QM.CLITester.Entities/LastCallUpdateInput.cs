using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public class LastCallUpdateInput
    {
        public byte[] LastUpdateHandle { get; set; }
        public long LessThanID { get; set; }
        public int NbOfRows { get; set; }
    }
}
