using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupDetail
    {
        public StateBackup Entity { get; set; }

        public String Type { get; set; }

        public String Description { get; set; }
    }
}
