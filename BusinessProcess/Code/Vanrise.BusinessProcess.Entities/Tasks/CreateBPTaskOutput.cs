using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public enum CreateBPTaskResult { Succeeded = 0, Failed = 1 }

    public class CreateBPTaskOutput
    {
        public CreateBPTaskResult Result { get; set; }

        public long TaskId { get; set; }

        public string WFBookmarkName { get; set; }
    }
}
