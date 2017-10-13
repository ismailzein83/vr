using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class UploadSwitchReleaseCauseLog
    {
        public int CountOfSwitchReleaseCausesAdded { get; set; }
        public int CountOfSwitchReleaseCausesExist { get; set; }

        public long fileID { get; set; }

    }
}
