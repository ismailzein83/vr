using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Jazz.BP.Arguments
{
    public class PreviewTaskData : BPTaskData
    {
        public int? FileId { get; set; }
        public int ProcessInstanceId { get; set; }
    }
}
