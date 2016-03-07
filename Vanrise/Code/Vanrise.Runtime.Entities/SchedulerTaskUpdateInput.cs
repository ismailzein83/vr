using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskUpdateInput
    {
        public byte[] LastUpdateHandle { get; set; }
        public int NbOfRows { get; set; }
        public SchedulerTaskQuery Filter { get; set; }
    }
}
