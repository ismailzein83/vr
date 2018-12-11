using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTaskUpdateOutput
    {
        public List<BPTaskDetail> ListBPTaskDetails { get; set; }
        public object LastUpdateHandle { get; set; }
    }
}